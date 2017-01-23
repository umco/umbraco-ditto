namespace Our.Umbraco.Ditto
{
    /// <summary>
    ///     Represents the context for a DittoMultiProcessorAttribute
    /// </summary>
    public class DittoMultiProcessorContext : DittoProcessorContext
    {
        /// <summary>
        /// Creates a new DittoMultiProcessorContext using an existing context
        /// </summary>
        /// <param name="context"></param>
        public DittoMultiProcessorContext(DittoProcessorContext context) : base(context)
        {
            ContextCache = new DittoProcessorContextCache(Content, TargetType, PropertyDescriptor, Culture);
        }

        /// <summary>
        /// Creates a new DittoMultiProcessorContext
        /// </summary>
        public DittoMultiProcessorContext()
        {
        }

        /// <summary>
        /// Gets or sets the context cache.
        /// </summary>
        /// <value>
        /// The context cache.
        /// </value>
        public DittoProcessorContextCache ContextCache { get; set; }
    }
}