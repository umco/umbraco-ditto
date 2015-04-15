namespace Our.Umbraco.Ditto
{
    using System;
    using System.Globalization;
    using System.Reflection;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Web;

    /// <summary>
    /// The Umbraco property attribute. 
    /// Used for providing Umbraco with additional information about the property to aid conversion.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class UmbracoPropertyAttribute : DittoValueAttribute
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
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="altPropertyName">
        /// The alternative property name.
        /// </param>
        /// <param name="recursive">
        /// Whether the property should be retrieved recursively up the tree.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        public UmbracoPropertyAttribute(
            string propertyName,
            string altPropertyName = "",
            bool recursive = false,
            object defaultValue = null)
        {
            this.PropertyName = propertyName;
            this.AltPropertyName = altPropertyName;
            this.Recursive = recursive;
            this.DefaultValue = defaultValue;
        }

        /// <summary>
        /// Gets or sets the property name.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the alternative property name.
        /// </summary>
        public string AltPropertyName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the property should be retrieved recursively up the tree.
        /// </summary>
        public bool Recursive { get; set; }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Returns the value for the given type and property.
        /// </summary>
        /// <param name="content">The <see cref="IPublishedContent"/> to convert.</param>
        /// <param name="type">The <see cref="Type"/> of items to return.</param>
        /// <param name="culture">The <see cref="CultureInfo"/></param>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> property info associated with the type.</param>
        /// <returns>The <see cref="object"/> representing the value.</returns>
        public override object GetValue(
            IPublishedContent content,
            Type type,
            CultureInfo culture,
            PropertyInfo propertyInfo)
        {
            var contentType = content.GetType();
            var umbracoPropertyName = this.PropertyName ?? propertyInfo.Name;
            var altUmbracoPropertyName = this.AltPropertyName ?? string.Empty;
            var recursive = this.Recursive;
            object defaultValue = this.DefaultValue;

            // Try fetching the value.
            var contentProperty = contentType.GetProperty(umbracoPropertyName);
            object propertyValue = contentProperty != null
                ? contentProperty.GetValue(content, null)
                : content.GetPropertyValue(umbracoPropertyName, recursive);

            // Try fetching the alt value.
            if ((propertyValue == null || propertyValue.ToString().IsNullOrWhiteSpace())
                && !string.IsNullOrWhiteSpace(altUmbracoPropertyName))
            {
                contentProperty = contentType.GetProperty(altUmbracoPropertyName);
                propertyValue = contentProperty != null
                    ? contentProperty.GetValue(content, null)
                    : content.GetPropertyValue(altUmbracoPropertyName, recursive);
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