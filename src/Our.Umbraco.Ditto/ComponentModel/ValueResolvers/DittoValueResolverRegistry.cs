namespace Our.Umbraco.Ditto
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Registry for globally registered value resolvers.
    /// </summary>
    internal class DittoValueResolverRegistry
    {
        /// <summary>
        /// The cache for storing handler information.
        /// </summary>
        private static readonly Dictionary<Type, DittoValueResolverAttribute> Cache = new Dictionary<Type, DittoValueResolverAttribute>();

        /// <summary>
        /// Static holder for singleton instance.
        /// </summary>
        private static readonly Lazy<DittoValueResolverRegistry> InternalInstance = new Lazy<DittoValueResolverRegistry>(() => new DittoValueResolverRegistry());

        /// <summary>
        /// The lock object to make Cache access thread safe
        /// </summary>
        private static readonly object CacheLock = new object();

        /// <summary>
        /// Prevents a default instance of the <see cref="DittoValueResolverRegistry"/> class from being created.
        /// </summary>
        private DittoValueResolverRegistry()
        {
        }

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
                return InternalInstance.Value;
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
            this.RegisterResolverAttribute<TObjectType, DittoValueResolverAttribute>(new DittoValueResolverAttribute(typeof(TResolverType)));
        }

        /// <summary>
        /// Registers a global value resolver attribute.
        /// </summary>
        /// <typeparam name="TObjectType">The type of the object being converted.</typeparam>
        /// <typeparam name="TResolverAttributeType">The type of the value resolver attribute.</typeparam>
        public void RegisterResolverAttribute<TObjectType, TResolverAttributeType>()
            where TResolverAttributeType : DittoValueResolverAttribute, new()
        {
            this.RegisterResolverAttribute<TObjectType, TResolverAttributeType>((TResolverAttributeType)typeof(TResolverAttributeType).GetInstance());
        }

        /// <summary>
        /// Registers a global value resolver attribute.
        /// </summary>
        /// <typeparam name="TObjectType">The type of the object type.</typeparam>
        /// <typeparam name="TResolverAttributeType">The type of the resolver attribute type.</typeparam>
        /// <param name="instance">An instance of the value resolver attribute type to use.</param>
        public void RegisterResolverAttribute<TObjectType, TResolverAttributeType>(TResolverAttributeType instance)
            where TResolverAttributeType : DittoValueResolverAttribute
        {
            var objType = typeof(TObjectType);

            lock (CacheLock)
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
        /// <returns>
        /// Returns the registered value resolver attribute for the given object type.
        /// </returns>
        public DittoValueResolverAttribute GetRegisteredResolverAttributeFor(Type objectType)
        {
            lock (CacheLock)
            {
                return Cache.ContainsKey(objectType)
                    ? Cache[objectType]
                    : null;
            }
        }
    }
}
