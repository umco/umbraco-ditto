using Our.Umbraco.Ditto.Attributes;
using Our.Umbraco.Ditto.ComponentModel;
using Our.Umbraco.Ditto.ComponentModel.ConversionHandlers;

namespace Our.Umbraco.Ditto
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Web;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Web;

    /// <summary>
    /// Encapsulates extension methods for <see cref="IPublishedContent"/>.
    /// </summary>
    public static class PublishedContentExtensions
    {
        /// <summary>
        /// The cache for storing constructor parameter information.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, ParameterInfo[]> ConstructorCache
            = new ConcurrentDictionary<Type, ParameterInfo[]>();

        /// <summary>
        /// The cache for storing type property information.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> VirtualPropertyCache
            = new ConcurrentDictionary<Type, PropertyInfo[]>();

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
        /// <param name="onConverting">
        /// The <see cref="Action{ConversionHandlerContext}"/> to fire when converting.
        /// </param>
        /// <param name="onConverted">
        /// The <see cref="Action{ConversionHandlerContext}"/> to fire when converted.
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
            Action<DittoConversionHandlerContext> onConverting = null,
            Action<DittoConversionHandlerContext> onConverted = null,
            CultureInfo culture = null,
            T instance = null)
            where T : class
        {
            return content.As(typeof(T), onConverting, onConverted, culture, instance) as T;
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
        /// <param name="onConverting">
        /// The <see cref="Action{ConversionHandlerContext}"/> to fire when converting.
        /// </param>
        /// <param name="onConverted">
        /// The <see cref="Action{ConversionHandlerContext}"/> to fire when converted.
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
            Action<DittoConversionHandlerContext> onConverting = null,
            Action<DittoConversionHandlerContext> onConverted = null,
            CultureInfo culture = null)
            where T : class
        {
            return items.As(typeof(T), documentTypeAlias, onConverting, onConverted, culture)
                        .Select(x => x as T);
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
        /// <param name="onConverting">
        /// The <see cref="Action{ConversionHandlerContext}"/> to fire when converting.
        /// </param>
        /// <param name="onConverted">
        /// The <see cref="Action{ConversionHandlerContext}"/> to fire when converted.
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
            Action<DittoConversionHandlerContext> onConverting = null,
            Action<DittoConversionHandlerContext> onConverted = null,
            CultureInfo culture = null)
        {
            using (DisposableTimer.DebugDuration<IEnumerable<object>>(string.Format("IEnumerable As ({0})", documentTypeAlias)))
            {
                IEnumerable<object> typedItems;
                if (string.IsNullOrWhiteSpace(documentTypeAlias))
                {
                    typedItems = items.Select(x => x.As(type, onConverting, onConverted, culture));
                }
                else
                {
                    typedItems = items.Where(x => documentTypeAlias.InvariantEquals(x.DocumentTypeAlias))
                                      .Select(x => x.As(type, onConverting, onConverted, culture));
                }

                // We need to cast back here as nothing is strong typed anymore.
                return (IEnumerable<object>)EnumerableInvocations.Cast(type, typedItems);
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
        /// <param name="onConverting">
        /// The <see cref="Action{ConversionHandlerContext}"/> to fire when converting.
        /// </param>
        /// <param name="onConverted">
        /// The <see cref="Action{ConversionHandlerContext}"/> to fire when converted.
        /// </param>
        /// <param name="culture">The <see cref="CultureInfo"/></param>
        /// <returns>
        /// The converted <see cref="Object"/> as the given type.
        /// </returns>
        public static object As(
            this IPublishedContent content,
            Type type,
            Action<DittoConversionHandlerContext> onConverting = null,
            Action<DittoConversionHandlerContext> onConverted = null,
            CultureInfo culture = null,
            object instance = null)
        {
            if (content == null)
            {
                return null;
            }

            if (instance != null && !type.IsAssignableFrom(instance.GetType()))
            {
                throw new ArgumentException(string.Format("The instance parameter does not implement Type '{0}'" , type.Name), "instance");
            }

            using (DisposableTimer.DebugDuration<object>(string.Format("IPublishedContent As ({0})", content.DocumentTypeAlias), "Complete"))
            {
                return ConvertContent(content, type, onConverting, onConverted, culture, instance);
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
        /// <param name="onConverting">
        /// The <see cref="Action{ConversionHandlerContext}"/> to fire when converting.
        /// </param>
        /// <param name="onConverted">
        /// The <see cref="Action{ConversionHandlerContext}"/> to fire when converted.
        /// </param>
        /// <param name="culture">The <see cref="CultureInfo"/></param>
        /// <returns>
        /// The converted <see cref="Object"/> as the given type.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the given type has invalid constructors.
        /// </exception>
        private static object ConvertContent(
            IPublishedContent content,
            Type type,
            Action<DittoConversionHandlerContext> onConverting = null,
            Action<DittoConversionHandlerContext> onConverted = null,
            CultureInfo culture = null,
            object instance = null)
        {
            // Check if the culture has been set, otherwise use from Umbraco, or fallback to a default
            if (culture == null)
            {
                if (UmbracoContext.Current != null && UmbracoContext.Current.PublishedContentRequest != null)
                {
                    culture = UmbracoContext.Current.PublishedContentRequest.Culture;
                }
                else
                {
                    // Fallback
                    culture = CultureInfo.CurrentCulture;
                }
            }

            // Get the default constructor, parameters and create an instance of the type.
            // Try and return from the cache first. TryGetValue is faster than GetOrAdd.
            ParameterInfo[] constructorParams;
            ConstructorCache.TryGetValue(type, out constructorParams);
            bool hasParameter = false;
            if (constructorParams == null)
            {
                var constructor = type.GetConstructors().OrderBy(x => x.GetParameters().Length).First();
                constructorParams = constructor.GetParameters();
                ConstructorCache.TryAdd(type, constructorParams);
            }

            // If not already an instance, create an instance of the object
            if (instance == null)
            {
                if (constructorParams.Length == 0)
                {
                    // Internally this uses Activator.CreateInstance which is heavily optimized.
                    instance = type.GetInstance();
                }
                else if (constructorParams.Length == 1 & constructorParams[0].ParameterType == typeof(IPublishedContent))
                {
                    // This extension method is about 7x faster than the native implementation.
                    instance = type.GetInstance(content);
                    hasParameter = true;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Can't convert IPublishedContent to {0} as it has no valid constructor. A valid constructor is either an empty one, or one accepting a single IPublishedContent parameter.", type));
                }
            }

            // Collect all the properties of the given type and loop through writable ones.
            PropertyInfo[] virtualProperties;
            PropertyInfo[] nonVirtualProperties;
            VirtualPropertyCache.TryGetValue(type, out virtualProperties);
            PropertyCache.TryGetValue(type, out nonVirtualProperties);
            if (virtualProperties == null && nonVirtualProperties == null)
            {
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                     .Where(x => x.CanWrite).ToArray();

                // Split out the properties.
                virtualProperties = properties.Where(p => p.IsVirtualAndOverridable()).ToArray();
                nonVirtualProperties = properties.Except(virtualProperties).ToArray();
                VirtualPropertyCache.TryAdd(type, virtualProperties);
                PropertyCache.TryAdd(type, nonVirtualProperties);
            }

            // A dictionary to store lazily invoked values.
            var lazyProperties = new Dictionary<string, Lazy<object>>();

            // If there are any virtual properties we want to lazily invoke them.
            if (virtualProperties != null && virtualProperties.Any())
            {
                foreach (var propertyInfo in virtualProperties)
                {
                    using (DisposableTimer.DebugDuration<object>(string.Format("ForEach Virtual Property ({1} {0})", propertyInfo.Name, content.Id), "Complete"))
                    {
                        // Check for the ignore attribute.
                        var ignoreAttr = propertyInfo.GetCustomAttribute<DittoIgnoreAttribute>();
                        if (ignoreAttr != null)
                        {
                            continue;
                        }

                        // Create a Lazy<object> to deferr returning our value.
                        var deferredPropertyInfo = propertyInfo;
                        var localInstance = instance;
                        lazyProperties.Add(
                            propertyInfo.Name,
                            new Lazy<object>(
                                () =>
                                {
                                    // Get the value from Umbraco.
                                    object propertyValue = GetRawValue(content, culture, deferredPropertyInfo, localInstance);
                                    return GetTypedValue(content, culture, deferredPropertyInfo, propertyValue, localInstance);
                                }));
                    }
                }

                // Create a proxy instance to replace our object.
                LazyInterceptor interceptor = new LazyInterceptor(instance, lazyProperties);
                ProxyFactory factory = new ProxyFactory();

                instance = hasParameter
                    ? factory.CreateProxy(type, interceptor, content)
                    : factory.CreateProxy(type, interceptor);
            }

            // We have the instance object but haven't yet populated properties
            // so fire the on converting event handlers
            OnConverting(content, type, instance, onConverting);

            // Now loop through and convert non-virtual properties.
            if (nonVirtualProperties != null && nonVirtualProperties.Any())
            {
                foreach (var propertyInfo in nonVirtualProperties)
                {
                    using (DisposableTimer.DebugDuration<object>(string.Format("ForEach Property ({1} {0})", propertyInfo.Name, content.Id), "Complete"))
                    {
                        // Check for the ignore attribute.
                        var ignoreAttr = propertyInfo.GetCustomAttribute<DittoIgnoreAttribute>();
                        if (ignoreAttr != null)
                        {
                            continue;
                        }

                        // Set the value normally.
                        object propertyValue = GetRawValue(content, culture, propertyInfo, instance);
                        var result = GetTypedValue(content, culture, propertyInfo, propertyValue, instance);
                        propertyInfo.SetValue(instance, result, null);
                    }
                }
            }

            // We have now finished populating the instance object so go ahead
            // and fire the on converted event handlers
            OnConverted(content, type, instance, onConverted);

            return instance;
        }

        /// <summary>
        /// Returns the raw value for the given type and property.
        /// </summary>
        /// <param name="content">The <see cref="IPublishedContent"/> to convert.</param>
        /// <param name="culture">The <see cref="CultureInfo"/></param>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> property info associated with the type.</param>
        /// <returns>The <see cref="object"/> representing the Umbraco value.</returns>
        /// <param name="instance">The instance to assign the value to.</param>
        private static object GetRawValue(
            IPublishedContent content,
            CultureInfo culture,
            PropertyInfo propertyInfo,
            object instance)
        {
            // Check the property for an associated value attribute, otherwise fall-back on expected behaviour.
            var valueAttr = propertyInfo.GetCustomAttribute<DittoValueResolverAttribute>(true)
                ?? new UmbracoPropertyAttribute();

            // TODO: Only create one context and share between GetRawValue and SetTypedValue?
            var descriptor = TypeDescriptor.GetProperties(instance)[propertyInfo.Name];
            var context = new PublishedContentContext(content, descriptor);

            // Time custom value-resolver.
            using (DisposableTimer.DebugDuration<object>(string.Format("Custom ValueResolver ({0}, {1})", content.Id, propertyInfo.Name), "Complete"))
            {
                // Get the value from the custom attribute.
                // TODO: Cache these?

                DittoValueResolver resolver = null;

                // Look for constructor with context param
                var constructor = valueAttr.ResolverType
                    .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(x => x.GetParameters().Length == 1 &&
                        typeof(DittoValueResolverContext).IsAssignableFrom(x.GetParameters()[0].ParameterType));

                if (constructor != null)
                {
                    var contextType = constructor.GetParameters()[0].ParameterType;
                    var resolverCtx = DittoValueResolver.GetRegistedContext(contextType);
                    if (resolverCtx != null)
                    {
                        // Need to run this past James as I'm invoking the constructor myself
                        // rather than going via GetInstance so this isn't getting cached.
                        // Couldn't go via GetInstance though as the parameter type is only known
                        // at runtime so can't use it as a generic arg as required by GetInstance
                        resolver = (DittoValueResolver)constructor.Invoke(new object[] { resolverCtx });
                    }
                }

                if (resolver == null)
                {
                    resolver = (DittoValueResolver)valueAttr.ResolverType.GetInstance();
                }
                
                return resolver.ResolveValue(context, valueAttr, culture);
            }
        }

        /// <summary>
        /// Set the typed value to the given instance.
        /// </summary>
        /// <param name="content">The <see cref="IPublishedContent"/> to convert.</param>
        /// <param name="culture">The <see cref="CultureInfo"/></param>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> property info associated with the type.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="instance">The instance to assign the value to.</param>
        /// <returns>The strong typed converted value for the given property.</returns>
        private static object GetTypedValue(
            IPublishedContent content,
            CultureInfo culture,
            PropertyInfo propertyInfo,
            object propertyValue,
            object instance)
        {
            // Process the value.
            object result = null;
            var propertyType = propertyInfo.PropertyType;
            var typeInfo = propertyType.GetTypeInfo();

            // This should return false against typeof(string) also.
            var propertyIsEnumerableType = propertyType.IsEnumerableType() && typeInfo.GenericTypeArguments.Any();

            // Try any custom type converters first.
            // 1: Check the property.
            // 2: Check any type arguments in generic enumerable types.
            // 3: Check the type itself.
            var converterAttribute =
                propertyInfo.GetCustomAttribute<TypeConverterAttribute>()
                ?? (propertyIsEnumerableType ? typeInfo.GenericTypeArguments.First().GetCustomAttribute<TypeConverterAttribute>(true)
                                             : propertyType.GetCustomAttribute<TypeConverterAttribute>(true));

            if (converterAttribute != null && converterAttribute.ConverterTypeName != null)
            {
                // Time custom conversions.
                using (DisposableTimer.DebugDuration<object>(string.Format("Custom TypeConverter ({0}, {1})", content.Id, propertyInfo.Name), "Complete"))
                {
                    // Get the custom converter from the attribute and attempt to convert.
                    var converterType = Type.GetType(converterAttribute.ConverterTypeName);
                    if (converterType != null)
                    {
                        var converter = converterType.GetDependencyResolvedInstance() as TypeConverter;

                        if (converter != null)
                        {
                            // Create context to pass to converter implementations.
                            // This contains the IPublishedContent and the currently converting property descriptor.
                            var descriptor = TypeDescriptor.GetProperties(instance)[propertyInfo.Name];
                            var context = new PublishedContentContext(content, descriptor);

                            Type propertyValueType = null;
                            if (propertyValue != null)
                            {
                                propertyValueType = propertyValue.GetType();
                            }

                            // We're deliberately passing null.
                            // ReSharper disable once AssignNullToNotNullAttribute
                            if (converter.CanConvertFrom(context, propertyValueType))
                            {
                                object converted = converter.ConvertFrom(context, culture, propertyValue);

                                if (converted != null)
                                {
                                    // Handle Typeconverters returning single objects when we want an IEnumerable.
                                    // Use case: Someone selects a folder of images rather than a single image with the media picker.
                                    var convertedType = converted.GetType();

                                    if (propertyIsEnumerableType)
                                    {
                                        var parameterType = typeInfo.GenericTypeArguments.First();

                                        // Some converters return an IEnumerable so we check again.
                                        if (!convertedType.IsEnumerableType())
                                        {
                                            // Using 'Cast' to convert the type back to IEnumerable<T>.
                                            object enumerablePropertyValue = EnumerableInvocations.Cast(
                                                                                parameterType,
                                                                                converted.YieldSingleItem());

                                            result = enumerablePropertyValue;
                                        }
                                        else
                                        {
                                            // Nothing is strong typed anymore.
                                            result = EnumerableInvocations.Cast(parameterType, (IEnumerable)converted);
                                        }
                                    }
                                    else
                                    {
                                        // Return single expected items from converters returning an IEnumerable.
                                        // Check for string.
                                        if (convertedType.IsEnumerableType() && convertedType.GenericTypeArguments.Any())
                                        {
                                            // Use 'FirstOrDefault' to convert the type back to T.
                                            result = EnumerableInvocations.FirstOrDefault(propertyType, (IEnumerable)converted);
                                        }
                                        else
                                        {
                                            result = converted;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (propertyInfo.PropertyType == typeof(HtmlString))
            {
                // Handle Html strings so we don't have to set the attribute.
                var converterType = typeof(DittoHtmlStringConverter);
                var converter = converterType.GetDependencyResolvedInstance() as TypeConverter;

                if (converter != null)
                {
                    // This contains the IPublishedContent and the currently converting property descriptor.
                    var descriptor = TypeDescriptor.GetProperties(instance)[propertyInfo.Name];
                    var context = new PublishedContentContext(content, descriptor);

                    Type propertyValueType = null;
                    if (propertyValue != null)
                    {
                        propertyValueType = propertyValue.GetType();
                    }

                    // We're deliberately passing null.
                    // ReSharper disable once AssignNullToNotNullAttribute
                    if (converter.CanConvertFrom(context, propertyValueType))
                    {
                        result = converter.ConvertFrom(context, culture, propertyValue);
                    }
                }
            }
            else if (propertyInfo.PropertyType.IsInstanceOfType(propertyValue))
            {
                // Simple types
                result = propertyValue;
            }
            else if (propertyValue is IPublishedContent && propertyInfo.PropertyType.IsClass)
            {
                // If the property value is an IPublishedContent, then we can use Ditto to map to the target type.
                result = ((IPublishedContent)propertyValue).As(propertyInfo.PropertyType);
            }
            else if (propertyValue != null
                && propertyValue.GetType().IsEnumerableOfType(typeof(IPublishedContent))
                && propertyInfo.PropertyType.IsEnumerable()
                && propertyInfo.PropertyType.GetEnumerableType() != null
                && propertyInfo.PropertyType.GetEnumerableType().IsClass)
            {
                // If the property value is IEnumerable<IPublishedContent>, then we can use Ditto to map to the target type.
                result = ((IEnumerable<IPublishedContent>)propertyValue).As(propertyInfo.PropertyType.GetEnumerableType());
            }
            else
            {
                using (DisposableTimer.DebugDuration<object>(string.Format("TypeConverter ({0}, {1})", content.Id, propertyInfo.Name), "Complete"))
                {
                    var convert = propertyValue.TryConvertTo(propertyInfo.PropertyType);
                    if (convert.Success)
                    {
                        result = convert.Result;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Fires off the various on converting events.
        /// </summary>
        /// <param name="content">The <see cref="IPublishedContent"/> to convert.</param>
        /// <param name="type">The instance type.</param>
        /// <param name="instance">The instance to assign the value to.</param>
        /// <param name="callback">
        /// The <see cref="Action{ConversionHandlerContext}"/> to fire when converting.
        /// </param>
        private static void OnConverting(IPublishedContent content,
            Type type,
            object instance,
            Action<DittoConversionHandlerContext> callback = null)
        {
            // Trigger conversion handlers
            var conversionCtx = new DittoConversionHandlerContext
            {
                Content = content,
                ModelType = type,
                Model = instance
            };

            // Check for class level DittoOnConvertedAttributes
            foreach (var attr in type.GetCustomAttributes<DittoConversionHandlerAttribute>())
            {
                ((DittoConversionHandler)attr.HandlerType.GetInstance(conversionCtx)).OnConverting();
            }

            // Check for method level DittoOnConvertedAttributes
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.GetCustomAttribute<DittoOnConvertingAttribute>() != null))
            {
                var p = method.GetParameters();
                if (p.Length == 1 && p[0].ParameterType == typeof(DittoConversionHandlerContext))
                {
                    method.Invoke(instance, new[] { conversionCtx });
                }
            }

            // Check for a callback function
            if (callback != null)
            {
                callback(conversionCtx);
            }
        }

        /// <summary>
        /// Fires off the various on converted events.
        /// </summary>
        /// <param name="content">The <see cref="IPublishedContent"/> to convert.</param>
        /// <param name="type">The instance type.</param>
        /// <param name="instance">The instance to assign the value to.</param>
        /// <param name="callback">
        /// The <see cref="Action{ConversionHandlerContext}"/> to fire when converted.
        /// </param>
        private static void OnConverted(IPublishedContent content,
            Type type,
            object instance,
            Action<DittoConversionHandlerContext> callback = null)
        {
            // Trigger conversion handlers
            var conversionCtx = new DittoConversionHandlerContext
            {
                Content = content,
                ModelType = type,
                Model = instance
            };

            // Check for class level DittoOnConvertedAttributes
            foreach (var attr in type.GetCustomAttributes<DittoConversionHandlerAttribute>())
            {
                ((DittoConversionHandler)attr.HandlerType.GetInstance(conversionCtx)).OnConverted();
            }

            // Check for method level DittoOnConvertedAttributes
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.GetCustomAttribute<DittoOnConvertedAttribute>() != null))
            {
                var p = method.GetParameters();
                if (p.Length == 1 && p[0].ParameterType == typeof(DittoConversionHandlerContext))
                {
                    method.Invoke(instance, new[] { conversionCtx });
                }
            }

            // Check for a callback function
            if (callback != null)
            {
                callback(conversionCtx);
            }
        }
    }
}