using System;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The current content value resolver.
    /// </summary>
    public class CurrentContentProcessor : DittoProcessor<IPublishedContent, DittoProcessorContext, CurrentContentProcessorAttribute>
    {
        /// <summary>
        /// Processes the value.
        /// Gets the current <see cref="IPublishedContent"/> from Umbraco.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/> representing the processed value.
        /// </returns>
        public override object ProcessValue()
        {
            // NOTE: [LK] In order to prevent an infinite loop / stack-overflow, we check if the
            // property's type matches the containing model's type, then we throw an exception.
            if (this.Context.PropertyDescriptor.PropertyType == this.Context.PropertyDescriptor.ComponentType)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Unable to process property type '{0}', it is the same as the containing model type.",
                        this.Context.PropertyDescriptor.PropertyType.Name));
            }

            return this.Value;
        }
    }
}