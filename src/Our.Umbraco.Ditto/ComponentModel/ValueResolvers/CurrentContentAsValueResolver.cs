namespace Our.Umbraco.Ditto
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    using global::Umbraco.Core.Models;

    /// <summary>
    /// The current content value resolver.
    /// </summary>
    public class CurrentContentAsValueResolver : DittoValueResolver<CurrentContentAsAttribute>
    {
        /// <summary>
        /// Resolves the value.
        /// Gets the current <see cref="IPublishedContent"/> from Umbraco.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="attribute">
        /// The <see cref="CurrentContentAsAttribute"/> containing additional information 
        /// indicating how to resolve the property.
        /// </param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <returns>
        /// The <see cref="object"/> representing the raw value.
        /// </returns>
        public override object ResolveValue(ITypeDescriptorContext context, CurrentContentAsAttribute attribute, CultureInfo culture)
        {
            // NOTE: [LK] In order to prevent an infinite loop / stack-overflow, we check if the
            // property's type matches the containing model's type, then we throw an exception.
            if (context.PropertyDescriptor.PropertyType == context.PropertyDescriptor.ComponentType)
            {
                throw new InvalidOperationException(
                    string.Format("Unable to process property type '{0}', it is the same as the containing model type.",
                    context.PropertyDescriptor.PropertyType.Name));
            }

            return ((IPublishedContent)context.Instance).As(context.PropertyDescriptor.PropertyType);
        }
    }
}