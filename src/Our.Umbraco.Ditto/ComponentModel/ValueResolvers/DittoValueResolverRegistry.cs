using System;
using System.Collections.Generic;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Registory for globaly registered value resovlers.
    /// </summary>
    internal class DittoValueResolverRegistry
    {
        /// <summary>
        /// The cache for storing handler information.
        /// </summary>
        private static readonly Dictionary<Type, DittoValueResolverAttribute> Cache = new Dictionary<Type, DittoValueResolverAttribute>();

        /// <summary>
        /// The lock object to make Cache access thread safe
        /// </summary>
        private static object _cacheLock = new object();

        /// <summary>
        /// Static holder for singleton instance.
        /// </summary>
        private static readonly Lazy<DittoValueResolverRegistry> _instance = new Lazy<DittoValueResolverRegistry>(() => new DittoValueResolverRegistry());

        /// <summary>
        /// Private constructor to prevent direct instantiation..
        /// </summary>
        private DittoValueResolverRegistry()
        { }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static DittoValueResolverRegistry Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        /// <summary>
        /// Registers a global value resolver.
        /// </summary>
        /// <typeparam name="TObjectType">The type of the object being converted.</typeparam>
        /// <typeparam name="TResolverType">The type of the value resolver.</typeparam>
        public void RegisterResolver<TObjectType, TResolverType>()
            where TResolverType : DittoValueResolver
        {
            RegisterResolverAttribute<TObjectType, DittoValueResolverAttribute>(new DittoValueResolverAttribute(typeof(TResolverType)));
        }

        /// <summary>
        /// Registers a global value resolver attribute.
        /// </summary>
        /// <typeparam name="TObjectType">The type of the object being converted.</typeparam>
        /// <typeparam name="TResolverAttributeType">The type of the value resolver attribute.</typeparam>
        public void RegisterResolverAttribute<TObjectType, TResolverAttributeType>()
            where TResolverAttributeType : DittoValueResolverAttribute, new()
        {
            RegisterResolverAttribute<TObjectType, TResolverAttributeType>((TResolverAttributeType)typeof(TResolverAttributeType).GetInstance());
        }

        /// <summary>
        /// Registers a global value resolver attribute.
        /// </summary>
        /// <typeparam name="TObjectType">The type of the object type.</typeparam>
        /// <typeparam name="TResolverAttributeType">The type of the resolver attribute type.</typeparam>
        /// <param name="instance">An instance of the value resolver attibute type to use.</param>
        public void RegisterResolverAttribute<TObjectType, TResolverAttributeType>(TResolverAttributeType instance)
            where TResolverAttributeType : DittoValueResolverAttribute
        {
            var objType = typeof(TObjectType);

            lock (_cacheLock)
            {
                if (Cache.ContainsKey(objType))
                {
                    Cache[objType] = instance;
                }
                else
                {
                    Cache.Add(objType, instance);
                }
            }
        }

        /// <summary>
        /// Gets the registered value resolver attribute for the given object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns></returns>
        public DittoValueResolverAttribute GetRegisteredResolverAttributeFor(Type objectType)
        {
            lock (_cacheLock)
            {
                return Cache.ContainsKey(objectType)
                    ? Cache[objectType]
                    : null;
            }
        }
    }
}
