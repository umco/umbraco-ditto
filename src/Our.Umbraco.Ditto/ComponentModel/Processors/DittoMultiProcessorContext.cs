namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Represents the context for a DitoMultiProcessorAttribute
    /// </summary>
    internal class DittoMultiProcessorContext : DittoProcessorContext
    {
        /// <summary>
        /// Gets or sets the context cache.
        /// </summary>
        /// <value>
        /// The context cache.
        /// </value>
        public DittoProcessorContextCache ContextCache { get; set; }
    }
}