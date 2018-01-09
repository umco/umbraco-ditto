using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
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
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="culture">The culture.</param>
        internal DittoCacheContext(
            DittoCacheableAttribute attribute,
            IPublishedContent content,
            Type targetType,
            PropertyInfo propertyInfo,
            CultureInfo culture)
        {
            this.Attribute = attribute;
            this.Content = content;
            this.TargetType = targetType;
            this.Culture = culture;
            this.PropertyInfo = propertyInfo;
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
        [Obsolete("PropertyDescriptor has been deprecated, please use PropertyInfo instead. This property will be removed in a future Ditto version.", false)]
        public PropertyDescriptor PropertyDescriptor
        {
            get
            {
                return this.TargetType != null && this.PropertyInfo != null
                    ? TypeDescriptor.GetProperties(this.TargetType)[this.PropertyInfo.Name]
                    : null;
            }
        }

        /// <summary>
        /// Gets the property info.
        /// </summary>
        /// <value>
        /// The property info.
        /// </value>
        public PropertyInfo PropertyInfo { get; set; }

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