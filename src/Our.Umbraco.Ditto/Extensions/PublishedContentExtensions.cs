using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Encapsulates extension methods for <see cref="IPublishedContent"/>.
    /// </summary>
    public static class PublishedContentExtensions
    {
        /// <summary>
        /// Get the context accessor (for access to ApplicationContext, UmbracoContext, et al)
        /// </summary>
        private static readonly IDittoContextAccessor ContextAccessor = Ditto.GetContextAccessor();

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
#if DEBUG
            using (ContextAccessor?.ApplicationContext?.ProfilingLogger?.DebugDuration(typeof(Ditto), $"IEnumerable As<{type.Name}>", "Complete"))
            {
#endif
                var typedItems = items
                    .Select(x => x.As(type, culture, null, processorContexts, onConverting, onConverted, chainContext))
                    .ToArray(); // Avoid deferred execution.

                // We need to cast back here as nothing is strong typed anymore.
                return (IEnumerable<object>)EnumerableInvocations.Cast(type, typedItems);
#if DEBUG
            }
#endif
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

            // Check if the culture has been set, otherwise use from Umbraco, or fallback to a default
            if (culture == null)
            {
                culture = ContextAccessor?.UmbracoContext?.PublishedContentRequest?.Culture ?? CultureInfo.CurrentCulture;
            }

            // Ensure a chain context
            if (chainContext == null)
            {
                chainContext = new DittoChainContext();
            }

            // Populate prcessor contexts collection with any passed in contexts
            chainContext.ProcessorContexts.AddRange(processorContexts);

            // Convert
#if DEBUG
            using (ContextAccessor?.ApplicationContext?.ProfilingLogger?.DebugDuration(typeof(Ditto), $"As<{type.Name}>({content.DocumentTypeAlias} {content.Id})", "Complete"))
            {
#endif
                var config = DittoTypeInfoCache.GetOrAdd(type);

                if (config.IsCacheable)
                {
                    var ctx = new DittoCacheContext(config.CacheInfo, content, type, culture);
                    return config.CacheInfo.GetCacheItem(ctx, () => ConvertContent(content, config, culture, instance, onConverting, onConverted, chainContext));
                }
                else
                {
                    return ConvertContent(content, config, culture, instance, onConverting, onConverted, chainContext);
                }
#if DEBUG
            }
#endif
        }

        /// <summary>Returns an object representing the given <see cref="Type"/>.</summary>
        /// <param name="content">The <see cref="IPublishedContent"/> to convert.</param> 
        /// <param name="config">The Ditto configuration for the type.</param>
        /// <param name="culture">The <see cref="CultureInfo"/></param>
        /// <param name="instance">An existing instance of T to populate</param>
        /// <param name="onConverting">The <see cref="Action{ConversionHandlerContext}"/> to fire when converting.</param>
        /// <param name="onConverted">The <see cref="Action{ConversionHandlerContext}"/> to fire when converted.</param>
        /// <param name="chainContext">The <see cref="DittoChainContext"/> for the current processor chain.</param>
        /// <returns>The converted <see cref="Object"/> as the given type.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the given type has invalid constructors.</exception>
        private static object ConvertContent(
            IPublishedContent content,
            DittoTypeInfo config,
            CultureInfo culture,
            object instance,
            Action<DittoConversionHandlerContext> onConverting,
            Action<DittoConversionHandlerContext> onConverted,
            DittoChainContext chainContext)
        {
            // If not already an instance, create an instance of the object.
            if (instance == null)
            {
                // Check the validity of the mapped type constructor.
                if (config.ConstructorIsValid == false)
                {
                    throw new InvalidOperationException(
                        $"Cannot convert IPublishedContent to {config.TargetType} as it has no valid constructor. " +
                        "A valid constructor is either empty, or one accepting a single IPublishedContent parameter.");
                }

                // We can only proxy new instances.
                if (config.ConstructorRequiresProxyType)
                {
                    var factory = new ProxyFactory();
                    instance = config.ConstructorHasPublishedContentParameter
                        ? factory.CreateProxy(config.TargetType, config.LazyPropertyNames, content)
                        : factory.CreateProxy(config.TargetType, config.LazyPropertyNames);

                }
                else if (config.IsOfTypePublishedContent)
                {
                    instance = content;
                }
                else
                {
                    // 1: This extension method is about 7x faster than the native implementation.
                    // 2: Internally this uses Activator.CreateInstance which is heavily optimized.
                    instance = config.ConstructorHasPublishedContentParameter
                        // TODO: Review this, as we could get the Constructor metadata from the "type-cache"?
                        ? config.TargetType.GetInstance(content) // 1
                        : config.TargetType.GetInstance(); // 2
                }
            }

            // We have the instance object but haven't yet populated properties
            // so fire the on converting event handlers
            OnConverting(content, config, culture, instance, onConverting);

            if (config.HasLazyProperties)
            {
                // A dictionary to store lazily invoked values.
                var lazyMappings = new Dictionary<string, Lazy<object>>();
                foreach (var lazyProperty in config.LazyProperties)
                {
                    // Configure lazy properties
                    lazyMappings.Add(lazyProperty.PropertyInfo.Name, new Lazy<object>(() => GetProcessedValue(content, culture, config, lazyProperty, instance, chainContext)));
                }

                ((IProxy)instance).Interceptor = new LazyInterceptor(lazyMappings);
            }

            // Process any eager properties
            if (config.HasEagerProperties)
            {
                foreach (var eagerProperty in config.EagerProperties)
                {
                    // Set the value normally.
                    var value = GetProcessedValue(content, culture, config, eagerProperty, instance, chainContext);

                    // This over 4x faster as propertyInfo.SetValue(instance, value, null);
                    FastPropertyAccessor.SetValue(eagerProperty.PropertyInfo, instance, value);
                }
            }

            // We have now finished populating the instance object so go ahead
            // and fire the on converted event handlers
            OnConverted(content, config, culture, instance, onConverted);

            return instance;
        }

        /// <summary>Returns the processed value for the given type and property.</summary>
        /// <param name="content">The <see cref="IPublishedContent" /> to convert.</param>
        /// <param name="culture">The <see cref="CultureInfo" /></param> 
        /// <param name="config">The Ditto configuration for the type.</param>
        /// <param name="mappableProperty">Information about the mappable property.</param>
        /// <param name="instance">The instance to assign the value to.</param>
        /// <param name="chainContext">The <see cref="DittoChainContext"/> for the current processor chain.</param>
        /// <returns>The <see cref="object" /> representing the Umbraco value.</returns>
        private static object GetProcessedValue(
            IPublishedContent content,
            CultureInfo culture,
            DittoTypeInfo config,
            DittoTypeInfo.DittoTypePropertyInfo mappableProperty,
            object instance,
            DittoChainContext chainContext)
        {
#if DEBUG
            using (ContextAccessor?.ApplicationContext?.ProfilingLogger?.DebugDuration(typeof(Ditto), $"Processing '{mappableProperty.PropertyInfo.Name}' ({content.Id})", "Complete"))
            {
#endif
                // Create a base processor context for this current chain level
                var baseProcessorContext = new DittoProcessorContext
                {
                    Content = content,
                    TargetType = config.TargetType,
                    PropertyInfo = mappableProperty.PropertyInfo,
                    Culture = culture
                };

                // Check for cache attribute
                if (mappableProperty.IsCacheable)
                {
                    var ctx = new DittoCacheContext(mappableProperty.CacheInfo, content, config.TargetType, mappableProperty.PropertyInfo, culture);
                    return mappableProperty.CacheInfo.GetCacheItem(ctx, () => DoGetProcessedValue(content, mappableProperty, baseProcessorContext, chainContext));
                }
                else
                {
                    return DoGetProcessedValue(content, mappableProperty, baseProcessorContext, chainContext);
                }
#if DEBUG
            }
#endif
        }

        /// <summary>Returns the processed value for the given type and property.</summary>
        /// <param name="content">The content.</param> 
        /// <param name="mappableProperty">Information about the mappable property.</param>
        /// <param name="baseProcessorContext">The base processor context.</param>
        /// <param name="chainContext">The <see cref="DittoChainContext"/> for the current processor chain.</param>
        /// <returns>Returns the processed value.</returns>
        private static object DoGetProcessedValue(
            IPublishedContent content,
            DittoTypeInfo.DittoTypePropertyInfo mappableProperty,
            DittoProcessorContext baseProcessorContext,
            DittoChainContext chainContext)
        {
            // Create holder for value as it's processed
            object currentValue = content;

            // Process attributes
            foreach (var processorAttr in mappableProperty.Processors)
            {
#if DEBUG
                using (ContextAccessor?.ApplicationContext?.ProfilingLogger?.DebugDuration(typeof(Ditto), $"Processor '{processorAttr}' ({content.Id})", "Complete"))
                {
#endif
                    // Get the right context type
                    var ctx = chainContext.ProcessorContexts.GetOrCreate(baseProcessorContext, processorAttr.ContextType);

                    // Populate UmbracoContext & ApplicationContext
                    processorAttr.UmbracoContext = ContextAccessor.UmbracoContext;
                    processorAttr.ApplicationContext = ContextAccessor.ApplicationContext;

                    // Process value
                    currentValue = processorAttr.ProcessValue(currentValue, ctx, chainContext);
#if DEBUG
                }
#endif
            }

            // The following has to happen after all the processors.
            if (mappableProperty.IsEnumerable && currentValue != null && currentValue.Equals(Enumerable.Empty<object>()))
            {
                if (mappableProperty.PropertyInfo.PropertyType.IsInterface)
                {
                    // You cannot set an enumerable of type from an empty object array.
                    currentValue = EnumerableInvocations.Cast(mappableProperty.EnumerableType, (IEnumerable)currentValue);
                }
                else
                {
                    // This should allow the casting back of IEnumerable<T> to an empty List<T> Collection<T> etc.
                    // I cant think of any that don't have an empty constructor
                    currentValue = mappableProperty.PropertyInfo.PropertyType.GetInstance();
                }
            }

            return currentValue == null && mappableProperty.PropertyInfo.PropertyType.IsValueType
                ? mappableProperty.PropertyInfo.PropertyType.GetInstance() // Set to default instance of value type
                : currentValue;
        }

        /// <summary>Fires off the various on converting events.</summary>
        /// <param name="content">The <see cref="IPublishedContent"/> to convert.</param> 
        /// <param name="config">The Ditto configuration for the type.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="instance">The instance to assign the value to.</param>
        /// <param name="callback">The <see cref="Action{ConversionHandlerContext}"/> to fire when converting.</param>
        private static void OnConverting(
            IPublishedContent content,
            DittoTypeInfo config,
            CultureInfo culture,
            object instance,
            Action<DittoConversionHandlerContext> callback)
        {
            OnConvert<DittoOnConvertingAttribute>(
                DittoConversionHandlerType.OnConverting,
                content,
                config,
                culture,
                instance,
                callback);
        }

        /// <summary>Fires off the various on converted events.</summary>
        /// <param name="content">The <see cref="IPublishedContent"/> to convert.</param> 
        /// <param name="config">The Ditto configuration for the type.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="instance">The instance to assign the value to.</param>
        /// <param name="callback">The <see cref="Action{ConversionHandlerContext}"/> to fire when converted.</param>
        private static void OnConverted(
            IPublishedContent content,
            DittoTypeInfo config,
            CultureInfo culture,
            object instance,
            Action<DittoConversionHandlerContext> callback)
        {
            OnConvert<DittoOnConvertedAttribute>(
                DittoConversionHandlerType.OnConverted,
                content,
                config,
                culture,
                instance,
                callback);
        }

        /// <summary>Convenience method for calling converting/converter handlers.</summary>
        /// <typeparam name="TAttributeType">The type of the attribute type.</typeparam>
        /// <param name="conversionType">Type of the conversion.</param>
        /// <param name="content">The content.</param> 
        /// <param name="config">The Ditto configuration for the type.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="callback">The callback.</param>
        private static void OnConvert<TAttributeType>(
            DittoConversionHandlerType conversionType,
            IPublishedContent content,
            DittoTypeInfo config,
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
                ModelType = config.TargetType,
                Model = instance
            };

            // Run the registered handlers
            foreach (var handler in config.ConversionHandlers)
            {
                handler.Run(conversionCtx, conversionType);
            }

            var methods = conversionType == DittoConversionHandlerType.OnConverting
                ? config.ConvertingMethods
                : config.ConvertedMethods;

            if (methods.Any())
            {
                foreach (var method in methods)
                {
                    // TODO: Review this, `Invoke` could be CPU heavy?!
                    method.Invoke(instance, new object[] { conversionCtx });
                    // Could we use a RuntimeMethodHandle?
                    // https://web.archive.org/web/20150118044646/http://msdn.microsoft.com:80/en-us/magazine/cc163759.aspx#S8
                }
            }

            // Check for a callback function
            callback?.Invoke(conversionCtx);
        }
    }
}