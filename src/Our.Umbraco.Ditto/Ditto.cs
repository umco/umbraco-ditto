using System.ComponentModel;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The public facade for non extension method Ditto actions
    /// </summary>
    public class Ditto
    {
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
        /// Registers a global value resolver.
        /// </summary>
        /// <typeparam name="TObjectType">The type of the object being converted.</typeparam>
        /// <typeparam name="TResolverType">The type of the value resolver.</typeparam>
        public static void RegisterValueResolver<TObjectType, TResolverType>()
            where TResolverType : DittoValueResolver
        {
            DittoValueResolverRegistry.Instance.RegisterResolver<TObjectType, TResolverType>();
        }

        /// <summary>
        /// Registers a global value resolver attribute.
        /// </summary>
        /// <typeparam name="TObjectType">The type of the object being converted.</typeparam>
        /// <typeparam name="TResolverAttributeType">The type of the value resolver attribute.</typeparam>
        public static void RegisterValueResolverAttribute<TObjectType, TResolverAttributeType>()
            where TResolverAttributeType : DittoValueResolverAttribute, new()
        {
            DittoValueResolverRegistry.Instance.RegisterResolverAttribute<TObjectType, TResolverAttributeType>();
        }

        /// <summary>
        /// Registers a global value resolver attribute.
        /// </summary>
        /// <typeparam name="TObjectType">The type of the object being converted.</typeparam>
        /// <typeparam name="TResolverAttributeType">The type of the value resolver attribute.</typeparam>
        /// <param name="instance">An instance of the value resolver attribute to use.</param>
        public static void RegisterValueResolverAttribute<TObjectType, TResolverAttributeType>(TResolverAttributeType instance)
            where TResolverAttributeType : DittoValueResolverAttribute
        {
            DittoValueResolverRegistry.Instance.RegisterResolverAttribute<TObjectType, TResolverAttributeType>(instance);
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
