namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Conveniense processor for resolving a type based on the contents Doc Type
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