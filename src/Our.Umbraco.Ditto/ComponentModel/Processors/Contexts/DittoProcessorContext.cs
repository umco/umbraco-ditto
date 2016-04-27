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
        /// <param name="content">The content.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        internal DittoProcessorContext Populate(
            IPublishedContent content,
            Type targetType,
            PropertyDescriptor propertyDescriptor,
            CultureInfo culture)
        {
            Content = content;
            TargetType = targetType;
            PropertyDescriptor = propertyDescriptor;
            Culture = culture;

            return this;
        }
    }
}