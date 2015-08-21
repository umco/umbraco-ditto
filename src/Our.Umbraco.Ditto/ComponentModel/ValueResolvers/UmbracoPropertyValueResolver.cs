namespace Our.Umbraco.Ditto
{
    using System.Reflection;
    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Web;

    /// <summary>
    /// The Umbraco property value resolver.
    /// </summary>
    public class UmbracoPropertyValueResolver : DittoValueResolver<DittoValueResolverContext, UmbracoPropertyAttribute>
    {
        /// <summary>
        /// Gets the raw value for the current property from Umbraco.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/> representing the raw value.
        /// </returns>
        public override object ResolveValue()
        {
            var defaultValue = this.Attribute.DefaultValue;

            var recursive = this.Attribute.Recursive;
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

            var umbracoPropertyName = this.Attribute.PropertyName ?? propName;
            var altUmbracoPropertyName = this.Attribute.AltPropertyName ?? altPropName;

            var content = this.Context.Instance as IPublishedContent;
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
                propertyValue = contentProperty != null
                    ? contentProperty.GetValue(content, null)
                    : content.GetPropertyValue(umbracoPropertyName, recursive);
            }

            // Try fetching the alt value.
            if ((propertyValue == null || propertyValue.ToString().IsNullOrWhiteSpace())
                && !string.IsNullOrWhiteSpace(altUmbracoPropertyName))
            {
                var contentProperty = contentType.GetProperty(altUmbracoPropertyName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static);
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
