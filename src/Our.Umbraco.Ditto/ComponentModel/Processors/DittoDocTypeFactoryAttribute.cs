namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Conveniense processor wrapper for resolving a type based on a content items Doc Type
    /// </summary>
    public class DittoDocTypeFactoryAttribute : DittoFactoryAttribute
    {
        /// <summary>
        /// Instatiates an instance of DittoDocTypeFactoryAttribute
        /// </summary>
        public DittoDocTypeFactoryAttribute()
            : base(typeof(DittoDocTypeFactoryTypeNameResolver))
        { }
    }
}