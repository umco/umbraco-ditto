namespace Our.Umbraco.Ditto
{
    using System.ComponentModel;
    using System.Globalization;

    using global::Umbraco.Core.Models;

    /// <summary>
    /// The current content value resolver.
    /// </summary>
    public class CurrentContentValueResolver : DittoValueResolver<CurrentContentAttribute>
    {
        /// <summary>
        /// Resolves the value.
        /// Gets the current <see cref="IPublishedContent"/> from Umbraco.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="attribute">
        /// The <see cref="CurrentContentAttribute"/> containing additional information 
        /// indicating how to resolve the property.
        /// </param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <returns>
        /// The <see cref="object"/> representing the raw value.
        /// </returns>
        public override object ResolveValue(ITypeDescriptorContext context, CurrentContentAttribute attribute, CultureInfo culture)
        {
            return context.Instance as IPublishedContent;
        }
    }
}