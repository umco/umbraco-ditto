namespace Our.Umbraco.Ditto
{
    using System.ComponentModel;
    using System.Globalization;

    using global::Umbraco.Core.Models;
    using global::Umbraco.Web;
    using System.Web.Configuration;

    /// <summary>
    /// The web.config app setting value resolver.
    /// </summary>
    public class AppSettingValueResolver : DittoValueResolver<AppSettingAttribute>
    {
        /// <summary>
        /// Gets the value from the web.config app setting
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="attribute">
        /// The <see cref="AppSettingAttribute"/> containing additional information 
        /// indicating how to resolve the property.
        /// </param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <returns>
        /// The <see cref="object"/> representing the raw value.
        /// </returns>
        public override object ResolveValue(ITypeDescriptorContext context, AppSettingAttribute attribute, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(attribute.AppSettingKey))
            {
                return null;
            }

            return WebConfigurationManager.AppSettings[attribute.AppSettingKey];
        }
    }
}