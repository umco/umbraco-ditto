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
        /// Initializes a new instance of the <see cref="UmbracoPropertyAttribute"/> class.
        /// </summary>
        public UmbracoPropertyAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoPropertyAttribute"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="altPropertyName">Name of the alternative property.</param>
        /// <param name="recursive">If set to <c>true</c>, a recursive lookup is performed.</param>
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
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the name of the alternative property.
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
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        public override object ProcessValue()
        {
            var defaultValue = this.DefaultValue;

            var recursive = this.Recursive;
            var propName = this.Context.PropertyDescriptor != null ? this.Context.PropertyDescriptor.Name : string.Empty;
            var altPropName = string.Empty;

            // Check for umbraco properties attribute on class
            if (this.Context.TargetType != null)
            {
                var classAttr = this.Context.TargetType
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

            var umbracoPropertyName = this.PropertyName ?? propName;
            var altUmbracoPropertyName = this.AltPropertyName ?? altPropName;

            var content = this.Value as IPublishedContent;
            if (content == null)
            {
                return defaultValue;
            }
            
            object propertyValue = null;

            // Try fetching the value.
            if (!umbracoPropertyName.IsNullOrWhiteSpace())
            {
                propertyValue = GetPropertyValue(content, umbracoPropertyName, recursive);
            }

            // Try fetching the alt value.
            if ((propertyValue == null || propertyValue.ToString().IsNullOrWhiteSpace())
                && !string.IsNullOrWhiteSpace(altUmbracoPropertyName))
            {
                propertyValue = GetPropertyValue(content, altUmbracoPropertyName, recursive);
            }

            // Try setting the default value.
            if ((propertyValue == null || propertyValue.ToString().IsNullOrWhiteSpace())
                && defaultValue != null)
            {
                propertyValue = defaultValue;
            }

            return propertyValue;
        }

        /// <summary>
        /// Gets a property value from the given content object
        /// </summary>
        /// <param name="content"></param>
        /// <param name="umbracoPropertyName"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        private object GetPropertyValue(IPublishedContent content, string umbracoPropertyName, bool recursive)
        {
            var propertyValue = GetOwnPropertyValue(content, umbracoPropertyName);

            if (propertyValue == null || propertyValue.ToString().IsNullOrWhiteSpace())
            {
                propertyValue = GetUmbracoPropertyValue(content, umbracoPropertyName, recursive);
            }

            return propertyValue;
        }

        /// <summary>
        /// Gets a property value from the given content objects own defined properties
        /// </summary>
        /// <param name="content"></param>
        /// <param name="umbracoPropertyName"></param>
        /// <returns></returns>
        private object GetOwnPropertyValue(IPublishedContent content, string umbracoPropertyName)
        {
            var contentProperty = content.GetType().GetProperty(umbracoPropertyName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static);
            if (contentProperty != null && contentProperty.IsMappable())
            {
                // This is more than 2x as fast as propertyValue = contentProperty.GetValue(content, null);
                return PropertyInfoInvocations.GetValue(contentProperty, content);
            }

            return null;
        }

        /// <summary>
        /// Gets a property value from the given content objects umbraco properties collection
        /// </summary>
        /// <param name="content"></param>
        /// <param name="umbracoPropertyName"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        private object GetUmbracoPropertyValue(IPublishedContent content, string umbracoPropertyName, bool recursive)
        {
            return content.GetPropertyValue(umbracoPropertyName, recursive);
        }
    }
}