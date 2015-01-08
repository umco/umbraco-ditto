namespace Our.Umbraco.Ditto
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

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
        internal static object As(this IPublishedContent content, Type type, Action<ConvertingTypeEventArgs> convertingType = null, Action<ConvertedTypeEventArgs> convertedType = null)
        {
            if (content == null)
            {
                return null;
            }

            // HACK: [LK:2014-07-24] Using the `RequestCache` to store the result, as the model-factory can be called multiple times per request.
            // The cache should only contain models have a corresponding converter, so in theory the result should be the same.
            // Reason for caching, is that Ditto uses reflection to set property values, this can be a performance hit (especially when called multiple times).
            //
            // Update [JS:2015-01-08] Moved so that non model-factory implementations can use the cache and included the type name in the  
            // cache key in case you are returning a base type and inherited type within the same request.
            return ApplicationContext.Current.ApplicationCache.RequestCache.GetCacheItem(
                    string.Format("Ditto.CreateModel_{0}_{1}", type.Name, content.Path),
                    () =>
                    {
                        using (DisposableTimer.DebugDuration(type, string.Format("IPublishedContent As ({0})", content.DocumentTypeAlias), "Complete"))
                        {
                            // Check for and fire any event args
                            var convertingArgs = new ConvertingTypeEventArgs
                            {
                                Content = content
                            };

                            EventHandlers.CallConvertingTypeHandler(convertingArgs);

                            if (!convertingArgs.Cancel && convertingType != null)
                            {
                                convertingType(convertingArgs);
                            }

                            // Cancel if applicable. 
                            if (convertingArgs.Cancel)
                            {
                                return null;
                            }

                            // Create an object and fetch it as the type.
                            object instance = GetTypedProperty(content, type);

                            // Fire the converted event
                            var convertedArgs = new ConvertedTypeEventArgs
                            {
                                Content = content,
                                Converted = instance,
                                ConvertedType = type
                            };

                            if (convertedType != null)
                            {
                                convertedType(convertedArgs);
                            }

                            EventHandlers.CallConvertedTypeHandler(convertedArgs);

                            return convertedArgs.Converted;
                        }
                    });
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
        /// <returns>
        /// The converted <see cref="Object"/> as the given type.
        /// </returns>
        private static object GetTypedProperty(IPublishedContent content, Type type)
        {
            var constructor = type.GetConstructors()
                    .OrderBy(x => x.GetParameters().Length)
                    .First();
            var constructorParams = constructor.GetParameters();

            object instance;
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

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var contentType = content.GetType();

            foreach (var propertyInfo in properties.Where(x => x.CanWrite))
            {
                using (DisposableTimer.DebugDuration(type, string.Format("ForEach Property ({1} {0})", propertyInfo.Name, content.Id), "Complete"))
                {
                    // Check for the ignore attribute.
                    var ignoreAttr = propertyInfo.GetCustomAttribute<DittoIgnoreAttribute>();
                    if (ignoreAttr != null)
                    {
                        continue;
                    }

                    var umbracoPropertyName = propertyInfo.Name;
                    var altUmbracoPropertyName = string.Empty;
                    var recursive = false;
                    object defaultValue = null;

                    var umbracoPropertyAttr = propertyInfo.GetCustomAttribute<UmbracoPropertyAttribute>();
                    if (umbracoPropertyAttr != null)
                    {
                        umbracoPropertyName = umbracoPropertyAttr.PropertyName;
                        altUmbracoPropertyName = umbracoPropertyAttr.AltPropertyName;
                        recursive = umbracoPropertyAttr.Recursive;
                        defaultValue = umbracoPropertyAttr.DefaultValue;
                    }

                    // Try fetching the value.
                    var contentProperty = contentType.GetProperty(umbracoPropertyName);
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
                        // Try any custom type converters first. Check both on class or property.
                        var converterAttribute = propertyInfo.GetCustomAttribute<TypeConverterAttribute>() ??
                            (TypeConverterAttribute)propertyInfo.PropertyType.GetCustomAttributes().FirstOrDefault(a => a is TypeConverterAttribute);

                        if (converterAttribute != null)
                        {
                            if (converterAttribute.ConverterTypeName != null)
                            {
                                // Time custom conversions.
                                using (DisposableTimer.DebugDuration(type, string.Format("Custom TypeConverter ({0}, {1})", content.Id, propertyInfo.Name), "Complete"))
                                {
                                    // Get the custom converter from the attribute and attempt to convert.
                                    var toConvert = Type.GetType(converterAttribute.ConverterTypeName);
                                    if (toConvert != null)
                                    {
                                        var converter = Activator.CreateInstance(toConvert) as TypeConverter;
                                        if (converter != null)
                                        {
                                            propertyInfo.SetValue(instance, converter.ConvertFrom(propertyValue), null);
                                        }
                                    }
                                }
                            }
                        }
                        else if (propertyInfo.PropertyType.IsInstanceOfType(propertyValue))
                        {
                            // Simple types
                            propertyInfo.SetValue(instance, propertyValue, null);
                        }
                        else
                        {
                            using (DisposableTimer.DebugDuration(type, string.Format("TypeConverter ({0}, {1})", content.Id, propertyInfo.Name), "Complete"))
                            {
                                var convert = propertyValue.TryConvertTo(propertyInfo.PropertyType);
                                if (convert.Success)
                                {
                                    propertyInfo.SetValue(instance, convert.Result, null);
                                }
                            }
                        }
                    }
                }
            }

            return instance;
        }
    }
}