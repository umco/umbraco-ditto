namespace Our.Umbraco.Ditto
{
    using System.ComponentModel;
    using System.Globalization;
    using System.Web.Configuration;

    /// <summary>
    /// The web.config app setting value resolver.
    /// </summary>
    public class AppSettingValueResolver : DittoValueResolver<DittoValueResolverContext, AppSettingAttribute>
    {
        /// <summary>
        /// Gets the value from the web.config app setting
        /// </summary>
        /// <returns>
        /// The <see cref="object"/> representing the raw value.
        /// </returns>
        public override object ResolveValue()
        {
            var appSettingKey = Attribute.AppSettingKey ?? (Context.PropertyDescriptor != null ? Context.PropertyDescriptor.Name : string.Empty);

            if (string.IsNullOrWhiteSpace(appSettingKey))
            {
                return null;
            }

            return WebConfigurationManager.AppSettings[appSettingKey];
        }
    }
}