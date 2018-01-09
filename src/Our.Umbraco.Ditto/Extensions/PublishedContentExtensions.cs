using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Encapsulates extension methods for <see cref="IPublishedContent"/>.
    /// </summary>
    public static class PublishedContentExtensions
    {
        /// <summary>
        /// The cache for storing type property information.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache
            = new ConcurrentDictionary<Type, PropertyInfo[]>();

        /// <summary>Returns the given instance of <see cref="IPublishedContent"/> as the specified type.</summary>
        /// <param name="content">The <see cref="IPublishedContent"/> to convert.</param>
        /// <param name="culture">The <see cref="CultureInfo"/></param>
        /// <param name="instance">An existing instance of T to populate</param>
        /// <param name="processorContexts">A collection of <see cref="DittoProcessorContext"/> entities to use whilst processing values.</param>
        /// <param name="onConverting">The <see cref="Action{ConversionHandlerContext}"/> to fire when converting.</param>
        /// <param name="onConverted">The <see cref="Action{ConversionHandlerContext}"/> to fire when converted.</param>
        /// <param name="chainContext">The <see cref="DittoChainContext"/> for the current processor chain.</param>
        /// <typeparam name="T">The <see cref="Type"/> of items to return.</typeparam>
        /// <returns>The converted generic <see cref="Type"/>.</returns>
        public static T As<T>(
            this IPublishedContent content,
            CultureInfo culture = null,
            T instance = null,
            IEnumerable<DittoProcessorContext> processorContexts = null,
            Action<DittoConversionHandlerContext> onConverting = null,
            Action<DittoConversionHandlerContext> onConverted = null,
            DittoChainContext chainContext = null)
            where T : class
        {
            return content.As(typeof(T), culture, instance, processorContexts, onConverting, onConverted, chainContext) as T;
        }

        /// <summary>Gets a collection of the given type from the given <see cref="IEnumerable{IPublishedContent}"/>.</summary>
        /// <param name="items">The <see cref="IEnumerable{IPublishedContent}"/> to convert.</param>
        /// <param name="culture">The <see cref="CultureInfo"/></param>
        /// <param name="processorContexts">A collection of <see cref="DittoProcessorContext"/> entities to use whilst processing values.</param>
        /// <param name="onConverting">The <see cref="Action{ConversionHandlerContext}"/> to fire when converting.</param>
        /// <param name="onConverted">The <see cref="Action{ConversionHandlerContext}"/> to fire when converted.</param>
        /// <param name="chainContext">The <see cref="DittoChainContext"/> for the current processor chain.</param>
        /// <typeparam name="T">The <see cref="Type"/> of items to return.</typeparam>
        /// <returns>The converted <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<T> As<T>(
            this IEnumerable<IPublishedContent> items,
            CultureInfo culture = null,
            IEnumerable<DittoProcessorContext> processorContexts = null,
            Action<DittoConversionHandlerContext> onConverting = null,
            Action<DittoConversionHandlerContext> onConverted = null,
            DittoChainContext chainContext = null)
            where T : class
        {
            return items.As(typeof(T), culture, processorContexts, onConverting, onConverted, chainContext).Select(x => x as T);
        }

        /// <summary>Gets a collection of the given type from the given <see cref="IEnumerable{IPublishedContent}"/>.</summary>
        /// <param name="items">The <see cref="IEnumerable{IPublishedContent}"/> to convert.</param>
        /// <param name="type">The <see cref="Type"/> of items to return.</param>
        /// <param name="culture">The <see cref="CultureInfo"/>.</param>
        /// <param name="processorContexts">A collection of <see cref="DittoProcessorContext"/> entities to use whilst processing values.</param>
        /// <param name="onConverting">The <see cref="Action{ConversionHandlerContext}"/> to fire when converting.</param>
        /// <param name="onConverted">The <see cref="Action{ConversionHandlerContext}"/> to fire when converted.</param>
        /// <param name="chainContext">The <see cref="DittoChainContext"/> for the current processor chain.</param>
        /// <returns>The converted <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<object> As(
            this IEnumerable<IPublishedContent> items,
            Type type,
            CultureInfo culture = null,
            IEnumerable<DittoProcessorContext> processorContexts = null,
            Action<DittoConversionHandlerContext> onConverting = null,
            Action<DittoConversionHandlerContext> onConverted = null,
            DittoChainContext chainContext = null)
        {
            using (DittoDisposableTimer.DebugDuration(typeof(Ditto), $"IEnumerable As<{type.Name}>"))
            {
                var typedItems = items.Select(x => x.As(type, culture, null, processorContexts, onConverting, onConverted, chainContext));

                // We need to cast back here as nothing is strong typed anymore.
                return (IEnumerable<object>)EnumerableInvocations.Cast(type, typedItems);
            }
        }

        /// <summary>Returns an object representing the given <see cref="Type"/>.</summary>
        /// <param name="content">The <see cref="IPublishedContent"/> to convert.</param>
        /// <param name="type">The <see cref="Type"/> of items to return.</param>
        /// <param name="culture">The <see cref="CultureInfo"/></param>
        /// <param name="instance">An existing instance of T to populate</param>
        /// <param name="processorContexts">A collection of <see cref="DittoProcessorContext"/> entities to use whilst processing values.</param>
        /// <param name="onConverting">The <see cref="Action{ConversionHandlerContext}"/> to fire when converting.</param>
        /// <param name="onConverted">The <see cref="Action{ConversionHandlerContext}"/> to fire when converted.</param>
        /// <param name="chainContext">The <see cref="DittoChainContext"/> for the current processor chain.</param>
        /// <returns>The converted <see cref="Object"/> as the given type.</returns>
        public static object As(
            this IPublishedContent content,
            Type type,
            CultureInfo culture = null,
            object instance = null,
            IEnumerable<DittoProcessorContext> processorContexts = null,
            Action<DittoConversionHandlerContext> onConverting = null,
            Action<DittoConversionHandlerContext> onConverted = null,
            DittoChainContext chainContext = null)
        {
            // Ensure content
            if (content == null)
            {
                return null;
            }

            // Ensure instance is of target type
            if (instance != null && type.IsInstanceOfType(instance) == false)
            {
                throw new ArgumentException($"The instance parameter does not implement Type '{type.Name}'", nameof(instance));
            }

            // Get the context accessor (for access to ApplicationContext, UmbracoContext, et al)
            var contextAccessor = Ditto.GetContextAccessor();

            // Check if the culture has been set, otherwise use from Umbraco, or fallback to a default
            if (culture == null)
            {
                culture = contextAccessor.UmbracoContext?.PublishedContentRequest != null
                    ? contextAccessor.UmbracoContext.PublishedContentRequest.Culture
                    : CultureInfo.CurrentCulture;
            }

            // Ensure a chain context
            if (chainContext == null)
            {
                chainContext = new DittoChainContext();
            }

            // Populate prcessor contexts collection with any passed in contexts
            chainContext.ProcessorContexts.AddRange(processorContexts);

            // Convert
            using (DittoDisposableTimer.DebugDuration(typeof(Ditto), $"As<{type.Name}>({content.DocumentTypeAlias} {content.Id})"))
            {
                if (Ditto.TryGetTypeAttribute(type, out DittoCacheAttribute cacheAttr))
                {
                    var ctx = new DittoCacheContext(cacheAttr, content, type, culture);
                    return cacheAttr.GetCacheItem(ctx, () => ConvertContent(content, type, contextAccessor, culture, instance, onConverting, onConverted, chainContext));
                }
                else
                {
                    return ConvertContent(content, type, contextAccessor, culture, instance, onConverting, onConverted, chainContext);
                }
            }
        }

        /// <summary>Returns an object representing the given <see cref="Type"/>.</summary>
        /// <param name="content">The <see cref="IPublishedContent"/> to convert.</param>
        /// <param name="type">The <see cref="Type"/> of items to return.</param>
        /// <param name="contextAccessor">The context accessor.</param>
        /// <param name="culture">The <see cref="CultureInfo"/></param>
        /// <param name="instance">An existing instance of T to populate</param>
        /// <param name="onConverting">The <see cref="Action{ConversionHandlerContext}"/> to fire when converting.</param>
        /// <param name="onConverted">The <see cref="Action{ConversionHandlerContext}"/> to fire when converted.</param>
        /// <param name="chainContext">The <see cref="DittoChainContext"/> for the current processor chain.</param>
        /// <returns>The converted <see cref="Object"/> as the given type.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the given type has invalid constructors.</exception>
        private static object ConvertContent(
            IPublishedContent content,
            Type type,
            IDittoContextAccessor contextAccessor,
            CultureInfo culture,
            object instance,
            Action<DittoConversionHandlerContext> onConverting,
            Action<DittoConversionHandlerContext> onConverted,
            DittoChainContext chainContext)
        {
            // Collect all the properties of the given type and loop through writable ones.
            PropertyCache.TryGetValue(type, out PropertyInfo[] properties);

            if (properties == null)
            {
                properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.CanWrite && x.GetSetMethod() != null).ToArray();

                PropertyCache.TryAdd(type, properties);
            }

            // Check the validity of the mpped type constructor as early as possible.
            ParameterInfo[] constructorParams = type.GetConstructorParameters();
            bool validConstructor = false;
            bool hasParameter = false;
            bool isType = false;
            bool hasLazy = false;

            if (constructorParams != null)
            {
                // Is it PublishedContentmModel or similar?
                if (constructorParams.Length == 1 && constructorParams[0].ParameterType == typeof(IPublishedContent))
                {
                    hasParameter = true;
                }

                if (constructorParams.Length == 0 || hasParameter)
                {
                    validConstructor = true;
                }
            }

            // No valid constructor, but see if the value can be cast to the type
            if (type.IsInstanceOfType(content))
            {
                isType = true;
                validConstructor = true;
            }

            if (validConstructor == false)
            {
                throw new InvalidOperationException(
                    $"Cannot convert IPublishedContent to {type} as it has no valid constructor. " +
                    "A valid constructor is either an empty one, or one accepting a single IPublishedContent parameter.");
            }

            PropertyInfo[] lazyProperties = null;

            // If not already an instance, create an instance of the object
            if (instance == null)
            {
                // We can only proxy new instances.
                lazyProperties = properties.Where(x => x.ShouldAttemptLazyLoad()).ToArray();

                if (lazyProperties.Any())
                {
                    hasLazy = true;

                    var factory = new ProxyFactory();
                    instance = hasParameter
                        ? factory.CreateProxy(type, lazyProperties.Select(x => x.Name), content)
                        : factory.CreateProxy(type, lazyProperties.Select(x => x.Name));

                }
                else if (isType)
                {
                    instance = content;
                }
                else
                {
                    // 1: This extension method is about 7x faster than the native implementation.
                    // 2: Internally this uses Activator.CreateInstance which is heavily optimized.
                    instance = hasParameter
                        ? type.GetInstance(content) // 1
                        : type.GetInstance(); // 2
                }
            }

            // We have the instance object but haven't yet populated properties
            // so fire the on converting event handlers
            OnConverting(content, type, culture, instance, onConverting);

            if (hasLazy)
            {
                // A dictionary to store lazily invoked values.
                var lazyMappings = new Dictionary<string, Lazy<object>>();
                foreach (var propertyInfo in lazyProperties)
                {
                    // Configure lazy properties
                    using (DittoDisposableTimer.DebugDuration(typeof(Ditto), $"Lazy Property ({content.Id} {propertyInfo.Name})"))
                    {
                        // Ensure it's a virtual property (Only relevant to property level lazy loads)
                        if (propertyInfo.IsVirtualAndOverridable() == false)
                        {
                            throw new InvalidOperationException($"Lazy property '{propertyInfo.Name}' of type '{type.AssemblyQualifiedName}' must be declared virtual in order to be lazy loadable.");
                        }

                        lazyMappings.Add(propertyInfo.Name, new Lazy<object>(() => GetProcessedValue(content, culture, type, propertyInfo, instance, contextAccessor, chainContext)));
                    }
                }

                ((IProxy)instance).Interceptor = new LazyInterceptor(lazyMappings);
            }

            // Process any non lazy properties
            foreach (var propertyInfo in properties.Where(x => x.ShouldAttemptLazyLoad() == false))
            {
                // Check for the ignore attribute.
                if (propertyInfo.HasCustomAttribute<DittoIgnoreAttribute>())
                {
                    continue;
                }

                // Set the value normally.
                var value = GetProcessedValue(content, culture, type, propertyInfo, instance, contextAccessor, chainContext);

                // This over 4x faster as propertyInfo.SetValue(instance, value, null);
                FastPropertyAccessor.SetValue(propertyInfo, instance, value);
            }

            // We have now finished populating the instance object so go ahead
            // and fire the on converted event handlers
            OnConverted(content, type, culture, instance, onConverted);

            return instance;
        }

        /// <summary>Returns the processed value for the given type and property.</summary>
        /// <param name="content">The <see cref="IPublishedContent" /> to convert.</param>
        /// <param name="culture">The <see cref="CultureInfo" /></param>
        /// <param name="targetType">The target type.</param>
        /// <param name="propertyInfo">The <see cref="PropertyInfo" /> property info associated with the type.</param>
        /// <param name="instance">The instance to assign the value to.</param>
        /// <param name="contextAccessor">The context accessor.</param>
        /// <param name="chainContext">The <see cref="DittoChainContext"/> for the current processor chain.</param>
        /// <returns>The <see cref="object" /> representing the Umbraco value.</returns>
        private static object GetProcessedValue(
            IPublishedContent content,
            CultureInfo culture,
            Type targetType,
            PropertyInfo propertyInfo,
            object instance,
            IDittoContextAccessor contextAccessor,
            DittoChainContext chainContext)
        {
            using (DittoDisposableTimer.DebugDuration(typeof(Ditto), $"Processing '{propertyInfo.Name}' ({content.Id})"))
            {
                // Create a base processor context for this current chain level
                var baseProcessorContext = new DittoProcessorContext
                {
                    Content = content,
                    TargetType = targetType,
                    PropertyInfo = propertyInfo,
                    Culture = culture
                };

                // Check for cache attribute
                var cacheAttr = propertyInfo.GetCustomAttribute<DittoCacheAttribute>(true);
                if (cacheAttr != null)
                {
                    var ctx = new DittoCacheContext(cacheAttr, content, targetType, propertyInfo, culture);
                    return cacheAttr.GetCacheItem(ctx, () => DoGetProcessedValue(content, propertyInfo, contextAccessor, baseProcessorContext, chainContext));
                }
                else
                {
                    return DoGetProcessedValue(content, propertyInfo, contextAccessor, baseProcessorContext, chainContext);
                }
            }
        }

        /// <summary>Returns the processed value for the given type and property.</summary>
        /// <param name="content">The content.</param>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="contextAccessor">The context accessor.</param>
        /// <param name="baseProcessorContext">The base processor context.</param>
        /// <param name="chainContext">The <see cref="DittoChainContext"/> for the current processor chain.</param>
        /// <returns>Returns the processed value.</returns>
        private static object DoGetProcessedValue(
            IPublishedContent content,
            PropertyInfo propertyInfo,
            IDittoContextAccessor contextAccessor,
            DittoProcessorContext baseProcessorContext,
            DittoChainContext chainContext)
        {
            // Check the property for any explicit processor attributes
            var processorAttrs = propertyInfo.GetCustomAttributes<DittoProcessorAttribute>(true)
                .OrderBy(x => x.Order)
                .ToList();

            if (processorAttrs.Any() == false)
            {
                // Adds the default processor for this conversion
                processorAttrs.Add(DittoProcessorRegistry.Instance.GetDefaultProcessorFor(baseProcessorContext.TargetType));
            }

            var propertyType = propertyInfo.PropertyType;

            // Check for type registered processors
            processorAttrs.AddRange(propertyType
                .GetCustomAttributes<DittoProcessorAttribute>(true)
                .OrderBy(x => x.Order));

            // Check any type arguments in generic enumerable types.
            // This should return false against typeof(string) etc also.
            var typeInfo = propertyType.GetTypeInfo();
            bool isEnumerable = false;
            Type typeArg = null;
            if (propertyType.IsCastableEnumerableType())
            {
                typeArg = typeInfo.GenericTypeArguments.First();
                processorAttrs.AddRange(typeInfo
                    .GenericTypeArguments
                    .First()
                    .GetCustomAttributes<DittoProcessorAttribute>(true)
                    .OrderBy(x => x.Order)
                    .ToList());

                isEnumerable = true;
            }

            // Check for globally registered processors
            processorAttrs.AddRange(DittoProcessorRegistry.Instance.GetRegisteredProcessorAttributesFor(propertyInfo.PropertyType));

            // Add any core processors onto the end
            processorAttrs.AddRange(DittoProcessorRegistry.Instance.GetPostProcessorAttributes());

            // Create holder for value as it's processed
            object currentValue = content;

            // Process attributes
            foreach (var processorAttr in processorAttrs)
            {
                using (DittoDisposableTimer.DebugDuration(typeof(Ditto), $"Processor '{processorAttr.GetType().Name}' ({content.Id})"))
                {
                    // Get the right context type
                    var ctx = chainContext.ProcessorContexts.GetOrCreate(baseProcessorContext, processorAttr.ContextType);

                    // Populate UmbracoContext & ApplicationContext
                    processorAttr.UmbracoContext = contextAccessor.UmbracoContext;
                    processorAttr.ApplicationContext = contextAccessor.ApplicationContext;

                    // Process value
                    currentValue = processorAttr.ProcessValue(currentValue, ctx, chainContext);
                }
            }

            // The following has to happen after all the processors.
            if (isEnumerable && currentValue != null && currentValue.Equals(Enumerable.Empty<object>()))
            {
                if (propertyType.IsInterface)
                {
                    // You cannot set an enumerable of type from an empty object array.
                    currentValue = EnumerableInvocations.Cast(typeArg, (IEnumerable)currentValue);
                }
                else
                {
                    // This should allow the casting back of IEnumerable<T> to an empty List<T> Collection<T> etc.
                    // I cant think of any that don't have an empty constructor
                    currentValue = propertyType.GetInstance();
                }
            }

            return (currentValue == null && propertyType.IsValueType)
                ? propertyInfo.PropertyType.GetInstance() // Set to default instance of value type
                : currentValue;
        }

        /// <summary>Fires off the various on converting events.</summary>
        /// <param name="content">The <see cref="IPublishedContent"/> to convert.</param>
        /// <param name="type">The instance type.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="instance">The instance to assign the value to.</param>
        /// <param name="callback">The <see cref="Action{ConversionHandlerContext}"/> to fire when converting.</param>
        private static void OnConverting(
            IPublishedContent content,
            Type type,
            CultureInfo culture,
            object instance,
            Action<DittoConversionHandlerContext> callback)
        {
            OnConvert<DittoOnConvertingAttribute>(
                DittoConversionHandlerType.OnConverting,
                content,
                type,
                culture,
                instance,
                callback);
        }

        /// <summary>Fires off the various on converted events.</summary>
        /// <param name="content">The <see cref="IPublishedContent"/> to convert.</param>
        /// <param name="type">The instance type.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="instance">The instance to assign the value to.</param>
        /// <param name="callback">The <see cref="Action{ConversionHandlerContext}"/> to fire when converted.</param>
        private static void OnConverted(
            IPublishedContent content,
            Type type,
            CultureInfo culture,
            object instance,
            Action<DittoConversionHandlerContext> callback)
        {
            OnConvert<DittoOnConvertedAttribute>(
                DittoConversionHandlerType.OnConverted,
                content,
                type,
                culture,
                instance,
                callback);
        }

        /// <summary>Convenience method for calling converting/converter handlers.</summary>
        /// <typeparam name="TAttributeType">The type of the attribute type.</typeparam>
        /// <param name="conversionType">Type of the conversion.</param>
        /// <param name="content">The content.</param>
        /// <param name="type">The type.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="callback">The callback.</param>
        private static void OnConvert<TAttributeType>(
            DittoConversionHandlerType conversionType,
            IPublishedContent content,
            Type type,
            CultureInfo culture,
            object instance,
            Action<DittoConversionHandlerContext> callback)
            where TAttributeType : Attribute
        {
            // Trigger conversion handlers
            var conversionCtx = new DittoConversionHandlerContext
            {
                Content = content,
                Culture = culture,
                ModelType = type,
                Model = instance
            };

            // Check for class level DittoConversionHandlerAttribute
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

            // Check for method level DittoOnConvert[ing|ed]Attribute
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