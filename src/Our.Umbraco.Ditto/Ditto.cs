using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The public facade for non extension method Ditto actions
    /// </summary>
    public static class Ditto
    {
        /// <summary>
        /// The global context accessor type for processors.
        /// </summary>
        private static Type contextAccessorType = typeof(DefaultDittoContextAccessor);

        /// <summary>
        /// The Ditto processor attribute targets
        /// </summary>
        public const AttributeTargets ProcessorAttributeTargets = AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Enum;

        /// <summary>
        /// The default processor cache by flags
        /// </summary>
        public static DittoCacheBy DefaultCacheBy = DittoCacheBy.ContentId | DittoCacheBy.ContentVersion | DittoCacheBy.PropertyName | DittoCacheBy.Culture;

        /// <summary>
        /// The default source for umbraco property mappings
        /// </summary>
        public static PropertySource DefaultPropertySource = PropertySource.InstanceThenUmbracoProperties;

        /// <summary>
        /// The property bindings for mappable properties
        /// </summary>
        internal const BindingFlags MappablePropertiesBindingFlags = BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static;

        /// <summary>
        /// A list of mappable properties defined on the IPublishedContent interface
        /// </summary>
        internal static readonly List<PropertyInfo> IPublishedContentProperties = typeof(IPublishedContent)
            .GetProperties(MappablePropertiesBindingFlags)
            .Where(x => x.IsMappable())
            .ToList();

        /// <summary>
        /// Indicates whether the application is running in debug mode.
        /// </summary>
#if DEBUG
        internal static bool IsDebuggingEnabled = true;
#else
        internal static bool IsDebuggingEnabled = false;
#endif

        internal static bool GetDebugFlag()
        {
            // Check for app-setting first
            var appSetting = ConfigurationManager.AppSettings["Ditto:DebugEnabled"];
            if (string.IsNullOrWhiteSpace(appSetting) == false && bool.TryParse(appSetting, out bool dittoDebugEnabled))
            {
                return dittoDebugEnabled;
            }

            // Until `Umbraco.Core.Configuration.GlobalSettings.DebugMode` is available, we're using the legacy API.
            // ref: https://github.com/umbraco/Umbraco-CMS/blob/release-7.3.2/src/umbraco.businesslogic/GlobalSettings.cs#L129
            return umbraco.GlobalSettings.DebugMode;
        }

        /// <summary>
        /// Indicates whether the application is running in profile mode, e.g. "umbDebug" querystring.
        /// </summary>
        internal static bool IsProfilingEnabled()
        {
            var qs = HttpContext.Current?.Request?.QueryString?["umbDebug"];
            if (string.IsNullOrWhiteSpace(qs) == false && bool.TryParse(qs, out bool umbDebug))
            {
                return umbDebug;
            }

            return false;
        }

        /// <summary>
        /// Indicates whether the application is running within a unit-test scenario.
        /// </summary>
        internal static bool IsRunningInUnitTest = false;

        /// <summary>
        /// Registers a global conversion handler.
        /// </summary>
        /// <typeparam name="TObjectType">The type of the object being converted.</typeparam>
        /// <typeparam name="THandlerType">The type of the handler.</typeparam>
        public static void RegisterConversionHandler<TObjectType, THandlerType>()
            where THandlerType : DittoConversionHandler
        {
            DittoConversionHandlerRegistry.Instance.RegisterHandler<TObjectType, THandlerType>();
        }

        /// <summary>
        /// Registers the default processor type.
        /// </summary>
        /// <typeparam name="TProcessorAttributeType">The type of the processor attribute type.</typeparam>
        public static void RegisterDefaultProcessorType<TProcessorAttributeType>()
            where TProcessorAttributeType : DittoProcessorAttribute, new()
        {
            DittoProcessorRegistry.Instance.RegisterDefaultProcessorType<TProcessorAttributeType>();
        }

        /// <summary>
        /// Registers a global processor attribute.
        /// </summary>
        /// <typeparam name="TObjectType">The type of the object being converted.</typeparam>
        /// <typeparam name="TProcessorAttributeType">The type of the processor attribute type.</typeparam>
        public static void RegisterProcessorAttribute<TObjectType, TProcessorAttributeType>()
            where TProcessorAttributeType : DittoProcessorAttribute, new()
        {
            DittoProcessorRegistry.Instance.RegisterProcessorAttribute<TObjectType, TProcessorAttributeType>();
        }

        /// <summary>
        /// Registers a global processor attribute.
        /// </summary>
        /// <typeparam name="TObjectType">The type of the object being converted.</typeparam>
        /// <typeparam name="TProcessorAttributeType">The type of the processor attribute type.</typeparam>
        /// <param name="instance">An instance of the processor attribute to use.</param>
        public static void RegisterProcessorAttribute<TObjectType, TProcessorAttributeType>(TProcessorAttributeType instance)
            where TProcessorAttributeType : DittoProcessorAttribute
        {
            DittoProcessorRegistry.Instance.RegisterProcessorAttribute<TObjectType, TProcessorAttributeType>(instance);
        }

        /// <summary>
        /// Registers a global type converter.
        /// </summary>
        /// <typeparam name="TObjectType">The type of the object being converted.</typeparam>
        /// <typeparam name="TConverterType">The type of the converter.</typeparam>
        public static void RegisterTypeConverter<TObjectType, TConverterType>()
            where TConverterType : TypeConverter
        {
            TypeDescriptor.AddAttributes(typeof(TObjectType), new TypeConverterAttribute(typeof(TConverterType)));
        }

        /// <summary>
        /// Registers a global Ditto context accessor.
        /// </summary>
        /// <typeparam name="TDittoContextAccessorType">The type of the context accessor.</typeparam>
        public static void RegisterContextAccessor<TDittoContextAccessorType>()
            where TDittoContextAccessorType : IDittoContextAccessor, new()
        {
            contextAccessorType = typeof(TDittoContextAccessorType);
        }

        /// <summary>
        /// Gets the global Umbraco application context accessor.
        /// </summary>
        /// <returns>
        /// Returns the global Umbraco application context accessor.
        /// </returns>
        public static IDittoContextAccessor GetContextAccessor()
        {
            return contextAccessorType.GetInstance<IDittoContextAccessor>();
        }

        /// <summary>
        /// Registers a processor attribute to the end of the default set of post-processor attributes.
        /// </summary>
        /// <typeparam name="TProcessorAttributeType"></typeparam>
        /// <param name="position"></param>
        public static void RegisterPostProcessorType<TProcessorAttributeType>(int position = -1)
            where TProcessorAttributeType : DittoProcessorAttribute, new()
        {
            DittoProcessorRegistry.Instance.RegisterPostProcessorType<TProcessorAttributeType>(position);
        }

        /// <summary>
        /// Deregisters a processor attribute from the default set of post-processor attributes.
        /// </summary>
        /// <typeparam name="TProcessorAttributeType"></typeparam>
        public static void DeregisterPostProcessorType<TProcessorAttributeType>()
            where TProcessorAttributeType : DittoProcessorAttribute, new()
        {
            DittoProcessorRegistry.Instance.DeregisterPostProcessorType<TProcessorAttributeType>();
        }

        /// <summary>
        /// Tries to get the associated attribute for a given object type.
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="objectType">The object type.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="inherit">A boolean flag to search the type's inheritance chain to find the attribute.</param>
        /// <returns>Returns the associated attribute for the given object-type.</returns>
        public static bool TryGetTypeAttribute<TAttribute>(Type objectType, out TAttribute attribute, bool inherit = false) where TAttribute : Attribute
        {
            if (AttributedTypeResolver<TAttribute>.HasCurrent == false)
            {
                AttributedTypeResolver<TAttribute>.Current = AttributedTypeResolver<TAttribute>.Create(new[] { objectType }, inherit);
            }

            return AttributedTypeResolver<TAttribute>.Current.TryGetTypeAttribute(objectType, out attribute, inherit);
        }
    }
}