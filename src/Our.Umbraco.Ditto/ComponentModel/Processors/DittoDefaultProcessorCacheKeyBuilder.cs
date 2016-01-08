namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Representes the default ditto processor cache key builder
    /// </summary>
    public class DittoDefaultProcessorCacheKeyBuilder : DittoProcessorCacheKeyBuilder
    {
        /// <summary>
        /// Builds the cache key.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns></returns>
        public override string BuildCacheKey(DittoProcessorAttribute attribute)
        {
            var cacheKey = "Ditto_DittoProcessorAttribute_ProcessValue";

            if ((attribute.CacheBy & DittoProcessorCacheBy.ContentId) == DittoProcessorCacheBy.ContentId)
            {
                cacheKey += "_" + attribute.Context.Content.Id;
            }
            if ((attribute.CacheBy & DittoProcessorCacheBy.PropertyName) == DittoProcessorCacheBy.PropertyName)
            {
                cacheKey += "_" + attribute.Context.PropertyDescriptor.Name;
            }
            if ((attribute.CacheBy & DittoProcessorCacheBy.TargetType) == DittoProcessorCacheBy.TargetType)
            {
                cacheKey += "_" + attribute.Context.TargetType.AssemblyQualifiedName;
            }
            if ((attribute.CacheBy & DittoProcessorCacheBy.Culture) == DittoProcessorCacheBy.Culture)
            {
                cacheKey += "_" + attribute.Context.Culture.LCID;
            }

            return cacheKey;
        }
    }
}