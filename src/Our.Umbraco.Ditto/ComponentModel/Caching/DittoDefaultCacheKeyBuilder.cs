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
            var cacheKey = new List<object>() { "DittoCache" };

            var cacheBy = context.Attribute.CacheBy;

            if (cacheBy.HasFlag(DittoCacheBy.ContentId))
            {
                cacheKey.Add(context.Content.Id);
            }

            if (cacheBy.HasFlag(DittoCacheBy.ContentVersion))
            {
                cacheKey.Add(context.Content.Version);
            }

            if (cacheBy.HasFlag(DittoCacheBy.PropertyName) && context.PropertyDescriptor != null)
            {
                cacheKey.Add(context.PropertyDescriptor.Name);
            }

            if (cacheBy.HasFlag(DittoCacheBy.TargetType))
            {
                cacheKey.Add(context.TargetType.AssemblyQualifiedName);
            }

            if (cacheBy.HasFlag(DittoCacheBy.Culture))
            {
                cacheKey.Add(context.Culture.LCID);
            }

            if (cacheBy.HasFlag(DittoCacheBy.AttributeType))
            {
                cacheKey.Add(context.Attribute.GetType().FullName);
            }

            var cacheKeyString = string.Join("_", cacheKey);

            return cacheBy.HasFlag(DittoCacheBy.Custom) ? context.Attribute.CustomiseCacheKey(cacheKeyString, context) : cacheKeyString;
        }
    }
}