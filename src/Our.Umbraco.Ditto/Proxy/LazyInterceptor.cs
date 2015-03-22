namespace Our.Umbraco.Ditto
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Intercepts virtual properties in classes.
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
        public LazyInterceptor(object target, Dictionary<string, object> values)
        {
            this.target = target;

            foreach (KeyValuePair<string, object> kp in values)
            {
                KeyValuePair<string, object> pair = kp;
                this.lazyDictionary.Add(pair.Key, new Lazy<object>(() => pair.Value));
            }
        }

        /// <summary>
        /// Intercepts the method in the proxy to return a replaced value.
        /// </summary>
        /// <param name="info">
        /// The <see cref="InvocationInfo"/> containing information about the current
        /// invoked method or property.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> replacing the original.
        /// </returns>
        public object Intercept(InvocationInfo info)
        {
            const string Getter = "get_";
            const string Setter = "set_";

            // Attempt to get the value from the lazy members.
            var name = info.TargetMethod.Name;
            var key = name.TrimStart(Getter.ToCharArray());
            if (name.StartsWith(Getter))
            {
                if (this.lazyDictionary.ContainsKey(key))
                {
                    return this.lazyDictionary[key].Value;
                }
            }

            // Set the value, remove the old lazy value.
            var value = info.TargetMethod.Invoke(this.target, info.Arguments);
            if (name.StartsWith(Setter))
            {
                key = name.TrimStart(Setter.ToCharArray());
                if (this.lazyDictionary.ContainsKey(key))
                {
                    this.lazyDictionary.Remove(key);
                }
            }

            return value;
        }
    }
}
