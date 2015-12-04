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
        /// <typeparam name="TProcessorType">The type of the processor type.</typeparam>
        public static void RegisterProcessor<TObjectType, TProcessorType>()
            where TProcessorType : DittoProcessor
        {
            DittoProcessorRegistry.Instance.RegisterProcessor<TObjectType, TProcessorType>();
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
    }
}
