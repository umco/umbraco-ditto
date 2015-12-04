using System.Web.Configuration;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The web.config app setting value processor.
    /// </summary>
    public class AppSettingProcessor : DittoProcessor<object, DittoProcessorContext, AppSettingProcessorAttribute>
    {
        /// <summary>
        /// Gets the value from the web.config app setting
        /// </summary>
        /// <returns>
        /// The <see cref="object"/> representing the raw value.
        /// </returns>
        public override object ProcessValue() 
        {
            var appSettingKey = this.Attribute.AppSettingKey ?? (this.Context.PropertyDescriptor != null ? this.Context.PropertyDescriptor.Name : string.Empty);

            if (string.IsNullOrWhiteSpace(appSettingKey))
            {
                return null;
            }

            return WebConfigurationManager.AppSettings[appSettingKey];
        }
    }
}