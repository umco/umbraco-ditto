using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Ditto
{
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
        /// <param name="culture">
        /// The <see cref="CultureInfo"/>
        /// </param>
        /// <param name="instance">
        /// An existing instance of T to populate
        /// </param>
        /// <param name="processorContexts">
        /// A collection of <see cref="DittoProcessorContext"/> entities to use whilst processing values.
        /// </param>
        /// <param name="onConverting">
        /// The <see cref="Action{ConversionHandlerContext}"/> to fire when converting.
        /// </param>
        /// <param name="onConverted">
        /// The <see cref="Action{ConversionHandlerContext}"/> to fire when converted.
        /// </param>
        /// <typeparam name="T">
        /// The <see cref="Type"/> of items to return.
        /// </typeparam>
        /// <returns>
        /// The converted generic <see cref="Type"/>.
        /// </returns>
        public static T As<T>(
            this IPublishedContent content,
            CultureInfo culture = null,
            T instance = null,
            IEnumerable<DittoProcessorContext> processorContexts = null,
            Action<DittoConversionHandlerContext> onConverting = null,
            Action<DittoConversionHandlerContext> onConverted = null)
            where T : class
        {
            return content.As(typeof(T), culture, instance, processorContexts, onConverting, onConverted) as T;
        }

        /// <summary>
        /// Gets a collection of the given type from the given <see cref="IEnumerable{IPublishedContent}"/>.
        /// </summary>
        /// <param name="items">
        /// The <see cref="IEnumerable{IPublishedContent}"/> to convert.
        /// </param>
        /// <param name="culture">The <see cref="CultureInfo"/></param>
        /// <param name="processorContexts">
        /// A collection of <see cref="DittoProcessorContext"/> entities to use whilst processing values.
        /// </param>
        /// <param name="onConverting">
        /// The <see cref="Action{ConversionHandlerContext}"/> to fire when converting.
        /// </param>
        /// <param name="onConverted">
        /// The <see cref="Action{ConversionHandlerContext}"/> to fire when converted.
        /// </param>
        /// <typeparam name="T">
        /// The <see cref="Type"/> of items to return.
        /// </typeparam>
        /// <returns>
        /// The converted <see cref="IEnumerable{T}"/>.
        /// </returns>
        public static IEnumerable<T> As<T>(
            this IEnumerable<IPublishedContent> items,
            CultureInfo culture = null,
            IEnumerable<DittoProcessorContext> processorContexts = null,
            Action<DittoConversionHandlerContext> onConverting = null,
            Action<DittoConversionHandlerContext> onConverted = null)
            where T : class
        {
            return items.As(typeof(T), culture, processorContexts, onConverting, onConverted)
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
        /// <param name="culture">
        /// The <see cref="CultureInfo"/>.
        /// </param>
        /// <param name="processorContexts">
        /// A collection of <see cref="DittoProcessorContext"/> entities to use whilst processing values.
        /// </param>
        /// <param name="onConverting">
        /// The <see cref="Action{ConversionHandlerContext}"/> to fire when converting.
        /// </param>
        /// <param name="onConverted">
        /// The <see cref="Action{ConversionHandlerContext}"/> to fire when converted.
        /// </param>
        /// <returns>
        /// The converted <see cref="IEnumerable{T}"/>.
        /// </returns>
        public static IEnumerable<object> As(
            this IEnumerable<IPublishedContent> items,
            Type type,
            CultureInfo culture = null,
            IEnumerable<DittoProcessorContext> processorContexts = null,
            Action<DittoConversionHandlerContext> onConverting = null,
            Action<DittoConversionHandlerContext> onConverted = null)
        {
            using (DittoDisposableTimer.DebugDuration<IEnumerable<object>>("IEnumerable As"))
            {
                var typedItems = items.Select(x => x.As(type, culture, null, processorContexts, onConverting, onConverted));

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
        /// <param name="culture">
        /// The <see cref="CultureInfo"/>
        /// </param>
        /// <param name="instance">
        /// An existing instance of T to populate
        /// </param>
        /// <param name="processorContexts">
        /// A collection of <see cref="DittoProcessorContext"/> entities to use whilst processing values.
        /// </param>
        /// <param name="onConverting">
        /// The <see cref="Action{ConversionHandlerContext}"/> to fire when converting.
        /// </param>
        /// <param name="onConverted">
        /// The <see cref="Action{ConversionHandlerContext}"/> to fire when converted.
        /// </param>
        /// <returns>
        /// The converted <see cref="Object"/> as the given type.
        /// </returns>
        public static object As(
            this IPublishedContent content,
            Type type,
            CultureInfo culture = null,
            object instance = null,
            IEnumerable<DittoProcessorContext> processorContexts = null,
            Action<DittoConversionHandlerContext> onConverting = null,
            Action<DittoConversionHandlerContext> onConverted = null)
        {
            // Ensure content
            if (content == null)
            {
                return null;
            }

            // Ensure instance is of target type
            if (instance != null && !type.IsInstanceOfType(instance))
            {
                throw new ArgumentException(string.Format("The instance parameter does not implement Type '{0}'", type.Name), "instance");
            }

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

            // Convert
            using (DittoDisposableTimer.DebugDuration<object>(string.Format("IPublishedContent As ({0})", content.DocumentTypeAlias)))
            {
                var cacheAttr = type.GetCustomAttribute<DittoCacheAttribute>(true);
                if (cacheAttr != null)
                {
                    var ctx = new DittoCacheContext(cacheAttr, content, type, culture);
                    return cacheAttr.GetCacheItem(ctx, () => ConvertContent(content, type, culture, instance, processorContexts, onConverting, onConverted));
                }
                else
                {
                    return ConvertContent(content, type, culture, instance, processorContexts, onConverting, onConverted);
                }
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
        /// <param name="culture">
        /// The <see cref="CultureInfo"/>
        /// </param>
        /// <param name="instance">
        /// An existing instance of T to populate
        /// </param>
        /// <param name="processorContexts">
        /// A collection of <see cref="DittoProcessorContext"/> entities to use whilst processing values.
        /// </param>
        /// <param name="onConverting">
        /// The <see cref="Action{ConversionHandlerContext}"/> to fire when converting.
        /// </param>
        /// <param name="onConverted">
        /// The <see cref="Action{ConversionHandlerContext}"/> to fire when converted.
        /// </param>
        /// <returns>
        /// The converted <see cref="Object"/> as the given type.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the given type has invalid constructors.
        /// </exception>
        private static object ConvertContent(
            IPublishedContent content,
            Type type,
            CultureInfo culture = null,
            object instance = null,
            IEnumerable<DittoProcessorContext> processorContexts = null,
            Action<DittoConversionHandlerContext> onConverting = null,
            Action<DittoConversionHandlerContext> onConverted = null)
        {
            // Get the default constructor, parameters and create an instance of the type.
            // Try and return from the cache first. TryGetValue is faster than GetOrAdd.
            ParameterInfo[] constructorParams;
            ConstructorCache.TryGetValue(type, out constructorParams);
            bool hasParameter = false;
            if (constructorParams == null)
            {
                var constructor = type.GetConstructors().OrderBy(x => x.GetParameters().Length).FirstOrDefault();
                if (constructor != null)
                {
                    constructorParams = constructor.GetParameters();
                    ConstructorCache.TryAdd(type, constructorParams);
                }
            }

            // If not already an instance, create an instance of the object
            if (instance == null)
            {
                if (constructorParams != null && constructorParams.Length == 0)
                {
                    // Internally this uses Activator.CreateInstance which is heavily optimized.
                    instance = type.GetInstance();
                }
                else if (constructorParams != null && constructorParams.Length == 1 & constructorParams[0].ParameterType == typeof(IPublishedContent))
                {
                    // This extension method is about 7x faster than the native implementation.
                    instance = type.GetInstance(content);
                    hasParameter = true;
                }
                else
                {
                    // No valid constructor, but see if the value can be cast to the type
                    if (type.IsInstanceOfType(content))
                    {
                        instance = content;
                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format("Can't convert IPublishedContent to {0} as it has no valid constructor. A valid constructor is either an empty one, or one accepting a single IPublishedContent parameter.", type));
                    }
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

            // Gets the default processor for this conversion.
            var defaultProcessorType = DittoProcessorRegistry.Instance.GetDefaultProcessorType(type);

            // A dictionary to store lazily invoked values.
            var lazyProperties = new Dictionary<string, Lazy<object>>();

            // If there are any virtual properties we want to lazily invoke them.
            if (virtualProperties != null && virtualProperties.Any())
            {
                foreach (var propertyInfo in virtualProperties)
                {
                    using (DittoDisposableTimer.DebugDuration<object>(string.Format("ForEach Virtual Property ({1} {0})", propertyInfo.Name, content.Id)))
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

                        lazyProperties.Add(propertyInfo.Name, new Lazy<object>(() => GetProcessedValue(content, culture, type, deferredPropertyInfo, localInstance, defaultProcessorType, processorContexts)));
                    }
                }

                // Create a proxy instance to replace our object.
                var interceptor = new LazyInterceptor(instance, lazyProperties);
                var factory = new ProxyFactory();

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
                    using (DittoDisposableTimer.DebugDuration<object>(string.Format("ForEach Property ({1} {0})", propertyInfo.Name, content.Id)))
                    {
                        // Check for the ignore attribute.
                        var ignoreAttr = propertyInfo.GetCustomAttribute<DittoIgnoreAttribute>();
                        if (ignoreAttr != null)
                        {
                            continue;
                        }

                        // Set the value normally.
                        // ReSharper disable once PossibleMultipleEnumeration
                        var value = GetProcessedValue(content, culture, type, propertyInfo, instance, defaultProcessorType, processorContexts);

                        // This is 2x as fast as propertyInfo.SetValue(instance, value, null);
                        PropertyInfoInvocations.SetValue(propertyInfo, instance, value);
                    }
                }
            }

            // We have now finished populating the instance object so go ahead
            // and fire the on converted event handlers
            OnConverted(content, type, instance, onConverted);

            return instance;
        }

        /// <summary>
        /// Returns the processed value for the given type and property.
        /// </summary>
        /// <param name="content">The <see cref="IPublishedContent" /> to convert.</param>
        /// <param name="culture">The <see cref="CultureInfo" /></param>
        /// <param name="targetType">The target type.</param>
        /// <param name="propertyInfo">The <see cref="PropertyInfo" /> property info associated with the type.</param>
        /// <param name="instance">The instance to assign the value to.</param>
        /// <param name="defaultProcessorType">The default processor type.</param>
        /// <param name="processorContexts">A collection of <see cref="DittoProcessorContext" /> entities to use whilst processing values.</param>
        /// <returns>
        /// The <see cref="object" /> representing the Umbraco value.
        /// </returns>
        private static object GetProcessedValue(
            IPublishedContent content,
            CultureInfo culture,
            Type targetType,
            PropertyInfo propertyInfo,
            object instance,
            Type defaultProcessorType,
            IEnumerable<DittoProcessorContext> processorContexts = null)
        {
            // Time custom value-processor.
            using (DittoDisposableTimer.DebugDuration<object>(string.Format("Custom ValueProcessor ({0}, {1})", content.Id, propertyInfo.Name)))
            {
                // Get the target property description
                var propertyDescriptor = TypeDescriptor.GetProperties(instance)[propertyInfo.Name];

                // Check for cache attribute
                var cacheAttr = propertyInfo.GetCustomAttribute<DittoCacheAttribute>(true);
                if (cacheAttr != null)
                {
                    var ctx = new DittoCacheContext(cacheAttr, content, targetType, propertyDescriptor, culture);
                    return cacheAttr.GetCacheItem(ctx, () => DoGetProcessedValue(content, culture, targetType, propertyInfo, propertyDescriptor, defaultProcessorType, processorContexts));
                }
                else
                {
                    return DoGetProcessedValue(content, culture, targetType, propertyInfo, propertyDescriptor, defaultProcessorType, processorContexts);
                }
            }
        }

        /// <summary>
        /// Returns the processed value for the given type and property.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <param name="defaultProcessorType">The default processor type.</param>
        /// <param name="processorContexts">The processor contexts.</param>
        /// <returns></returns>
        private static object DoGetProcessedValue(
            IPublishedContent content,
            CultureInfo culture,
            Type targetType,
            PropertyInfo propertyInfo,
            PropertyDescriptor propertyDescriptor,
            Type defaultProcessorType,
            IEnumerable<DittoProcessorContext> processorContexts = null)
        {
            // Check the property for any explicit processor attributes
            var processorAttrs = propertyInfo.GetCustomAttributes<DittoProcessorAttribute>(true)
                .OrderBy(x => x.Order)
                .ToList();

            if (!processorAttrs.Any())
            {
                // Adds the default processor attribute
                processorAttrs.Add((DittoProcessorAttribute)defaultProcessorType.GetInstance());
            }

            // Check for type registered processors
            processorAttrs.AddRange(propertyInfo.PropertyType.GetCustomAttributes<DittoProcessorAttribute>(true)
                .OrderBy(x => x.Order));

            // Check for globally registered processors
            processorAttrs.AddRange(DittoProcessorRegistry.Instance.GetRegisteredProcessorAttributesFor(propertyInfo.PropertyType));

            // Add any core processors onto the end
            processorAttrs.AddRange(DittoProcessorRegistry.Instance.GetPostProcessorAttributes());

            // Create holder for value as it's processed
            object currentValue = content;

            // Create a processor context cache
            var processorContextsCache = new DittoProcessorContextCache(content, targetType, propertyDescriptor, culture);

            // Add a multi processor context by default
            processorContextsCache.AddContext(new DittoMultiProcessorContext { ContextCache = processorContextsCache });

            // Add the passed in contexts
            processorContextsCache.AddContexts(processorContexts);

            // Process attributes
            foreach (var processorAttr in processorAttrs)
            {
                // Get the right context type
                var ctx = processorContextsCache.GetOrCreateContext(processorAttr.ContextType);

                // Process value
                currentValue = processorAttr.ProcessValue(currentValue, ctx);
            }

            return currentValue;
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
        private static void OnConverting(
            IPublishedContent content,
            Type type,
            object instance,
            Action<DittoConversionHandlerContext> callback = null)
        {

            OnConvert<DittoOnConvertingAttribute>(
                DittoConversionHandlerType.OnConverting,
                content,
                type,
                instance,
                callback);
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
        private static void OnConverted(
            IPublishedContent content,
            Type type,
            object instance,
            Action<DittoConversionHandlerContext> callback = null)
        {
            OnConvert<DittoOnConvertedAttribute>(
                DittoConversionHandlerType.OnConverted,
                content,
                type,
                instance,
                callback);
        }

        /// <summary>
        /// Convenience method for calling converting/converter handlers.
        /// </summary>
        /// <typeparam name="TAttributeType">The type of the attribute type.</typeparam>
        /// <param name="conversionType">Type of the conversion.</param>
        /// <param name="content">The content.</param>
        /// <param name="type">The type.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="callback">The callback.</param>
        private static void OnConvert<TAttributeType>(
            DittoConversionHandlerType conversionType,
            IPublishedContent content,
            Type type,
            object instance,
            Action<DittoConversionHandlerContext> callback = null)
            where TAttributeType : Attribute
        {
            // Trigger conversion handlers
            var conversionCtx = new DittoConversionHandlerContext
            {
                Content = content,
                ModelType = type,
                Model = instance
            };

            // Check for class level DittoOnConvertedAttribute
            foreach (var attr in type.GetCustomAttributes<DittoConversionHandlerAttribute>())
            {
                ((DittoConversionHandler)attr.HandlerType.GetInstance())
                    .Run(conversionCtx, conversionType);
            }

            // Check for globaly registered handlers
            foreach (var handlerType in DittoConversionHandlerRegistry.Instance.GetRegisteredHandlerTypesFor(type))
            {
                ((DittoConversionHandler)handlerType.GetInstance())
                    .Run(conversionCtx, conversionType);
            }

            // Check for method level DittoOnConvertedAttribute
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.GetCustomAttribute<TAttributeType>() != null))
            {
                var p = method.GetParameters();
                if (p.Length == 1 && p[0].ParameterType == typeof(DittoConversionHandlerContext))
                {
                    method.Invoke(instance, new object[] { conversionCtx });
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