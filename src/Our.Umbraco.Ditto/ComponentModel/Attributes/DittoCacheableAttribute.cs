using System;
using System.Web.Caching;
using Umbraco.Core;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Represents a Ditto attribute capable of caching
    /// </summary>
    public abstract class DittoCacheableAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DittoCacheableAttribute"/> class.
        /// </summary>
        protected DittoCacheableAttribute()
        {
            this.CacheBy = Ditto.DefaultCacheBy;
            this.CacheDuration = 0;
        }

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
        /// Gets the cache item.
        /// </summary>
        /// <typeparam name="TOuputType">The type of the output type.</typeparam>
        /// <param name="cacheContext">The cache context.</param>
        /// <param name="refresher">The refresher.</param>
        /// <returns>Returns the output type.</returns>
        /// <exception cref="System.ApplicationException">Expected a cache key builder of type  + typeof(DittoProcessorCacheKeyBuilder) +  but got  + CacheKeyBuilderType</exception>
        internal TOuputType GetCacheItem<TOuputType>(DittoCacheContext cacheContext, Func<TOuputType> refresher)
        {
            // If no cache duration set, (and in debug mode AND NOT a unit-test), then just run the refresher
            if (this.CacheDuration == 0 || (Ditto.IsDebuggingEnabled && Ditto.IsRunningInUnitTest == false))
            {
                return refresher();
            }

            // Get the cache key builder type
            var cacheKeyBuilderType = this.CacheKeyBuilderType ?? typeof(DittoDefaultCacheKeyBuilder);

            // Check the cache key builder type
            if (!typeof(DittoCacheKeyBuilder).IsAssignableFrom(cacheKeyBuilderType))
            {
                throw new ApplicationException($"Expected a cache key builder of type {typeof(DittoCacheKeyBuilder)} but got {this.CacheKeyBuilderType}");
            }

            // Construct the cache key builder
            var builder = (DittoCacheKeyBuilder)cacheKeyBuilderType.GetInstance();
            var cacheKey = builder.BuildCacheKey(cacheContext);

            // Get and cache the result
            return (TOuputType)ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem(
                cacheKey,
                () => refresher(),
                priority: CacheItemPriority.NotRemovable, // Same as Umbraco macros
                timeout: new TimeSpan(0, 0, 0, this.CacheDuration));
        }
    }
}