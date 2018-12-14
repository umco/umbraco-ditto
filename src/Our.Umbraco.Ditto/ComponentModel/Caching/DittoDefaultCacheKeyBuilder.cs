using System.Collections.Generic;

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
            var cacheKey = new List<object> { "DittoCache" };

            if ((context.Attribute.CacheBy & DittoCacheBy.ContentId) == DittoCacheBy.ContentId)
            {
                cacheKey.Add(context.Content.Id);
            }

            if ((context.Attribute.CacheBy & DittoCacheBy.UpdateDate) == DittoCacheBy.UpdateDate)
            {
                cacheKey.Add(context.Content.UpdateDate);
            }

            if (context.PropertyInfo != null && (context.Attribute.CacheBy & DittoCacheBy.PropertyName) == DittoCacheBy.PropertyName)
            {
                cacheKey.Add(context.PropertyInfo.Name);
            }

            if ((context.Attribute.CacheBy & DittoCacheBy.TargetType) == DittoCacheBy.TargetType)
            {
                cacheKey.Add(context.TargetType.AssemblyQualifiedName);
            }

            if ((context.Attribute.CacheBy & DittoCacheBy.Culture) == DittoCacheBy.Culture)
            {
                cacheKey.Add(context.Culture.LCID);
            }

            return string.Join("_", cacheKey);
        }
    }
}