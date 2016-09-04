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
        /// <returns>Returns a string representing the cache key.</returns>
        public abstract string BuildCacheKey(DittoCacheContext context);
    }
}