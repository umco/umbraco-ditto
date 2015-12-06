using System;
using System.Web.Configuration;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The AppSetting value attribute.
    /// Used for providing additional information about an AppSetting item to aid property value conversion.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AppSettingrAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// Gets or sets the app setting key.
        /// </summary>
        public string AppSettingKey { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingrAttribute" /> class.
        /// </summary>
        /// <param name="appSettingKey">The app setting key in the web.config</param>
        public AppSettingrAttribute(string appSettingKey)
        {
            this.AppSettingKey = appSettingKey;
        }

        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        public override object ProcessValue()
        {
            var appSettingKey = AppSettingKey ?? (this.Context.PropertyDescriptor != null ? this.Context.PropertyDescriptor.Name : string.Empty);

            if (string.IsNullOrWhiteSpace(appSettingKey))
            {
                return null;
            }

            return WebConfigurationManager.AppSettings[appSettingKey];
        }
    }
}