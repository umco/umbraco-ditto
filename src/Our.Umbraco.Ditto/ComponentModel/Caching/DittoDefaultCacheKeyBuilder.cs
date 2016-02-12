namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Represents the default ditto cache key builder
    /// </summary>
    public class DittoDefaultCacheKeyBuilder : DittoCacheKeyBuilder
    {
        /// <summary>
        /// Builds the cache key.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Returns the cache key.</returns>
        public override string BuildCacheKey(DittoCacheContext context)
        {
            var cacheKey = "DittoCache";

            if ((context.Attribute.CacheBy & DittoCacheBy.ContentId) == DittoCacheBy.ContentId)
            {
                cacheKey += "_" + context.Content.Id;
            }

            if ((context.Attribute.CacheBy & DittoCacheBy.ContentVersion) == DittoCacheBy.ContentVersion)
            {
                cacheKey += "_" + context.Content.Version;
            }

            if (context.PropertyDescriptor != null && (context.Attribute.CacheBy & DittoCacheBy.PropertyName) == DittoCacheBy.PropertyName)
            {
                cacheKey += "_" + context.PropertyDescriptor.Name;
            }

            if ((context.Attribute.CacheBy & DittoCacheBy.TargetType) == DittoCacheBy.TargetType)
            {
                cacheKey += "_" + context.TargetType.AssemblyQualifiedName;
            }

            if ((context.Attribute.CacheBy & DittoCacheBy.Culture) == DittoCacheBy.Culture)
            {
                cacheKey += "_" + context.Culture.LCID;
            }

            return cacheKey;
        }
    }
}