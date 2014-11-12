// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublishedContentExtensions.cs" company="Umbrella Inc, Our Umbraco and other contributors">
//   Copyright Umbrella Inc, Our Umbraco and other contributors
// </copyright>
// <summary>
//   Encapsulates extension methods for <see cref="IPublishedContent" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Our.Umbraco.Ditto.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    using Our.Umbraco.Ditto.Attributes;
    using Our.Umbraco.Ditto.EventArgs;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Web;

    /// <summary>
    /// Encapsulates extension methods for <see cref="IPublishedContent"/>.
    /// </summary>
    public static class PublishedContentExtensions
    {
        /// <summary>
        /// Returns the given instance of <see cref="IPublishedContent"/> as the specified type.
        /// </summary>
        /// <param name="content">
        /// The <see cref="IPublishedContent"/> to convert.
        /// </param>
        /// <param name="convertingType">
        /// The <see cref="Action{ConvertingTypeEventArgs}"/> to fire when converting.
        /// </param>
        /// <param name="convertedType">
        /// The <see cref="Action{ConvertedTypeEventArgs}"/> to fire when converted.
        /// </param>
        /// <typeparam name="T">
        /// The <see cref="Type"/> of items to return.
        /// </typeparam>
        /// <returns>
        /// The resolved <see cref="T"/>.
        /// </returns>
        public static T As<T>(
            this IPublishedContent content,
            Action<ConvertingTypeEventArgs> convertingType = null,
            Action<ConvertedTypeEventArgs> convertedType = null)
            where T : class
        {
            return content.As(typeof(T), convertingType, convertedType) as T;
        }

        /// <summary>
        /// Gets a collection of the given type from the given <see cref="IEnumerable{IPublishedContent}"/>.
        /// </summary>
        /// <param name="items">
        /// The <see cref="IEnumerable{IPublishedContent}"/> to convert.
        /// </param>
        /// <param name="documentTypeAlias">
        /// The document type alias.
        /// </param>
        /// <param name="convertingType">
        /// The <see cref="Action{ConvertingTypeEventArgs}"/> to fire when converting.
        /// </param>
        /// <param name="convertedType">
        /// The <see cref="Action{ConvertedTypeEventArgs}"/> to fire when converted.
        /// </param>
        /// <typeparam name="T">
        /// The <see cref="Type"/> of items to return.
        /// </typeparam>
        /// <returns>
        /// The resolved <see cref="IEnumerable{T}"/>.
        /// </returns>
        public static IEnumerable<T> As<T>(
            this IEnumerable<IPublishedContent> items,
            string documentTypeAlias = null,
            Action<ConvertingTypeEventArgs> convertingType = null,
            Action<ConvertedTypeEventArgs> convertedType = null)
            where T : class
        {
            using (DisposableTimer.DebugDuration<IEnumerable<T>>(string.Format("IEnumerable As ({0})", documentTypeAlias)))
            {
                if (string.IsNullOrWhiteSpace(documentTypeAlias))
                {
                    return items.Select(x => x.As<T>(convertingType, convertedType));
                }

                return items
                    .Where(x => documentTypeAlias.InvariantEquals(x.DocumentTypeAlias))
                    .Select(x => x.As<T>(convertingType, convertedType));
            }
        }

        /// <summary>
        /// Returns an object representing the given <see cref="Type"/>.
        /// </summary>
        /// <param name="content">
        /// The <see cref="IPublishedContent"/> to convert.
        /// </param>
        /// <param name="type">
        /// The <see cref="Type"/> of items to return.
        /// </param>
        /// <param name="convertingType">
        /// The <see cref="Action{ConvertingTypeEventArgs}"/> to fire when converting.
        /// </param>
        /// <param name="convertedType">
        /// The <see cref="Action{ConvertedTypeEventArgs}"/> to fire when converted.
        /// </param>
        /// <returns>
        /// The converted <see cref="Object"/> as the given type.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the given type has invalid constructors.
        /// </exception>
        internal static object As(
            this IPublishedContent content,
            Type type,
            Action<ConvertingTypeEventArgs> convertingType = null,
            Action<ConvertedTypeEventArgs> convertedType = null)
        {
            object instance = null;

            if (content == null)
            {
                return instance;
            }

            using (DisposableTimer.DebugDuration(type, string.Format("IPublishedContent As ({0})", content.DocumentTypeAlias), "Complete"))
            {
                ConstructorInfo constructor = type.GetConstructors()
                    .OrderBy(x => x.GetParameters().Length)
                    .First();
                ParameterInfo[] constructorParams = constructor.GetParameters();

                ConvertingTypeEventArgs args1 = new ConvertingTypeEventArgs { Content = content };

                EventHandlers.CallConvertingTypeHandler(args1);

                if (!args1.Cancel && convertingType != null)
                    convertingType(args1);

                if (args1.Cancel)
                    return instance;

                if (constructorParams.Length == 0)
                {
                    instance = Activator.CreateInstance(type);
                }
                else if (constructorParams.Length == 1 & constructorParams[0].ParameterType == typeof(IPublishedContent))
                {
                    instance = Activator.CreateInstance(type, content);
                }
                else
                {
                    throw new InvalidOperationException("Type {0} has invalid constructor parameters");
                }

                PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                Type contentType = content.GetType();

                foreach (PropertyInfo propertyInfo in properties.Where(x => x.CanWrite))
                {
                    using (DisposableTimer.DebugDuration(type, string.Format("ForEach Property ({1} {0})", propertyInfo.Name, content.Id), "Complete"))
                    {
                        // Check for the ignore attribute.
                        DittoIgnoreAttribute ignoreAttr = propertyInfo.GetCustomAttribute<DittoIgnoreAttribute>();
                        if (ignoreAttr != null)
                        {
                            continue;
                        }

                        string umbracoPropertyName = propertyInfo.Name;
                        string altUmbracoPropertyName = string.Empty;
                        bool recursive = false;
                        object defaultValue = null;

                        UmbracoPropertyAttribute umbracoPropertyAttr = propertyInfo.GetCustomAttribute<UmbracoPropertyAttribute>();
                        if (umbracoPropertyAttr != null)
                        {
                            umbracoPropertyName = umbracoPropertyAttr.PropertyName;
                            altUmbracoPropertyName = umbracoPropertyAttr.AltPropertyName;
                            recursive = umbracoPropertyAttr.Recursive;
                            defaultValue = umbracoPropertyAttr.DefaultValue;
                        }

                        // Try fetching the value.
                        PropertyInfo contentProperty = contentType.GetProperty(umbracoPropertyName);
                        object propertyValue = contentProperty != null
                                                ? contentProperty.GetValue(content, null)
                                                : content.GetPropertyValue(umbracoPropertyName, recursive);

                        // Try fetching the alt value.
                        if (propertyValue == null && !string.IsNullOrWhiteSpace(altUmbracoPropertyName))
                        {
                            contentProperty = contentType.GetProperty(altUmbracoPropertyName);
                            propertyValue = contentProperty != null
                                                ? contentProperty.GetValue(content, null)
                                                : content.GetPropertyValue(altUmbracoPropertyName, recursive);
                        }

                        // Try setting the default value.
                        if (propertyValue == null && defaultValue != null)
                        {
                            propertyValue = defaultValue;
                        }

                        // Process the value.
                        if (propertyValue != null)
                        {
                            if (propertyInfo.PropertyType.IsInstanceOfType(propertyValue))
                            {
                                propertyInfo.SetValue(instance, propertyValue, null);
                            }
                            else
                            {
                                using (DisposableTimer.DebugDuration(type, string.Format("TypeConverter ({0}, {1})", content.Id, propertyInfo.Name), "Complete"))
                                {
                                    TypeConverterAttribute converterAttr = propertyInfo.GetCustomAttribute<TypeConverterAttribute>();
                                    if (converterAttr != null)
                                    {
                                        TypeConverter converter = Activator.CreateInstance(Type.GetType(converterAttr.ConverterTypeName)) as TypeConverter;
                                        propertyInfo.SetValue(instance, converter.ConvertFrom(propertyValue), null);
                                    }
                                    else
                                    {
                                        Attempt<object> convert = propertyValue.TryConvertTo(propertyInfo.PropertyType);
                                        if (convert.Success)
                                        {
                                            propertyInfo.SetValue(instance, convert.Result, null);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                ConvertedTypeEventArgs args2 = new ConvertedTypeEventArgs
                {
                    Content = content,
                    Converted = instance,
                    ConvertedType = type
                };

                if (convertedType != null)
                {
                    convertedType(args2);
                }

                EventHandlers.CallConvertedTypeHandler(args2);

                return args2.Converted;
            }
        }
    }
}