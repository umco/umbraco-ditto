using System;
using System.Web.Caching;
using Umbraco.Core;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DittoCacheableAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the type of the cache key builder.
        /// </summary>
        /// <value>
        /// The type of the cache key builder.
        /// </value>
        public Type CacheKeyBuilderType { get; set; }

        /// <summary>
        /// Gets or sets the properties to cache by.
        /// </summary>
        /// <value>
        /// The cache by property flags.
        /// </value>
        public DittoCacheBy CacheBy { get; set; }

        /// <summary>
        /// Gets or sets the duration of the cache.
        /// </summary>
        /// <value>
        /// The duration of the cache.
        /// </value>
        public int CacheDuration { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DittoCacheableAttribute"/> class.
        /// </summary>
        protected DittoCacheableAttribute()
        {
            CacheBy = Ditto.DefaultCacheBy;
            CacheDuration = 0;
        }

        /// <summary>
        /// Gets the cache item.
        /// </summary>
        /// <typeparam name="TOuputType">The type of the ouput type.</typeparam>
        /// <param name="cacheContext">The cache context.</param>
        /// <param name="reresher">The reresher.</param>
        /// <returns></returns>
        /// <exception cref="System.ApplicationException">Expected a cache key builder of type  + typeof(DittoProcessorCacheKeyBuilder) +  but got  + CacheKeyBuilderType</exception>
        internal TOuputType GetCacheItem<TOuputType>(DittoCacheContext cacheContext, Func<TOuputType> reresher) 
        {
            // If no cache duration set, just run the refresher
            if (CacheDuration == 0 || Ditto.IsDebuggingEnabled) return reresher();

            // Get the cache key builder type
            var cacheKeyBuilderType = CacheKeyBuilderType ?? typeof(DittoDefaultCacheKeyBuilder);

            // Check the cache key builder type
            if (!typeof(DittoCacheKeyBuilder).IsAssignableFrom(cacheKeyBuilderType))
            {
                throw new ApplicationException("Expected a cache key builder of type " + typeof(DittoCacheKeyBuilder) + " but got " + CacheKeyBuilderType);
            }

            // Store a reference to self in the context
            cacheContext.Attribute = this;

            // Construct the cache key builder
            var builder = (DittoCacheKeyBuilder)cacheKeyBuilderType.GetInstance();
            var cacheKey = builder.BuildCacheKey(cacheContext);

            // Get and cache the result
            return (TOuputType)ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem(cacheKey,
                () => reresher(),
                priority: CacheItemPriority.NotRemovable,
                timeout: new TimeSpan(0, 0, 0, CacheDuration));
        }
    }
}