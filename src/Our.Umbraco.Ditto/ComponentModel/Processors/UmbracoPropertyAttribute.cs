using System;
using System.Reflection;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The Umbraco property processor attribute.
    /// </summary>
    public class UmbracoPropertyAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the name of the alterative property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public string AltPropertyName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UmbracoPropertyAttribute"/> is recursive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if recursive; otherwise, <c>false</c>.
        /// </value>
        public bool Recursive { get; set; }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>
        /// The default value.
        /// </value>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoPropertyAttribute"/> class.
        /// </summary>
        public UmbracoPropertyAttribute()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoPropertyAttribute"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="altPropertyName">Name of the alternative property.</param>
        /// <param name="recursive">if set to <c>true</c> recurse.</param>
        /// <param name="defaultValue">The default value.</param>
        public UmbracoPropertyAttribute(
            string propertyName,
            string altPropertyName = null,
            bool recursive = false,
            object defaultValue = null)
        {
            this.PropertyName = propertyName;
            this.AltPropertyName = altPropertyName;
            this.Recursive = recursive;
            this.DefaultValue = defaultValue;
        }

        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override object ProcessValue()
        {
            var defaultValue = DefaultValue;

            var recursive = Recursive;
            var propName = this.Context.PropertyDescriptor != null ? this.Context.PropertyDescriptor.Name : string.Empty;
            var altPropName = string.Empty;

            // Check for umbraco properties attribute on class
            if (this.Context.PropertyDescriptor != null)
            {
                var classAttr = this.Context.PropertyDescriptor.ComponentType
                    .GetCustomAttribute<UmbracoPropertiesAttribute>();
                if (classAttr != null)
                {
                    // Apply the prefix
                    if (!string.IsNullOrWhiteSpace(classAttr.Prefix))
                    {
                        altPropName = propName;
                        propName = classAttr.Prefix + propName;
                    }

                    // Apply global recursive setting
                    recursive |= classAttr.Recursive;
                }
            }

            var umbracoPropertyName = PropertyName ?? propName;
            var altUmbracoPropertyName = AltPropertyName ?? altPropName;

            var content = this.Context.Content;
            if (content == null)
            {
                return defaultValue;
            }

            var contentType = content.GetType();
            object propertyValue = null;

            // Try fetching the value.
            if (!umbracoPropertyName.IsNullOrWhiteSpace())
            {
                var contentProperty = contentType.GetProperty(umbracoPropertyName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static);

                if (contentProperty != null)
                {
                    propertyValue = contentProperty.GetValue(content, null);
                }

                if (propertyValue == null)
                {
                    propertyValue = content.GetPropertyValue(umbracoPropertyName, recursive);
                }
            }

            // Try fetching the alt value.
            if ((propertyValue == null || propertyValue.ToString().IsNullOrWhiteSpace())
                && !string.IsNullOrWhiteSpace(altUmbracoPropertyName))
            {
                var contentProperty = contentType.GetProperty(altUmbracoPropertyName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static);

                if (contentProperty != null)
                {
                    propertyValue = contentProperty.GetValue(content, null);
                }

                if (propertyValue == null)
                {
                    propertyValue = content.GetPropertyValue(altUmbracoPropertyName, recursive);
                }
            }

            // Try setting the default value.
            if ((propertyValue == null || propertyValue.ToString().IsNullOrWhiteSpace())
                && defaultValue != null)
            {
                propertyValue = defaultValue;
            }

            return propertyValue;
        }
    }
}