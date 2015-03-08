using System.Collections;
using Newtonsoft.Json;
using Our.Umbraco.Ditto.Models.Archetype;
using Our.Umbraco.Ditto.Models.Archetype.Abstract;

namespace Our.Umbraco.Ditto
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Web;

    /// <summary>
    /// Encapsulates extension methods for <see cref="IPublishedContent"/>.
    /// </summary>
    public static class PublishedContentExtensions
    {
        private static readonly ConcurrentDictionary<string, Type> ArchetypeCache
           = new ConcurrentDictionary<string, Type>();

        /// <summary>
        /// The cache for storing constructor parameter information.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, ParameterInfo[]> ConstructorCache
            = new ConcurrentDictionary<Type, ParameterInfo[]>();

        /// <summary>
        /// The cache for storing type property information.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache
            = new ConcurrentDictionary<Type, PropertyInfo[]>();

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
        /// <param name="culture">
        /// The <see cref="CultureInfo"/>
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
            Action<ConvertedTypeEventArgs> convertedType = null,
            CultureInfo culture = null)
            where T : class
        {
            return content.As(typeof(T), convertingType, convertedType, culture) as T;
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
        /// <param name="culture">The <see cref="CultureInfo"/></param>
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
            Action<ConvertedTypeEventArgs> convertedType = null,
            CultureInfo culture = null)
            where T : class
        {
            return items.As(typeof(T), documentTypeAlias, convertingType, convertedType, culture)
                .Select(x => x as T);
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
        /// <param name="culture">The <see cref="CultureInfo"/></param>
        /// <returns>
        /// The converted <see cref="Object"/> as the given type.
        /// </returns>
        public static object As(
            this IPublishedContent content,
            Type type,
            Action<ConvertingTypeEventArgs> convertingType = null,
            Action<ConvertedTypeEventArgs> convertedType = null,
            CultureInfo culture = null)
        {
            if (content == null)
            {
                return null;
            }

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
                object instance = GetTypedProperty(content, type, culture);

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
        }

        /// <summary>
        /// Gets a collection of the given type from the given <see cref="IEnumerable{IPublishedContent}"/>.
        /// </summary>
        /// <param name="items">
        /// The <see cref="IEnumerable{IPublishedContent}"/> to convert.
        /// </param>
        /// <param name="type">
        /// The <see cref="Type"/> of items to return.
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
        /// <param name="culture">
        /// The <see cref="CultureInfo"/>.
        /// </param>
        /// <returns>
        /// The resolved <see cref="IEnumerable{T}"/>.
        /// </returns>
        public static IEnumerable<object> As(
            this IEnumerable<IPublishedContent> items,
            Type type,
            string documentTypeAlias = null,
            Action<ConvertingTypeEventArgs> convertingType = null,
            Action<ConvertedTypeEventArgs> convertedType = null,
            CultureInfo culture = null)
        {
            using (DisposableTimer.DebugDuration<IEnumerable<object>>(string.Format("IEnumerable As ({0})", documentTypeAlias)))
            {
                if (string.IsNullOrWhiteSpace(documentTypeAlias))
                {
                    return items.Select(x => x.As(type, convertingType, convertedType, culture));
                }

                return items
                    .Where(x => documentTypeAlias.InvariantEquals(x.DocumentTypeAlias))
                    .Select(x => x.As(type, convertingType, convertedType, culture));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">
        /// The <see cref="Type"/> to convert too.
        /// </param>
        /// <param name="instance">
        /// The <see cref="object"/> to populate.
        /// </param>
        /// <param name="propertyValue">
        /// The <see cref="object"/> whos property we will populate with.
        /// </param>
        /// <param name="actualPropertyName">
        /// The actual <see cref="string"/> name of the property.
        /// </param>
        /// <param name="propertyInfo"></param>
        /// The <see cref="PropertyInfo"/> of the property to set on the object
        /// <param name="content">
        /// The <see cref="IPublishedContent"/> of which to use as a base for populating content
        /// </param>
        /// <param name="isArchetype">
        /// A <see cref="bool"/> specifying if is of type archetype
        /// </param>
        private static void SetValue(Type type, object instance, object propertyValue, PropertyInfo propertyInfo, IPublishedContent content = null, bool isArchetype = false)
        {
            if (propertyValue != null)
            {
                var propertyType = propertyInfo.PropertyType;
                var typeInfo = propertyType.GetTypeInfo();
                var isEnumerableType = propertyType.IsEnumerableType() &&
                                       typeInfo.GenericTypeArguments.Any();

                // Try any custom type converters first.
                // 1: Check the property.
                // 2: Check any type arguments in generic enumerable types.
                // 3: Check the type itself.

                var converterAttribute =
                    propertyInfo.GetCustomAttribute<TypeConverterAttribute>()
                    ??
                    (isEnumerableType
                         ? typeInfo.GenericTypeArguments.First()
                                   .GetCustomAttribute<TypeConverterAttribute>(true)
                         : propertyType.GetCustomAttribute<TypeConverterAttribute>(true));

                if (isArchetype)
                {
                    /* [ML]: TODO - There must be a better way to do this to avoid an archetype dependency with its model binding to CMS json. */

                    var json = propertyValue is string ? propertyValue as string : JsonConvert.SerializeObject(propertyValue, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

                    var jsonModel = JsonConvert.DeserializeObject<ArchetypeCMSModel>(json);

                    var data = GetTypedArchetype(content, propertyType, jsonModel);

                    propertyInfo.SetValue(instance, data, null);
                }
                else if (converterAttribute != null && content != null)
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
                                var converter = DependencyResolver.Current.GetService(toConvert) as TypeConverter;

                                if (converter != null && converter.CanConvertFrom(propertyValue.GetType()))
                                {
                                    // Create context to pass to converter implementations.
                                    // This contains the IPublishedContent and the currently converting property name.

                                    var descriptor = TypeDescriptor.GetProperties(instance)[propertyInfo.Name];
                                    var context = new PublishedContentContext(content, descriptor);
                                    var culture = UmbracoContext.Current.PublishedContentRequest.Culture;
                                    var converted = converter.ConvertFrom(context, culture, propertyValue);

                                    if (converted != null)
                                    {
                                        // Handle Typeconverters returning single objects when we want an IEnumerable.
                                        // Use case: Someone selects a folder of images rather than a single image with the media picker.

                                        if (isEnumerableType)
                                        {
                                            var parameterType = typeInfo.GenericTypeArguments.First();

                                            // Some converters return an IEnumerable so we check again.
                                            if (!converted.GetType().IsEnumerableType())
                                            {
                                                // Generate a method using 'Cast' to convert the type back to IEnumerable<T>.
                                                MethodInfo castMethod = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(parameterType);
                                                object enumerablePropertyValue = castMethod.Invoke(null, new object[] { converted.YieldSingleItem() });
                                                propertyInfo.SetValue(instance, enumerablePropertyValue, null);
                                            }
                                            else
                                            {
                                                propertyInfo.SetValue(instance, converted, null);
                                            }
                                        }
                                        else
                                        {
                                            // Return single expected items from converters returning an IEnumerable.
                                            if (converted.GetType().IsEnumerableType())
                                            {
                                                // Generate a method using 'FirstOrDefault' to convert the type back to T.
                                                MethodInfo firstMethod = typeof(Enumerable)
                                                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                    .First(
                                                        m =>
                                                        {
                                                            if (m.Name != "FirstOrDefault")
                                                            {
                                                                return false;
                                                            }

                                                            var parameters = m.GetParameters();
                                                            return parameters.Length == 1
                                                                   && parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
                                                        })
                                                    .MakeGenericMethod(propertyType);

                                                object singleValue = firstMethod.Invoke(null, new[] { converted });
                                                propertyInfo.SetValue(instance, singleValue, null);
                                            }
                                            else
                                            {
                                                propertyInfo.SetValue(instance, converted, null);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (propertyInfo.PropertyType == typeof(HtmlString) || propertyInfo.PropertyType == typeof(IHtmlString))
                {
                    // Handle Html strings so we don't have to set the attribute.
                    var converter = new HtmlStringConverter();

                    if (converter.CanConvertFrom(propertyValue.GetType()))
                    {
                        var descriptor = TypeDescriptor.GetProperties(instance)[propertyInfo.Name];
                        var context = new PublishedContentContext(content, descriptor);
                        var culture = UmbracoContext.Current.PublishedContentRequest.Culture;

                        propertyInfo.SetValue(instance, converter.ConvertFrom(context, culture, propertyValue), null);
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

        private static PropertyInfo[] AddToPropertyCache(Type type)
        {
            PropertyInfo[] properties;
            PropertyCache.TryGetValue(type, out properties);

            if (properties == null)
            {
                properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(x => x.CanWrite)
                        .ToArray();

                PropertyCache.TryAdd(type, properties);
            }

            return properties;
        }

        private static object GetTypedArchetype(IPublishedContent content, Type entityType, ArchetypeCMSModel archetype)
        {
            var isGenericList = entityType.IsGenericType && (entityType.GetGenericTypeDefinition() == typeof(IList<>) || entityType.GetGenericTypeDefinition() == typeof(List<>));

            Type archetypeListType = null;

            archetypeListType = isGenericList ? entityType.GetGenericArguments().FirstOrDefault() : entityType;

            if (archetypeListType == null)
            {
                throw new NullReferenceException(string.Format("The type ({0}) can not be inferred?", entityType.Name));
            }

            // ML - Build a generic list from the type fuond above

            var constructedListType = typeof(List<>).MakeGenericType(archetypeListType);
            var list = (IList)Activator.CreateInstance(constructedListType);

            if (archetype != null && (archetype.Fieldsets != null && archetype.Fieldsets.Any()))
            {
                if (archetype.Fieldsets != null && archetype.Fieldsets.Any())
                {
                    foreach (var fieldset in archetype.Fieldsets)
                    {
                        if (fieldset.Properties != null && fieldset.Properties.Any())
                        {
                            Type instanceType = null;

                            ArchetypeCache.TryGetValue(fieldset.Alias, out instanceType);

                            if (instanceType == null)
                            {
                                instanceType = TypeInferenceExtensions.GetTypeByName(fieldset.Alias, typeof(ArchetypeModel)).FirstOrDefault();

                                if (instanceType != null)
                                {
                                    ArchetypeCache.TryAdd(fieldset.Alias, instanceType);

                                    AddToPropertyCache(instanceType);
                                }
                            }

                            if (instanceType != null)
                            {
                                // ML - Create an instance for each archetype object

                                var instance = Activator.CreateInstance(instanceType) as ArchetypeModel;

                                if (instance != null)
                                {
                                    instance.Alias = fieldset.Alias;

                                    PropertyInfo[] properties = null;
                                    PropertyCache.TryGetValue(instanceType, out properties);

                                    if (properties != null && properties.Any())
                                    {
                                        foreach (var property in fieldset.Properties)
                                        {
                                            var propertyInfo = properties.FirstOrDefault(i => i.Name.ToLower() == property.Alias.ToLower());

                                            if (propertyInfo != null)
                                            {
                                                var isArchetype = false;
                                                var umbracoPropertyAttr = propertyInfo.GetCustomAttribute<UmbracoPropertyAttribute>();

                                                if (umbracoPropertyAttr != null)
                                                {
                                                    isArchetype = umbracoPropertyAttr.IsArchetype;
                                                }

                                                SetValue(instanceType, instance, property.Value, propertyInfo, content, isArchetype);
                                            }
                                        }

                                        // [ML] - If this is not a generic type, then return the first item

                                        if (!isGenericList)
                                        {
                                            return instance;
                                        }

                                        list.Add(instance);
                                    }
                                }
                            }
                        }
                    }
                }
            }


            return isGenericList ? list : null;
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
        /// <param name="culture">The <see cref="CultureInfo"/></param>
        /// <returns>
        /// The converted <see cref="Object"/> as the given type.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the given type has invalid constructors.
        /// </exception>
        private static object GetTypedProperty(
            IPublishedContent content,
            Type type,
            CultureInfo culture = null)
        {
            // Check if the culture has been set, otherwise use from Umbraco.
            if (culture == null)
            {
                culture = UmbracoContext.Current.PublishedContentRequest.Culture;
            }

            // Get the default constructor, parameters and create an instance of the type.
            // Try and return from the cache first. TryGetValue is faster than GetOrAdd.
            ParameterInfo[] constructorParams;
            ConstructorCache.TryGetValue(type, out constructorParams);

            if (constructorParams == null)
            {
                var constructor = type.GetConstructors().OrderBy(x => x.GetParameters().Length).First();
                constructorParams = constructor.GetParameters();
                ConstructorCache.TryAdd(type, constructorParams);
            }

            object instance;
            if (constructorParams.Length == 0)
            {
                // Internally this uses Activator.CreateInstance which is heavily optimized.
                instance = type.GetInstance();
            }
            else if (constructorParams.Length == 1 & constructorParams[0].ParameterType == typeof(IPublishedContent))
            {
                // This extension method is about 7x faster than the native implementation.
                instance = type.GetInstance(content);
            }
            else
            {
                throw new InvalidOperationException("Type {0} has invalid constructor parameters");
            }

            // Collect all the properties of the given type and loop through writable ones.
            
            var properties = AddToPropertyCache(type);

            var contentType = content.GetType();

            foreach (var propertyInfo in properties)
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
                    var isArchetype = false;

                    var umbracoPropertyAttr = propertyInfo.GetCustomAttribute<UmbracoPropertyAttribute>();

                    if (umbracoPropertyAttr != null)
                    {
                        umbracoPropertyName = umbracoPropertyAttr.PropertyName;
                        altUmbracoPropertyName = umbracoPropertyAttr.AltPropertyName;
                        recursive = umbracoPropertyAttr.Recursive;
                        defaultValue = umbracoPropertyAttr.DefaultValue;
                        isArchetype = umbracoPropertyAttr.IsArchetype;
                    }

                    // Try fetching the value.
                    var contentProperty = contentType.GetProperty(umbracoPropertyName);
                    object propertyValue = contentProperty != null ? contentProperty.GetValue(content, null) : content.GetPropertyValue(umbracoPropertyName, recursive);

                    // Try fetching the alt value.
                    if ((propertyValue == null || propertyValue.ToString().IsNullOrWhiteSpace()) && !string.IsNullOrWhiteSpace(altUmbracoPropertyName))
                    {
                        contentProperty = contentType.GetProperty(altUmbracoPropertyName);

                        propertyValue = contentProperty != null ? contentProperty.GetValue(content, null) : content.GetPropertyValue(altUmbracoPropertyName, recursive);
                    }

                    // Try setting the default value.
                    if ((propertyValue == null || propertyValue.ToString().IsNullOrWhiteSpace()) && defaultValue != null)
                    {
                        propertyValue = defaultValue;
                    }


                    SetValue(type, instance, propertyValue, propertyInfo, content, isArchetype);
                }
            }

            return instance;
        }
    }
}