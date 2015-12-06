using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// An enumerable ditto processor
    /// </summary>
    internal class RecursiveDittoAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override object ProcessValue()
        {
            var result = Value;

            // If we aren't already the right type, try recursing if the type is IPublishedContent
            if (Value != null && !Context.PropertyDescriptor.PropertyType.IsInstanceOfType(Value))
            {
                if (Value is IPublishedContent && Context.PropertyDescriptor.PropertyType.IsClass)
                {
                    // If the property value is an IPublishedContent, then we can use Ditto to map to the target type.
                    result = ((IPublishedContent)Value).As(Context.PropertyDescriptor.PropertyType);
                }
                else if (Value != null && Value.GetType().IsEnumerableOfType(typeof(IPublishedContent))
                    && Context.PropertyDescriptor.PropertyType.IsEnumerable()
                    && Context.PropertyDescriptor.PropertyType.GetEnumerableType() != null
                    && Context.PropertyDescriptor.PropertyType.GetEnumerableType().IsClass)
                {
                    // If the property value is IEnumerable<IPublishedContent>, then we can use Ditto to map to the target type.
                    result = ((IEnumerable<IPublishedContent>)Value).As(Context.PropertyDescriptor.PropertyType.GetEnumerableType());
                }
            }

            return result;
        }
    }
}
