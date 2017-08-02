using System;
using System.ComponentModel;
using System.Globalization;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Context object passed into processor instances to give context about the conversion taking place
    /// </summary>
    public class DittoProcessorContext
    {
        /// <summary>
        /// Gets the IPublishedContent instance being processed.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public IPublishedContent Content { get; internal set; }

        /// <summary>
        /// Gets the target type of value being processed.
        /// </summary>
        /// <value>
        /// The type of the target.
        /// </value>
        public Type TargetType { get; internal set; }

        /// <summary>
        /// Gets the property descriptor.
        /// </summary>
        /// <value>
        /// The property descriptor.
        /// </value>
        public PropertyDescriptor PropertyDescriptor { get; internal set; }

        /// <summary>
        /// Gets the culture.
        /// </summary>
        /// <value>
        /// The culture.
        /// </value>
        public CultureInfo Culture { get; internal set; }

        /// <summary>
        /// Populates the core context fields.
        /// </summary>
        /// <param name="baseProcessorContext">The base processor content.</param>
        /// <returns>Returns the Ditto processors context.</returns>
        internal DittoProcessorContext Populate(DittoProcessorContext baseProcessorContext)
        {
            Content = baseProcessorContext.Content;
            TargetType = baseProcessorContext.TargetType;
            PropertyDescriptor = baseProcessorContext.PropertyDescriptor;
            Culture = baseProcessorContext.Culture;

            return this;
        }

        /// <summary>
        /// Shallow clones the current context.
        /// </summary>
        /// <returns>Returns a cloned Ditto processor context.</returns>
        internal DittoProcessorContext Clone()
        {
            return (DittoProcessorContext)MemberwiseClone();
        }
    }
}