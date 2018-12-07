using System;
using System.Linq;
using Umbraco.Core.Logging;
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
            PropertySource = Ditto.DefaultPropertySource;
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
            : this()
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
        /// Gets or sets the property source from which to map values from
        /// </summary>
        public PropertySource PropertySource { get; set; }

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
            var propName = this.Context.PropertyInfo != null ? this.Context.PropertyInfo.Name : string.Empty;
            var altPropName = string.Empty;

            // Check for Umbraco properties attribute on class
            if (Ditto.TryGetTypeAttribute(this.Context.TargetType, out UmbracoPropertiesAttribute classAttr))
            {
                // Apply the prefix
                if (string.IsNullOrWhiteSpace(classAttr.Prefix) == false)
                {
                    altPropName = propName;
                    propName = classAttr.Prefix + propName;
                }

                // Apply global recursive setting
                recursive |= classAttr.Recursive;

                // Apply property source only if it's different from the default,
                // and the current value is the default. We only do it this
                // way because if they change it at the property level, we
                // want that to take precedence over the class level.
                if (classAttr.PropertySource != Ditto.DefaultPropertySource && PropertySource == Ditto.DefaultPropertySource)
                {
                    PropertySource = classAttr.PropertySource;
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
            if (string.IsNullOrWhiteSpace(umbracoPropertyName) == false)
            {
                propertyValue = GetPropertyValue(content, umbracoPropertyName, recursive);
            }

            // Try fetching the alt value.
            if ((propertyValue == null || (propertyValue is string tmp && string.IsNullOrWhiteSpace(tmp)))
                && string.IsNullOrWhiteSpace(altUmbracoPropertyName) == false)
            {
                propertyValue = GetPropertyValue(content, altUmbracoPropertyName, recursive);
            }

            // Try setting the default value.
            if ((propertyValue == null || (propertyValue is string tmp2 && string.IsNullOrWhiteSpace(tmp2)))
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
        protected object GetPropertyValue(IPublishedContent content, string umbracoPropertyName, bool recursive)
        {
            object propertyValue = null;

            if (PropertySource == PropertySource.InstanceProperties || PropertySource == PropertySource.InstanceThenUmbracoProperties)
            {
                propertyValue = GetClassPropertyValue(content, umbracoPropertyName);
            }

            if ((propertyValue == null || (propertyValue is string tmp && string.IsNullOrWhiteSpace(tmp)))
                && (PropertySource != PropertySource.InstanceProperties))
            {
                propertyValue = GetUmbracoPropertyValue(content, umbracoPropertyName, recursive);
            }

            if ((propertyValue == null || (propertyValue is string tmp2 && string.IsNullOrWhiteSpace(tmp2)))
                && PropertySource == PropertySource.UmbracoThenInstanceProperties)
            {
                propertyValue = GetClassPropertyValue(content, umbracoPropertyName);
            }

            return propertyValue;
        }

        /// <summary>
        /// Gets a property value from the given content objects class properties
        /// </summary>
        /// <param name="content"></param>
        /// <param name="umbracoPropertyName"></param>
        /// <returns></returns>
        private object GetClassPropertyValue(IPublishedContent content, string umbracoPropertyName)
        {
            var contentType = content.GetType();
            var contentProperty = contentType.GetProperty(umbracoPropertyName, Ditto.MappablePropertiesBindingFlags);
            if (contentProperty != null && contentProperty.IsMappable())
            {
#if DEBUG
                if (PropertySource == PropertySource.InstanceThenUmbracoProperties
                    && Ditto.IPublishedContentProperties.Any(x => string.Equals(x.Name, umbracoPropertyName, StringComparison.InvariantCultureIgnoreCase))
                    && content.HasProperty(umbracoPropertyName))
                {
                    // Property is an IPublishedContent property and an Umbraco property exists so warn the user
                    LogHelper.Warn<UmbracoPropertyAttribute>($"The property {umbracoPropertyName} being mapped from content type {contentType.Name}'s instance properties hides a property in the Umbraco properties collection of the same name. It is recommended that you avoid using Umbraco property aliases that conflict with IPublishedContent instance property names, but if you can't avoid this and you require access to the hidden property you can use the PropertySource parameter of the processors attribute to override the order in which properties are checked.");
                }
#endif

                // This is over 4x faster than propertyValue = contentProperty.GetValue(content, null);
                return FastPropertyAccessor.GetValue(contentProperty, content);
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
#if DEBUG
            if (PropertySource == PropertySource.UmbracoThenInstanceProperties
                && Ditto.IPublishedContentProperties.Any(x => string.Equals(x.Name, umbracoPropertyName, StringComparison.InvariantCultureIgnoreCase))
                && content.HasProperty(umbracoPropertyName))
            {
                // Property is an IPublishedContent property and an Umbraco property exists so warn the user
                LogHelper.Warn<UmbracoPropertyAttribute>($"The property {umbracoPropertyName} being mapped from the Umbraco properties collection hides an instance property of the same name on content type {content}. It is recommended that you avoid using Umbraco property aliases that conflict with IPublishedContent instance property names, but if you can't avoid this and you require access to the hidden property you can use the PropertySource parameter of the processors attribute to override the order in which properties are checked.");
            }
#endif

            return content.GetPropertyValue(umbracoPropertyName, recursive);
        }
    }

    /// <summary>
    /// Defines the source from which the <see cref="UmbracoPropertyAttribute"/> should map values from
    /// </summary>
    public enum PropertySource
    {
        /// <summary>
        /// Properties declared on the content instance only
        /// </summary>
        InstanceProperties,

        /// <summary>
        /// Properties declared in the umbraco properties collection only
        /// </summary>
        UmbracoProperties,

        /// <summary>
        /// Properties declared on the content instance, followed by properties in the umbraco properties collection if no value is found
        /// </summary>
        InstanceThenUmbracoProperties,

        /// <summary>
        /// Properties declared in the umbraco properties collection, followed by the content instance properties if no value is found
        /// </summary>
        UmbracoThenInstanceProperties
    }
}