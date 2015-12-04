using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The Umbraco dictionary value processor.
    /// </summary>
    public class UmbracoDictionaryPocessor : DittoProcessor<IPublishedContent, DittoProcessorContext, UmbracoDictionaryProcessorAttribute>
    {
        /// <summary>
        /// Gets the raw value for the current value being processed.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/> representing the processed value.
        /// </returns>
        public override object ProcessValue()
        {
            var dictionaryKey = this.Attribute.DictionaryKey ?? (this.Context.PropertyDescriptor != null ? this.Context.PropertyDescriptor.Name : string.Empty);

            if (string.IsNullOrWhiteSpace(dictionaryKey))
            {
                return null;
            }

            // HACK: [LK:2015-04-14] Resorting to using `UmbracoHelper`, as `CultureDictionaryFactoryResolver` isn't public in v6.2.x.
            return new UmbracoHelper(UmbracoContext.Current, this.Value).GetDictionaryValue(dictionaryKey);
        }
    }
}