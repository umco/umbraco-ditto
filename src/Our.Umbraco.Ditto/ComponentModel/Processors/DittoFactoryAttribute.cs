using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Factory processor for dynamically typing an items based on properties of the item itself 
    /// </summary>
    public class DittoFactoryAttribute : DittoProcessorAttribute
    {
        private Type _factoryTypeNameResovlerType;

        /// <summary>
        /// Instantiates an instance of DittoFactoryAttribute
        /// </summary>
        /// <param name="factoryTypeNameResovlerType"></param>
        public DittoFactoryAttribute(Type factoryTypeNameResovlerType)
        {
            if (!typeof(DittoFactoryTypeNameResolver).IsAssignableFrom(factoryTypeNameResovlerType))
            {
                throw new ArgumentException("The factoryTypeNameResovler argument must implement DittoFactoryTypeNameResolver", "factoryTypeNameResovler");
            }

            _factoryTypeNameResovlerType = factoryTypeNameResovlerType;
        }

        /// <summary>
        /// Processes the incoming value
        /// </summary>
        /// <returns></returns>
        public override object ProcessValue()
        {
            var propType = Context.PropertyDescriptor.PropertyType;
            var propTypeIsEnumerable = propType.IsEnumerableType();
            var baseType = propTypeIsEnumerable ? propType.GetEnumerableType() : propType;

            // TODO: Validate the base type more?

            // Find the appropreate types
            // There is no non generic version of ResolveTypes so we have to
            // call it via reflection
            var method = typeof(PluginManager).GetMethod("ResolveTypes");
            var generic = method.MakeGenericMethod(baseType);
            var types = (IEnumerable<Type>)generic.Invoke(PluginManager.Current, new object[] { true, null });

            // Resolve the type name
            var typeNameResolver = (DittoFactoryTypeNameResolver)_factoryTypeNameResovlerType.GetInstance();

            // Check for IEnumerable<IPublishedContent> value
            var enumerableValue = Value as IEnumerable<IPublishedContent>;
            if (enumerableValue != null && propTypeIsEnumerable)
            {
                var items = enumerableValue.Select(x =>
                {
                    var typeName = typeNameResolver.ResolveTypeName(Context, x);
                    var type = types.FirstOrDefault(y => y.Name.InvariantEquals(typeName));

                    // QUESTION: Should we return null? throw an exception? or strip items if a model can't be found?

                    return type != null ? x.As(type): null;
                });

                return EnumerableInvocations.Cast(baseType, items);
            }

            // Check for IPublishedContent value
            var ipublishedContentValue = Value as IPublishedContent;
            if (ipublishedContentValue != null && !propTypeIsEnumerable)
            {
                var typeName = typeNameResolver.ResolveTypeName(Context, ipublishedContentValue);
                var type = types.FirstOrDefault(y => y.Name.InvariantEquals(typeName));
                return type != null ? ipublishedContentValue.As(type) : null;
            }

            // No other possible options
            return null;
        }
    }
}
