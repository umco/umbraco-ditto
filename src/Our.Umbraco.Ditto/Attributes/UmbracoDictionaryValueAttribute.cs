namespace Our.Umbraco.Ditto
{
    using System;
    using System.Globalization;
    using System.Reflection;

    using global::Umbraco.Core.Models;
    using global::Umbraco.Web;

    /// <summary>
    /// The Umbraco dictionary value attribute.
    /// Used for providing Umbraco with additional information about a dictionary item to aid property value conversion.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class UmbracoDictionaryValueAttribute : DittoValueAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoDictionaryValueAttributeAttribute"/> class.
        /// </summary>
        /// <param name="dictionaryKey">
        /// The dictionary key.
        /// </param>
        public UmbracoDictionaryValueAttribute(string dictionaryKey)
        {
            this.DictionaryKey = dictionaryKey;
        }

        /// <summary>
        /// Gets or sets the dictionary key.
        /// </summary>
        public string DictionaryKey { get; set; }

        public override object GetValue(
            IPublishedContent content,
            Type type,
            CultureInfo culture,
            PropertyInfo propertyInfo)
        {
            if (string.IsNullOrWhiteSpace(this.DictionaryKey))
                return null;

            // HACK: [LK:2015-04-14] Resorting to using `UmbracoHelper`, as `CultureDictionaryFactoryResolver` isn't public in v6.2.x.
            return new UmbracoHelper(UmbracoContext.Current, content).GetDictionaryValue(this.DictionaryKey);
        }
    }
}