using System.ComponentModel;

namespace Our.Umbraco.Ditto
{
    using System.Globalization;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Web;

    public class UmbracoPropertyValueResolver : DittoValueResolver<UmbracoPropertyAttribute>
    {
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
