using System;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// An Umbraco dictionary.
    /// </summary>
    internal static class UmbracoDictionaryHelper
    {
        /// <summary>
        /// Gets the value from the Umbraco dictionary.
        /// </summary>
        internal static Func<string, string> GetValue = (key) => UmbracoContext.Current != null
            ? new UmbracoHelper(UmbracoContext.Current).GetDictionaryValue(key)
            : null;
    }

    /// <summary>
    /// The Umbraco dictionary value processor attribute.
    /// Used for providing Umbraco with additional information about a dictionary item to aid property value processing.
    /// </summary>
    [DittoProcessorMetaData(ValueType = typeof(IPublishedContent))]
    public class UmbracoDictionaryAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// Gets or sets the dictionary key.
        /// </summary>
        public string DictionaryKey { get; set; }

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
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override object ProcessValue()
        {
            var dictionaryKey = DictionaryKey
                ?? (Context.PropertyDescriptor != null ? Context.PropertyDescriptor.Name : string.Empty);

            if (string.IsNullOrWhiteSpace(dictionaryKey))
            {
                return null;
            }

            var content = this.Value as IPublishedContent;
            if (content == null)
            {
                return null;
            }

            return UmbracoDictionaryHelper.GetValue(dictionaryKey);
        }
    }
}