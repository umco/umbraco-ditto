using System;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The AppSetting value attribute.
    /// Used for providing additional information about an AppSetting item to aid property value conversion.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AppSettingProcessorAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingProcessorAttribute"/> class.
        /// </summary>
        public AppSettingProcessorAttribute()
            : base(typeof(AppSettingProcessor))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingProcessorAttribute" /> class.
        /// </summary>
        /// <param name="appSettingKey">The app setting key in the web.config</param>
        public AppSettingProcessorAttribute(string appSettingKey)
            : this()
        {
            this.AppSettingKey = appSettingKey;
        }

        /// <summary>
        /// Gets or sets the app setting key.
        /// </summary>
        public string AppSettingKey { get; set; }
    }
}