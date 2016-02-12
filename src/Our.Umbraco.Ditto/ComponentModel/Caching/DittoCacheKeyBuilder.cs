namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Abstract class for the Ditto cache key builder
    /// </summary>
    public abstract class DittoCacheKeyBuilder
    {
        /// <summary>
        /// Builds the cache key.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public abstract string BuildCacheKey(DittoCacheContext context);
    }
}