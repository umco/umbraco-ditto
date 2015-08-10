namespace Our.Umbraco.Ditto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Registry for globally registered conversion handlers.
    /// </summary>
    internal class DittoConversionHandlerRegistry
    {
        /// <summary>
        /// The cache for storing handler information.
        /// </summary>
        private static readonly Dictionary<Type, IList<Type>> Cache = new Dictionary<Type, IList<Type>>();

        /// <summary>
        /// Static holder for singleton instance.
        /// </summary>
        private static readonly Lazy<DittoConversionHandlerRegistry> InternalInstance = new Lazy<DittoConversionHandlerRegistry>(() => new DittoConversionHandlerRegistry());

        /// <summary>
        /// The lock object to make Cache access thread safe
        /// </summary>
        private static object cacheLock = new object();

        /// <summary>
        /// Prevents a default instance of the <see cref="DittoConversionHandlerRegistry"/> class from being created.
        /// </summary>
        private DittoConversionHandlerRegistry()
        {
        }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static DittoConversionHandlerRegistry Instance
        {
            get
            {
                return InternalInstance.Value;
            }
        }

        /// <summary>
        /// Registers a global conversion handler.
        /// </summary>
        /// <typeparam name="TObjectType">The type of the object being converted.</typeparam>
        /// <typeparam name="THandlerType">The type of the handler.</typeparam>
        public void RegisterHandler<TObjectType, THandlerType>()
            where THandlerType : DittoConversionHandler
        {
            var objType = typeof(TObjectType);
            var handlerType = typeof(THandlerType);

            lock (cacheLock)
            {
                if (Cache.ContainsKey(objType))
                {
                    if (!Cache[objType].Contains(handlerType))
                    {
                        Cache[objType].Add(handlerType);
                    }
                }
                else
                {
                    Cache.Add(objType, new[] { handlerType });
                }
            }
        }

        /// <summary>
        /// Gets the registered handler types for the given object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// Returns a list of registered handlers for the given object type.
        /// </returns>
        public IEnumerable<Type> GetRegisteredHandlerTypesFor(Type objectType)
        {
            lock (cacheLock)
            {
                return Cache.ContainsKey(objectType)
                    ? Cache[objectType].ToList()
                    : Enumerable.Empty<Type>();
            }
        }
    }
}