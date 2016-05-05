using System;
using System.ComponentModel;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using Umbraco.Core;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The public facade for non extension method Ditto actions
    /// </summary>
    public class Ditto
    {
        /// <summary>
        /// The Ditto processor attribute targets
        /// </summary>
        public const AttributeTargets ProcessorAttributeTargets = AttributeTargets.Property | AttributeTargets.Class;

        /// <summary>
        /// The default processor cache by flags
        /// </summary>
        public const DittoCacheBy DefaultCacheBy = DittoCacheBy.ContentId | DittoCacheBy.ContentVersion | DittoCacheBy.PropertyName | DittoCacheBy.Culture;

        /// <summary>
        /// Gets a value indicating whether application is running in debug mode.
        /// </summary>
        /// <value><c>true</c> if debug mode; otherwise, <c>false</c>.</value>
        internal static bool IsDebuggingEnabled
        {
            get
            {
                try
                {
                    // Check for app setting first
                    if (!ConfigurationManager.AppSettings["Ditto:DebugEnabled"].IsNullOrWhiteSpace())
                    {
                        return ConfigurationManager.AppSettings["Ditto:DebugEnabled"].InvariantEquals("true");
                    }

                    // Check the HTTP Context
                    if (HttpContext.Current != null)
                    {
                        return HttpContext.Current.IsDebuggingEnabled;
                    }

                    // Go and get it from config directly
                    var section = ConfigurationManager.GetSection("system.web/compilation") as CompilationSection;
                    return section != null && section.Debug;
                }
                catch
                {
                    return false;
                }
            }
        }

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
        /// Registers a global value resolver attribute.
        /// </summary>
        /// <typeparam name="TObjectType">The type of the object being converted.</typeparam>
        /// <typeparam name="TProcessorAttributeType">The type of the processor attribute type.</typeparam>
        public static void RegisterProcessorAttribute<TObjectType, TProcessorAttributeType>()
            where TProcessorAttributeType : DittoProcessorAttribute, new()
        {
            DittoProcessorRegistry.Instance.RegisterProcessorAttribute<TObjectType, TProcessorAttributeType>();
        }

        /// <summary>
        /// Registers a global value resolver attribute.
        /// </summary>
        /// <typeparam name="TObjectType">The type of the object being converted.</typeparam>
        /// <typeparam name="TProcessorAttributeType">The type of the processor attribute type.</typeparam>
        /// <param name="instance">An instance of the value resolver attribute to use.</param>
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
    }
}