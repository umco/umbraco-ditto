using System.Reflection;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The Umbraco Property processor.
    /// </summary>
    public class UmbracoPropertyProcessor : DittoProcessor<IPublishedContent, DittoProcessorContext, UmbracoPropertyProcessorAttribute>
    {
        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        public override object ProcessValue()
        {
            var defaultValue = this.Attribute.DefaultValue;

            var recursive = this.Attribute.Recursive;
            var propName = this.Context.PropertyDescriptor != null ? this.Context.PropertyDescriptor.Name : string.Empty;
            var altPropName = string.Empty;

            // Check for umbraco properties attribute on class
            if (this.Context.PropertyDescriptor != null)
            {
                var classAttr = this.Context.PropertyDescriptor.ComponentType
                    .GetCustomAttribute<UmbracoPropertiesProcessorAttribute>();
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

            var umbracoPropertyName = this.Attribute.PropertyName ?? propName;
            var altUmbracoPropertyName = altPropName;

            var content = this.Value;
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