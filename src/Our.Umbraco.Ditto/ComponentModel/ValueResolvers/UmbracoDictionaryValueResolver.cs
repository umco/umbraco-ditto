namespace Our.Umbraco.Ditto
{
    using System.ComponentModel;
    using System.Globalization;

    using global::Umbraco.Core.Models;
    using global::Umbraco.Web;

    /// <summary>
    /// The Umbraco dictionary value resolver.
    /// </summary>
    public class UmbracoDictionaryValueResolver : DittoValueResolver<UmbracoDictionaryAttribute>
    {
        /// <summary>
        /// Gets the raw value for the current property from Umbraco.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="attribute">
        /// The <see cref="UmbracoDictionaryAttribute"/> containing additional information 
        /// indicating how to resolve the property.
        /// </param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <returns>
        /// The <see cref="object"/> representing the raw value.
        /// </returns>
        public override object ResolveValue(ITypeDescriptorContext context, UmbracoDictionaryAttribute attribute, CultureInfo culture)
        {
            var dictionaryKey = attribute.DictionaryKey ?? (context.PropertyDescriptor != null ? context.PropertyDescriptor.Name : string.Empty);

            if (string.IsNullOrWhiteSpace(dictionaryKey))
            {
                return null;
            }

            var content = context.Instance as IPublishedContent;

            // HACK: [LK:2015-04-14] Resorting to using `UmbracoHelper`, as `CultureDictionaryFactoryResolver` isn't public in v6.2.x.
            return new UmbracoHelper(UmbracoContext.Current, content).GetDictionaryValue(dictionaryKey);
        }
    }
}