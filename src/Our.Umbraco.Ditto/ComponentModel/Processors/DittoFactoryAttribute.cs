using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Factory processor for dynamically typing items based on properties of the item itself.
    /// </summary>
    [AttributeUsage(Ditto.ProcessorAttributeTargets | AttributeTargets.Interface, AllowMultiple = true)]
    public abstract class DittoFactoryAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// Gets or sets the list of allowed types
        /// </summary>
        public Type[] AllowedTypes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DittoFactoryAttribute"/> class.
        /// </summary>
        protected DittoFactoryAttribute()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DittoFactoryAttribute"/> class.
        /// </summary>
        /// <param name="allowedTypes">List of allowed types</param>
        protected DittoFactoryAttribute(Type[] allowedTypes)
            : this()
        {
            AllowedTypes = allowedTypes;
        }

        /// <summary>
        /// Resolves a type name based upon the current content item.
        /// </summary>
        /// <param name="currentContent">The current published content.</param>
        /// <returns>The name.</returns>
        public abstract string ResolveTypeName(IPublishedContent currentContent);

        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        public override object ProcessValue()
        {
            var propType = this.Context.PropertyDescriptor.PropertyType;
            var propTypeIsEnumerable = propType.IsEnumerableType();
            var baseType = propTypeIsEnumerable ? propType.GetEnumerableType() : propType;

            // We have an enumerable processor that runs at the end of every conversion
            // converting individual instances to IEnumerables and vica versa, so we
            // won't worry about returning in the right way, rather we'll just ensure
            // that the IPublishedContent's are converted to the right types
            // and let the enumerable processor handle the rest.

            // TODO: Validate the base type more?

            // Get the list of types to search through
            // If we have explicitly set a list of allowed types, just use those
            // otherwise attempt to search through loaded assemblies
            IEnumerable<Type> types;

            if (AllowedTypes != null && AllowedTypes.Length > 0)
            {
                types = AllowedTypes;
            }
            else
            {
                types = (IEnumerable<Type>)ApplicationContext.ApplicationCache.StaticCache.GetCacheItem(
                    $"DittoFactoryAttribute_ResolveTypes_{baseType.AssemblyQualifiedName}",
                    () =>
                    {
                        // Workaround for http://issues.umbraco.org/issue/U4-9011
                        if (baseType.Assembly.IsAppCodeAssembly())
                        {
                            // This logic is taken from the core type finder so it should be performing the same checks.
                            return baseType.Assembly
                                .GetTypes()
                                .Where(t => baseType.IsAssignableFrom(t)
                                    && t.IsClass
                                    && !t.IsAbstract
                                    && !t.IsSealed
                                    && !t.IsNestedPrivate
                                    && t.GetCustomAttribute<HideFromTypeFinderAttribute>(true) == null)
                                .ToArray();
                        }

                        // Find the appropriate types
                        // There is no non generic version of ResolveTypes so we have to
                        // call it via reflection.
                        var method = typeof(PluginManager).GetMethod("ResolveTypes");
                        var generic = method.MakeGenericMethod(baseType);
                        return ((IEnumerable<Type>)generic.Invoke(PluginManager.Current, new object[] { true, null })).ToArray();
                    });
            }

            // Check for IEnumerable<IPublishedContent> value
            if (this.Value is IEnumerable<IPublishedContent> enumerableValue && enumerableValue != null)
            {
                var items = enumerableValue.Select(x =>
                {
                    var typeName = this.ResolveTypeName(x);
                    var type = types.FirstOrDefault(y => y.Name.InvariantEquals(typeName));

                    return type != null ? x.As(type, chainContext: ChainContext) : null;
                });

                return EnumerableInvocations.Cast(baseType, items);
            }

            // Check for IPublishedContent value
            if (this.Value is IPublishedContent ipublishedContentValue && ipublishedContentValue != null)
            {
                var typeName = this.ResolveTypeName(ipublishedContentValue);
                var type = types.FirstOrDefault(y => y.Name.InvariantEquals(typeName));
                return type != null
                    ? ipublishedContentValue.As(type, chainContext: ChainContext)
                    : null;
            }

            // No other possible options
            return null;
        }
    }
}