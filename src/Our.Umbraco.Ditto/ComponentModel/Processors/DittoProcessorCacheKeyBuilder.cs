namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DittoProcessorCacheKeyBuilder
    {
        /// <summary>
        /// Builds the cache key.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns></returns>
        public abstract string BuildCacheKey(DittoProcessorAttribute attribute);
    }
}