using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The Umbraco dictionary value processor attribute.
    /// Used for providing Umbraco with additional information about a dictionary item to aid property value processing.
    /// </summary>
    [DittoProcessorMetaData(ValueType = typeof(IPublishedContent))]
    public class UmbracoDictionaryAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoDictionaryAttribute" /> class.
        /// </summary>
        public UmbracoDictionaryAttribute()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoDictionaryAttribute" /> class.
        /// </summary>
        /// <param name="dictionaryKey">The dictionary key.</param>
        public UmbracoDictionaryAttribute(string dictionaryKey)
        {
            this.DictionaryKey = dictionaryKey;
        }

        /// <summary>
        /// Gets or sets the dictionary key.
        /// </summary>
        public string DictionaryKey { get; set; }

        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        public override object ProcessValue()
        {
            var dictionaryKey = this.DictionaryKey
                ?? (this.Context.PropertyInfo != null ? this.Context.PropertyInfo.Name : string.Empty);

            if (string.IsNullOrWhiteSpace(dictionaryKey))
            {
                return null;
            }

            var content = this.Value as IPublishedContent;
            if (content == null)
            {
                return null;
            }

            return Umbraco.GetDictionaryValue(dictionaryKey);
        }
    }
}