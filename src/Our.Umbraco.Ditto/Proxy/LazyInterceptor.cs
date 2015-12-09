namespace Our.Umbraco.Ditto
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Intercepts virtual properties in classes replacing them with lazily implemented versions.
    /// </summary>
    internal class LazyInterceptor : IInterceptor
    {
        /// <summary>
        /// The lazy dictionary.
        /// </summary>
        private readonly Dictionary<string, Lazy<object>> lazyDictionary = new Dictionary<string, Lazy<object>>();

        /// <summary>
        /// The base target instance from which the proxy type is derived.
        /// </summary>
        private readonly object target;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyInterceptor"/> class.
        /// </summary>
        /// <param name="target">
        /// The base target instance from which the proxy type is derived.
        /// </param>
        /// <param name="values">
        /// The dictionary of values containing the property name to replace and the value to replace it with.
        /// </param>
        public LazyInterceptor(object target, Dictionary<string, Lazy<object>> values)
        {
            this.target = target;

            foreach (KeyValuePair<string, Lazy<object>> pair in values)
            {
                this.lazyDictionary.Add(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Intercepts the <see cref="MethodBase"/> in the proxy to return a replaced value.
        /// </summary>
        /// <param name="methodBase">
        /// The <see cref="MethodBase"/> containing information about the current
        /// invoked property.
        /// </param>
        /// <param name="value">
        /// The object to set the <see cref="MethodBase"/> to if it is a setter.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> replacing the original implementation value.
        /// </returns>
        public object Intercept(MethodBase methodBase, object value)
        {
            const string Getter = "get_";
            const string Setter = "set_";
            var name = methodBase.Name;
            var key = name.Substring(4);
            var parameters = value == null ? new object[] { } : new[] { value };

            // Attempt to get the value from the lazy members.
            if (name.StartsWith(Getter))
            {
                if (this.lazyDictionary.ContainsKey(key))
                {
                    return this.lazyDictionary[key].Value;
                }
            }

            // Set the value, remove the old lazy value.
            if (name.StartsWith(Setter))
            {
                if (this.lazyDictionary.ContainsKey(key))
                {
                    this.lazyDictionary.Remove(key);
                }
            }

            return methodBase.Invoke(this.target, parameters);
        }
    }
}