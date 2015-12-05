using System;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The Umbraco dictionary value processor attribute.
    /// Used for providing Umbraco with additional information about a dictionary item to aid property value processing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class UmbracoDictionaryProcessorAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// Gets or sets the dictionary key.
        /// </summary>
        public string DictionaryKey { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoDictionaryProcessorAttribute" /> class.
        /// </summary>
        /// <param name="dictionaryKey">The dictionary key.</param>
        public UmbracoDictionaryProcessorAttribute(string dictionaryKey)
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
            var dictionaryKey = DictionaryKey ?? (this.Context.PropertyDescriptor != null ? this.Context.PropertyDescriptor.Name : string.Empty);

            if (string.IsNullOrWhiteSpace(dictionaryKey))
            {
                return null;
            }

            var content = Value as IPublishedContent;
            if (content == null)
            {
                return null;
            }

            // HACK: [LK:2015-04-14] Resorting to using `UmbracoHelper`, as `CultureDictionaryFactoryResolver` isn't public in v6.2.x.
            return new UmbracoHelper(UmbracoContext.Current, content).GetDictionaryValue(dictionaryKey);
        }
    }
}