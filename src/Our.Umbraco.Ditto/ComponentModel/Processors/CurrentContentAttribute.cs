using System;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The current content processor attribute.
    /// Used for providing Ditto with the current <see cref="IPublishedContent"/> object from Umbraco.
    /// </summary>
    public class CurrentContentAttribute : DittoProcessorAttribute
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
            // NOTE: [LK] In order to prevent an infinite loop / stack-overflow, we check if the
            // property's type matches the containing model's type, then we throw an exception.
            if (this.Context.PropertyDescriptor.PropertyType == this.Context.PropertyDescriptor.ComponentType)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Unable to process property type '{0}', it is the same as the containing model type.",
                        this.Context.PropertyDescriptor.PropertyType.Name));
            }

            return Value;
        }
    }
}