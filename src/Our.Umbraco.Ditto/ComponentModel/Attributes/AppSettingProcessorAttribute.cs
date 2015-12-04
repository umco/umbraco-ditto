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
        /// Initializes a new instance of the <see cref="AppSettingProcessorAttribute" /> class.
        /// </summary>
        public AppSettingProcessorAttribute()
            : this(0)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingProcessorAttribute"/> class.
        /// </summary>
        /// <param name="order">The order.</param>
        public AppSettingProcessorAttribute(int order)
            : base(order, typeof(AppSettingProcessor))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingProcessorAttribute"/> class.
        /// </summary>
        /// <param name="appSettingKey">The application setting key.</param>
        public AppSettingProcessorAttribute(string appSettingKey)
            : this(0, appSettingKey)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingProcessorAttribute" /> class.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="appSettingKey">The app setting key in the web.config</param>
        public AppSettingProcessorAttribute(int order, 
            string appSettingKey)
            : this(order)
        {
            this.AppSettingKey = appSettingKey;
        }

        /// <summary>
        /// Gets or sets the app setting key.
        /// </summary>
        public string AppSettingKey { get; set; }
    }
}