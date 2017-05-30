using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// A recursive Ditto processor
    /// </summary>
    internal class RecursiveDittoAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// Gets or sets the processor contexts.
        /// </summary>
        public IEnumerable<DittoProcessorContext> ProcessorContexts { get; set; }

        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        public override object ProcessValue()
        {
            var result = this.Value;

            // If we aren't already the right type, try recursing if the type is IPublishedContent
            if (this.Value != null && !this.Context.PropertyDescriptor.PropertyType.IsInstanceOfType(this.Value))
            {
                if (this.Value is IPublishedContent && this.Context.PropertyDescriptor.PropertyType.IsClass)
                {
                    // If the property value is an IPublishedContent, then we can use Ditto to map to the target type.
                    result = ((IPublishedContent)this.Value).As(
                        this.Context.PropertyDescriptor.PropertyType,
                        this.Context.Culture,
                        null,
                        chainContext:ChainContext);
                }
                else if (this.Value != null && this.Value.GetType().IsEnumerableOfType(typeof(IPublishedContent))
                    && this.Context.PropertyDescriptor.PropertyType.IsEnumerable()
                    && this.Context.PropertyDescriptor.PropertyType.GetEnumerableType() != null
                    && this.Context.PropertyDescriptor.PropertyType.GetEnumerableType().IsClass)
                {
                    // If the property value is IEnumerable<IPublishedContent>, then we can use Ditto to map to the target type.
                    result = ((IEnumerable<IPublishedContent>)this.Value).As(
                        this.Context.PropertyDescriptor.PropertyType.GetEnumerableType(),
                        this.Context.Culture,
                        chainContext: ChainContext);
                }
            }

            return result;
        }
    }
}