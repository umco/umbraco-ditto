using System;
using System.Web.Configuration;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The AppSetting value attribute.
    /// Used for providing additional information about an AppSetting item to aid property value conversion.
    /// </summary>
    public class AppSettingAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingAttribute" /> class.
        /// </summary>
        /// <param name="appSettingKey">The app setting key in the web.config</param>
        public AppSettingAttribute(string appSettingKey)
        {
            this.AppSettingKey = appSettingKey;
        }

        /// <summary>
        /// Gets or sets the app setting key.
        /// </summary>
        public string AppSettingKey { get; protected set; }

        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        public override object ProcessValue()
        {
            var appSettingKey = this.AppSettingKey ?? this.Context.PropertyInfo?.Name ?? string.Empty;

            if (string.IsNullOrWhiteSpace(appSettingKey))
            {
                return null;
            }

            return WebConfigurationManager.AppSettings[appSettingKey];
        }
    }
}