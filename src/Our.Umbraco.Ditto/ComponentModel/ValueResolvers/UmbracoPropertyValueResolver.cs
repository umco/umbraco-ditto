namespace Our.Umbraco.Ditto
{
    using System.ComponentModel;
    using System.Globalization;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Web;

    /// <summary>
    /// The Umbraco property value resolver.
    /// </summary>
    public class UmbracoPropertyValueResolver : DittoValueResolver<UmbracoPropertyAttribute>
    {
        /// <summary>
        /// Gets the raw value for the current property from Umbraco.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="attribute">
        /// The <see cref="UmbracoPropertyAttribute"/> containing additional information 
        /// indicating how to resolve the property.
        /// </param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <returns>
        /// The <see cref="object"/> representing the raw value.
        /// </returns>
        public override object ResolveValue(ITypeDescriptorContext context, UmbracoPropertyAttribute attribute, CultureInfo culture)
        {
            var defaultValue = attribute.DefaultValue;

            var umbracoPropertyName = attribute.PropertyName ?? (context.PropertyDescriptor != null ? context.PropertyDescriptor.Name : string.Empty);
            var altUmbracoPropertyName = attribute.AltPropertyName ?? string.Empty;
            var recursive = attribute.Recursive;

            var content = context.Instance as IPublishedContent;
            if (content == null)
            {
                return defaultValue;
            }

            var contentType = content.GetType();
            object propertyValue = null;

            // Try fetching the value.
            if (!umbracoPropertyName.IsNullOrWhiteSpace())
            {
                var contentProperty = contentType.GetProperty(umbracoPropertyName);
                propertyValue = contentProperty != null
                    ? contentProperty.GetValue(content, null)
                    : content.GetPropertyValue(umbracoPropertyName, recursive);
            }

            // Try fetching the alt value.
            if ((propertyValue == null || propertyValue.ToString().IsNullOrWhiteSpace())
                && !string.IsNullOrWhiteSpace(altUmbracoPropertyName))
            {
                var contentProperty = contentType.GetProperty(altUmbracoPropertyName);
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
