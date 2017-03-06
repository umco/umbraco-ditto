using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Internal methods for cached invocations.
    /// </summary>
    internal class CachedInvocations
    {
        /// <summary>
        /// The method cache for storing function implementations.
        /// </summary>
        protected static readonly ConcurrentDictionary<MethodBaseCacheItem, Func<object, object>> FunctionCache
            = new ConcurrentDictionary<MethodBaseCacheItem, Func<object, object>>();

        /// <summary>
        /// The method cache for storing action implementations.
        /// </summary>
        protected static readonly ConcurrentDictionary<MethodBaseCacheItem, Action<object, object>> ActionCache
            = new ConcurrentDictionary<MethodBaseCacheItem, Action<object, object>>();

        /// <summary>
        /// Returns a cache key for the given method and type.
        /// </summary>
        /// <param name="type">
        /// The <see cref="object"/> the key reflects.
        /// </param>
        /// <param name="memberName">
        /// The method name. Generated at compile time.
        /// </param>
        /// <returns>
        /// The <see cref="MethodBaseCacheItem"/> for the given method and type.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static MethodBaseCacheItem GetMethodCacheKey(object type, [CallerMemberName] string memberName = null)
        {
            return new MethodBaseCacheItem(memberName, type);
        }
    }
}