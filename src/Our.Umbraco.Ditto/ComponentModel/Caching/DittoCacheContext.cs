using System;
using System.ComponentModel;
using System.Globalization;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Represents a context object for ditto cache
    /// </summary>
    public class DittoCacheContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DittoCacheContext"/> class.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="content">The content.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="culture">The culture.</param>
        internal DittoCacheContext(
            DittoCacheableAttribute attribute, 
            IPublishedContent content, 
            Type targetType, 
            CultureInfo culture)
            : this(attribute, content, targetType, null, culture)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DittoCacheContext"/> class.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="content">The content.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <param name="culture">The culture.</param>
        internal DittoCacheContext(
            DittoCacheableAttribute attribute, 
            IPublishedContent content, 
            Type targetType, 
            PropertyDescriptor propertyDescriptor, 
            CultureInfo culture)
        {
            this.Attribute = attribute;
            this.Content = content;
            this.TargetType = targetType;
            this.Culture = culture;
            this.PropertyDescriptor = propertyDescriptor;
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public IPublishedContent Content { get; internal set; }

        /// <summary>
        /// Gets the type of the target.
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
        /// Gets the attribute.
        /// </summary>
        /// <value>
        /// The attribute.
        /// </value>
        public DittoCacheableAttribute Attribute { get; internal set; }
    }
}