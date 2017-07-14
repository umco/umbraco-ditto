using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Registry for globally registered processors.
    /// </summary>
    internal class DittoProcessorRegistry
    {
        /// <summary>
        /// The cache for storing handler information.
        /// </summary>
        private static readonly Dictionary<Type, List<DittoProcessorAttribute>> Cache = new Dictionary<Type, List<DittoProcessorAttribute>>();

        /// <summary>
        /// Static holder for singleton instance.
        /// </summary>
        private static readonly Lazy<DittoProcessorRegistry> InternalInstance = new Lazy<DittoProcessorRegistry>(() => new DittoProcessorRegistry());

        /// <summary>
        /// The lock object to make Cache access thread safe
        /// </summary>
        private static readonly object CacheLock = new object();

        /// <summary>
        /// The default processor type, (defaults to `UmbracoProperty`).
        /// </summary>
        private Type defaultProcessorType = typeof(UmbracoPropertyAttribute);

        /// <summary>
        /// Prevents a default instance of the <see cref="DittoProcessorRegistry"/> class from being created.
        /// </summary>
        private DittoProcessorRegistry()
        { }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static DittoProcessorRegistry Instance
        {
            get
            {
                return InternalInstance.Value;
            }
        }

        /// <summary>
        /// Registers the default processor attribute.
        /// </summary>
        /// <typeparam name="TProcessorAttributeType">The processor attribute type.</typeparam>
        public void RegisterDefaultProcessorType<TProcessorAttributeType>()
            where TProcessorAttributeType : DittoProcessorAttribute, new()
        {
            this.defaultProcessorType = typeof(TProcessorAttributeType);
        }

        /// <summary>
        /// Registers the processor attribute.
        /// </summary>
        /// <typeparam name="TObjectType">The type of the object type.</typeparam>
        /// <typeparam name="TProcessorAttributeType">The type of the processor attribute type.</typeparam>
        public void RegisterProcessorAttribute<TObjectType, TProcessorAttributeType>()
            where TProcessorAttributeType : DittoProcessorAttribute, new()
        {
            this.RegisterProcessorAttribute<TObjectType, TProcessorAttributeType>((TProcessorAttributeType)typeof(TProcessorAttributeType).GetInstance());
        }

        /// <summary>
        /// Registers the processor attribute.
        /// </summary>
        /// <typeparam name="TObjectType">The type of the object type.</typeparam>
        /// <typeparam name="TProcessorAttributeType">The type of the processor attribute type.</typeparam>
        /// <param name="instance">The instance.</param>
        public void RegisterProcessorAttribute<TObjectType, TProcessorAttributeType>(TProcessorAttributeType instance)
            where TProcessorAttributeType : DittoProcessorAttribute
        {
            var objType = typeof(TObjectType);

            lock (CacheLock)
            {
                if (!Cache.ContainsKey(objType))
                {
                    Cache.Add(objType, new List<DittoProcessorAttribute>());
                }

                Cache[objType].Add(instance);
            }
        }

        /// <summary>
        /// Gets the default processor attribute type for the given object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// Returns the default processor attribute type for the given object type.
        /// </returns>
        public Type GetDefaultProcessorType(Type objectType)
        {
            var attr = objectType.GetCustomAttribute<DittoDefaultProcessorAttribute>();
            if (attr != null)
            {
                return attr.ProcessorType;
            }

            return this.defaultProcessorType;
        }

        /// <summary>
        /// Gets the post-processor attributes.
        /// </summary>
        /// <returns>
        /// Returns the post-processor attributes.
        /// </returns>
        public IEnumerable<DittoProcessorAttribute> GetPostProcessorAttributes()
        {
            // TODO: [LK] Enable the post-processor attributes to be configurable
            yield return new HtmlStringAttribute();
            yield return new EnumerableConverterAttribute();
            yield return new RecursiveDittoAttribute();
            yield return new TryConvertToAttribute();
        }

        /// <summary>
        /// Gets the registered processor attributes for the given object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// Returns the registered processor attributes for the given object type.
        /// </returns>
        public IEnumerable<DittoProcessorAttribute> GetRegisteredProcessorAttributesFor(Type objectType)
        {
            lock (CacheLock)
            {
                return Cache.ContainsKey(objectType)
                    ? Cache[objectType]
                    : Enumerable.Empty<DittoProcessorAttribute>();
            }
        }        
    }
}