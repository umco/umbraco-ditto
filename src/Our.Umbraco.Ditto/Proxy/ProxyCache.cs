namespace Our.Umbraco.Ditto
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The proxy object cache.
    /// </summary>
    public class ProxyCache
    {
        /// <summary>
        /// The proxy type cache.
        /// </summary>
        private static readonly Dictionary<ProxyCacheEntry, Type> Cache = new Dictionary<ProxyCacheEntry, Type>();

        /// <summary>
        /// The lock.
        /// </summary>
        private static readonly object SyncLock = new object();

        /// <summary>
        /// Returns a value indicating whether the proxy cache contains a match for the 
        /// given type.
        /// </summary>
        /// <param name="baseType">
        /// The base type.
        /// </param>
        /// <param name="baseInterfaces">
        /// The base interfaces.
        /// </param>
        /// <returns>
        /// True if the cache contains the proxy; otherwise, false.
        /// </returns>
        public bool Contains(Type baseType, params Type[] baseInterfaces)
        {
            if (baseType == null)
            {
                return false;
            }

            lock (SyncLock)
            {
                ProxyCacheEntry entry = new ProxyCacheEntry(baseType, baseInterfaces);
                return Cache.ContainsKey(entry);
            }
        }

        /// <summary>
        /// Returns the correct proxy type for the given base type.
        /// </summary>
        /// <param name="baseType">
        /// The base type.
        /// </param>
        /// <param name="baseInterfaces">
        /// The base interfaces.
        /// </param>
        /// <returns>
        /// The proxy <see cref="Type"/>.
        /// </returns>
        public Type GetProxyType(Type baseType, params Type[] baseInterfaces)
        {
            lock (SyncLock)
            {
                ProxyCacheEntry entry = new ProxyCacheEntry(baseType, baseInterfaces);
                return Cache[entry];
            }
        }

        /// <summary>
        /// Adds the proxy type to the cache.
        /// </summary>
        /// <param name="result">
        /// The proxy result.
        /// </param>
        /// <param name="baseType">
        /// The base type.
        /// </param>
        /// <param name="baseInterfaces">
        /// The base interfaces.
        /// </param>
        public void StoreProxyType(Type result, Type baseType, params Type[] baseInterfaces)
        {
            lock (SyncLock)
            {
                ProxyCacheEntry entry = new ProxyCacheEntry(baseType, baseInterfaces);
                Cache[entry] = result;
            }
        }
    }
}
