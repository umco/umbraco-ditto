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

            // Find the appropreate types
            // There is no non generic version of ResolveTypes so we have to
            // call it via reflection.
            var method = typeof(PluginManager).GetMethod("ResolveTypes");
            var generic = method.MakeGenericMethod(baseType);
            var types = (IEnumerable<Type>)generic.Invoke(PluginManager.Current, new object[] { true, null });

            // Check for IEnumerable<IPublishedContent> value
            var enumerableValue = this.Value as IEnumerable<IPublishedContent>;
            if (enumerableValue != null)
            {
                var items = enumerableValue.Select(x =>
                {
                    var typeName = this.ResolveTypeName(x);
                    var type = types.FirstOrDefault(y => y.Name.InvariantEquals(typeName));

                    return type != null ? x.As(type) : null;
                });

                return EnumerableInvocations.Cast(baseType, items);
            }

            // Check for IPublishedContent value
            var ipublishedContentValue = this.Value as IPublishedContent;
            if (ipublishedContentValue != null)
            {
                var typeName = this.ResolveTypeName(ipublishedContentValue);
                var type = types.FirstOrDefault(y => y.Name.InvariantEquals(typeName));
                return type != null ? ipublishedContentValue.As(type) : null;
            }

            // No other possible options
            return null;
        }
    }
}