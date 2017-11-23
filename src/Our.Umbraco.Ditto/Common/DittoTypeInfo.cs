using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    internal sealed class DittoTypeInfo
    {
        public static DittoTypeInfo Create<T>()
        {
            return Create(typeof(T));
        }

        public static DittoTypeInfo Create(Type type)
        {
            var config = new DittoTypeInfo
            {
                TargetType = type
            };

            // constructor
            //
            // Check the validity of the mapped type constructor as early as possible.
            var constructorParams = type.GetConstructorParameters();
            if (constructorParams != null)
            {
                // Is it a PublishedContent or similar?
                if (constructorParams.Length == 1 && constructorParams[0].ParameterType == typeof(IPublishedContent))
                {
                    config.ConstructorHasPublishedContentParameter = true;
                }

                if (constructorParams.Length == 0 || config.ConstructorHasPublishedContentParameter)
                {
                    config.ConstructorIsValid = true;
                }
            }

            // No valid constructor, but see if the value can be cast to the type
            if (type.IsAssignableFrom(typeof(IPublishedContent)))
            {
                config.IsOfTypePublishedContent = true;
                config.ConstructorIsValid = true;
            }

            // attributes
            //
            config.CustomAttributes = type.GetCustomAttributes();

            // cacheable
            //
            var conversionHandlers = new List<DittoConversionHandler>();

            foreach (var attr in config.CustomAttributes)
            {
                if (attr is DittoCacheAttribute)
                {
                    config.IsCacheable = true;
                    config.CacheInfo = (DittoCacheAttribute)attr;
                }

                // Check for class level DittoConversionHandlerAttribute
                if (attr is DittoConversionHandlerAttribute)
                {
                    conversionHandlers.Add(((DittoConversionHandlerAttribute)attr).HandlerType.GetInstance<DittoConversionHandler>());
                }
            }

            // properties
            //
            var properties = new List<DittoTypePropertyInfo>();

            // Collect all the properties of the given type and loop through writable ones.
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.CanWrite == false)
                    continue;

                if (property.GetSetMethod() == null)
                    continue;

                var attributes = new List<Attribute>(property.GetCustomAttributes());

                if (attributes.Any(x => x is DittoIgnoreAttribute))
                    continue;

                var propertyType = property.PropertyType;

                var propertyConfig = new DittoTypePropertyInfo
                {
                    CustomAttributes = attributes,
                    PropertyInfo = property,
                };


                // Check the property for any explicit processor attributes
                var processors = attributes.Where(x => x is DittoProcessorAttribute).Cast<DittoProcessorAttribute>().ToList();
                if (processors.Count == 0)
                {
                    // Adds the default processor for this conversion
                    var defaultProcessor = DittoProcessorRegistry.Instance.GetDefaultProcessorFor(type);
                    // Forces the default processor to be the very first processor
                    defaultProcessor.Order = -1;
                    processors.Add(defaultProcessor);
                }

                // Check for registered processors on the property's type
                processors.AddRange(propertyType.GetCustomAttributes<DittoProcessorAttribute>(true));

                // Check any type arguments in generic enumerable types.
                // This should return false against typeof(string) etc also.
                if (propertyType.IsCastableEnumerableType())
                {
                    propertyConfig.IsEnumerable = true;
                    propertyConfig.EnumerableType = propertyType.GenericTypeArguments[0];

                    processors.AddRange(propertyConfig.EnumerableType.GetCustomAttributes<DittoProcessorAttribute>(true));
                }

                // Sort the order of the processors
                processors.Sort((x, y) => x.Order.CompareTo(y.Order));

                // Check for globally registered processors
                processors.AddRange(DittoProcessorRegistry.Instance.GetRegisteredProcessorAttributesFor(propertyType));

                // Add any core processors onto the end
                processors.AddRange(DittoProcessorRegistry.Instance.GetPostProcessorAttributes());

                propertyConfig.Processors = processors;


                var propertyCache = attributes.Where(x => x is DittoCacheAttribute).Cast<DittoCacheAttribute>().FirstOrDefault();
                if (propertyCache != null)
                {
                    propertyConfig.IsCacheable = true;
                    propertyConfig.CacheInfo = propertyCache;
                }

                properties.Add(propertyConfig);
            }

            if (properties.Count > 0)
            {
                config.HasProperties = true;
                config.Properties = properties;
            }



            // conversion handlers
            //

            // Check for globaly registered handlers
            foreach (var handlerType in DittoConversionHandlerRegistry.Instance.GetRegisteredHandlerTypesFor(type))
            {
                conversionHandlers.Add(handlerType.GetInstance<DittoConversionHandler>());
            }

            config.ConversionHandlers = conversionHandlers;

            var before = new List<MethodInfo>();
            var after = new List<MethodInfo>();

            // Check for method level DittoOnConvert[ing|ed]Attribute
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var ing = method.GetCustomAttribute<DittoOnConvertingAttribute>();
                var ed = method.GetCustomAttribute<DittoOnConvertedAttribute>();

                if (ing == null && ed == null)
                    continue;

                var p = method.GetParameters();
                if (p.Length == 1 && p[0].ParameterType == typeof(DittoConversionHandlerContext))
                {
                    if (ing != null)
                        before.Add(method);

                    if (ed != null)
                        after.Add(method);
                }
            }

            config.ConvertingMethods = before;
            config.ConvertedMethods = after;


            return config;
        }

        public Type TargetType { get; set; }

        public bool IsCacheable { get; set; }
        public DittoCacheAttribute CacheInfo { get; set; }

        public bool ConstructorIsValid { get; set; }
        public bool ConstructorHasPublishedContentParameter { get; set; }

        public bool IsOfTypePublishedContent { get; set; }

        public IEnumerable<Attribute> CustomAttributes { get; set; }

        public bool HasProperties { get; set; }
        public IEnumerable<DittoTypePropertyInfo> Properties { get; set; }

        public IEnumerable<DittoConversionHandler> ConversionHandlers { get; set; }
        public IEnumerable<MethodInfo> ConvertingMethods { get; set; }
        public IEnumerable<MethodInfo> ConvertedMethods { get; set; }

        internal sealed class DittoTypePropertyInfo
        {
            public bool IsCacheable { get; set; }
            public DittoCacheAttribute CacheInfo { get; set; }

            public IEnumerable<Attribute> CustomAttributes { get; set; }

            public IEnumerable<DittoProcessorAttribute> Processors { get; set; }
            public PropertyInfo PropertyInfo { get; set; }

            public bool IsEnumerable { get; set; }
            public Type EnumerableType { get; set; }
        }
    }
}