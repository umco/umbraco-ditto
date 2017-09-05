using System;
using System.Collections.Generic;
using System.Reflection;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Intercepts virtual properties in classes replacing them with lazily implemented versions.
    /// </summary>
    internal class LazyInterceptor : IInterceptor
    {
        private readonly Dictionary<string, Lazy<object>> lazyDictionary = new Dictionary<string, Lazy<object>>();
        private readonly Dictionary<string, object> nonLazyDictionary = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyInterceptor"/> class.
        /// </summary>
        /// <param name="values">
        /// The dictionary of values containing the property name to replace and the value to replace it with.
        /// </param>
        public LazyInterceptor(Dictionary<string, Lazy<object>> values)
        {
            foreach (KeyValuePair<string, Lazy<object>> pair in values)
            {
                this.lazyDictionary.Add(pair.Key, pair.Value);
            }
        }

        /// <inheritdoc />
        public object Intercept(MethodBase methodBase, object value)
        {
            const string getter = "get_";
            const string setter = "set_";
            string name = methodBase.Name;
            string key = name.Substring(4);

            // Attempt to get the value from the lazy members.
            if (name.StartsWith(getter))
            {
                if (this.lazyDictionary.ContainsKey(key))
                {
                    return this.lazyDictionary[key].Value;
                }

                if (this.nonLazyDictionary.ContainsKey(key))
                {
                    return this.nonLazyDictionary[key];
                }
            }

            // Set the value, remove the old lazy value.
            if (name.StartsWith(setter))
            {
                if (this.lazyDictionary.ContainsKey(key))
                {
                    this.lazyDictionary.Remove(key);
                }

                this.nonLazyDictionary[key] = value;
            }

            return null;
        }
    }
}