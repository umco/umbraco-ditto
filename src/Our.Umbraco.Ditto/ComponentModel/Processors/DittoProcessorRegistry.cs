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
        /// The cache for storing processors associated by type.
        /// </summary>
        private static readonly Dictionary<Type, List<DittoProcessorAttribute>> ProcessorCache = new Dictionary<Type, List<DittoProcessorAttribute>>();

        /// <summary>
        /// Static holder for singleton instance.
        /// </summary>
        private static readonly Lazy<DittoProcessorRegistry> InternalInstance = new Lazy<DittoProcessorRegistry>(() => new DittoProcessorRegistry());

        /// <summary>
        /// The lock object to make ProcessorCache access thread safe.
        /// </summary>
        private static readonly object ProcessorCacheLock = new object();

        /// <summary>
        /// The default processor type, (defaults to `UmbracoProperty`).
        /// </summary>
        private static Type DefaultProcessorType = typeof(UmbracoPropertyAttribute);

        /// <summary>
        /// The lock object to make DefaultProcessorType access thread safe.
        /// </summary>
        private static readonly object DefaultProcessorTypeLock = new object();

        /// <summary>
        /// The default post-processors.
        /// </summary>
        private static readonly List<Type> PostProcessorTypes = new List<Type>()
        {
            typeof(HtmlStringAttribute),
            typeof(EnumerableConverterAttribute),
            typeof(RecursiveDittoAttribute),
            typeof(TryConvertToAttribute)
        };

        /// <summary>
        /// The lock object to make PostProcessorTypes access thread safe.
        /// </summary>
        private static readonly object PostProcessorTypesLock = new object();

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
            lock (DefaultProcessorTypeLock)
            {
                DefaultProcessorType = typeof(TProcessorAttributeType);
            }
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

            lock (ProcessorCacheLock)
            {
                if (!ProcessorCache.ContainsKey(objType))
                {
                    ProcessorCache.Add(objType, new List<DittoProcessorAttribute>());
                }

                ProcessorCache[objType].Add(instance);
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

            return DefaultProcessorType;
        }

        /// <summary>
        /// Gets the post-processor attributes.
        /// </summary>
        /// <returns>
        /// Returns the post-processor attributes.
        /// </returns>
        public IEnumerable<DittoProcessorAttribute> GetPostProcessorAttributes()
        {
            foreach (var type in PostProcessorTypes)
            {
                yield return (DittoProcessorAttribute)type.GetInstance();
            }
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
            lock (ProcessorCacheLock)
            {
                return ProcessorCache.ContainsKey(objectType)
                    ? ProcessorCache[objectType]
                    : Enumerable.Empty<DittoProcessorAttribute>();
            }
        }

        /// <summary>
        /// Registers a processor attribute to the end of the default set of post-processor attributes.
        /// </summary>
        /// <typeparam name="TProcessorAttributeType"></typeparam>
        /// <param name="position"></param>
        public void RegisterPostProcessorType<TProcessorAttributeType>(int position = -1)
            where TProcessorAttributeType : DittoProcessorAttribute, new()
        {
            lock (PostProcessorTypesLock)
            {
                var type = typeof(TProcessorAttributeType);
                if (!PostProcessorTypes.Contains(type))
                {
                    if (position < 0)
                    {
                        PostProcessorTypes.Add(type);
                    }
                    else
                    {
                        PostProcessorTypes.Insert(position, type);
                    }
                }
            }
        }

        /// <summary>
        /// Deregisters a processor attribute from the default set of post-processor attributes.
        /// </summary>
        /// <typeparam name="TProcessorAttributeType"></typeparam>
        public void DeregisterPostProcessorType<TProcessorAttributeType>()
             where TProcessorAttributeType : DittoProcessorAttribute, new()
        {
            lock (PostProcessorTypesLock)
            {
                PostProcessorTypes.RemoveAll(x => x == typeof(TProcessorAttributeType));
            }
        }
    }
}