using System;
using System.Collections.Concurrent;

namespace Our.Umbraco.Ditto
{
    internal static class DittoTypeInfoCache
    {
        private static readonly ConcurrentDictionary<Type, DittoTypeInfo> _cache = new ConcurrentDictionary<Type, DittoTypeInfo>();

        public static void Add<T>()
        {
            Add(typeof(T));
        }

        public static void Add(Type type)
        {
            _cache.TryAdd(type, DittoTypeInfo.Create(type));
        }

        public static void Clear<T>()
        {
            Clear(typeof(T));
        }

        public static void Clear(Type type)
        {
            _cache.TryRemove(type, out DittoTypeInfo config);
        }

        public static DittoTypeInfo GetOrAdd<T>()
        {
            return GetOrAdd(typeof(T));
        }

        public static DittoTypeInfo GetOrAdd(Type type)
        {
            if (_cache.TryGetValue(type, out DittoTypeInfo config) == false)
            {
                config = DittoTypeInfo.Create(type);
                _cache.TryAdd(type, config);
            }

            return config;
        }
    }
}