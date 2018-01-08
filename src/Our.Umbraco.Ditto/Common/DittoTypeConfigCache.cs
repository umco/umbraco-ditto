using System;
using System.Collections.Concurrent;

namespace Our.Umbraco.Ditto
{
    internal static class DittoTypeConfigCache
    {
        private static readonly ConcurrentDictionary<Type, DittoTypeConfig> _cache = new ConcurrentDictionary<Type, DittoTypeConfig>();

        public static void Add<T>()
        {
            Add(typeof(T));
        }

        public static void Add(Type type)
        {
            _cache.TryAdd(type, DittoTypeConfig.Create(type));
        }

        public static void Clear<T>()
        {
            Clear(typeof(T));
        }

        public static void Clear(Type type)
        {
            _cache.TryRemove(type, out DittoTypeConfig config);
        }

        public static DittoTypeConfig GetOrAdd<T>()
        {
            return GetOrAdd(typeof(T));
        }

        public static DittoTypeConfig GetOrAdd(Type type)
        {
            if (_cache.TryGetValue(type, out DittoTypeConfig config) == false)
            {
                config = DittoTypeConfig.Create(type);
                _cache.TryAdd(type, config);
            }

            return config;
        }
    }
}