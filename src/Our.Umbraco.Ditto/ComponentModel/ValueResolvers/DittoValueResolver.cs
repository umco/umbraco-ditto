namespace Our.Umbraco.Ditto
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    /// <summary>
    /// The Ditto value resolver provides reusable methods for returning property values from Umbraco.
    /// </summary>
    public abstract class DittoValueResolver
    {
        /// <summary>
        /// Gets the raw value for the current property from Umbraco.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="attribute">
        /// The <see cref="DittoValueResolverAttribute"/> containing additional information 
        /// indicating how to resolve the property.
        /// </param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <returns>
        /// The <see cref="object"/> representing the raw value.
        /// </returns>
        public abstract object ResolveValue(ITypeDescriptorContext context, DittoValueResolverAttribute attribute, CultureInfo culture);
    }

    /// <summary>
    /// A generic value resolver that accepts an implementation of <see cref="DittoValueResolverAttribute"/>
    /// in order to resolve the raw value from Umbraco.
    /// </summary>
    /// <typeparam name="TAttributeType">
    /// The <see cref="T:System.Type"/> of attribute that will resolve the raw value. 
    /// </typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public abstract class DittoValueResolver<TAttributeType> : DittoValueResolver
        where TAttributeType : DittoValueResolverAttribute
    {
        /// <summary>
        /// Gets the raw value for the current property from Umbraco.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="attribute">
        /// The <see cref="DittoValueResolverAttribute"/> containing additional information 
        /// indicating how to resolve the property.
        /// </param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <returns>
        /// The <see cref="object"/> representing the raw value.
        /// </returns>
        public override object ResolveValue(ITypeDescriptorContext context, DittoValueResolverAttribute attribute, CultureInfo culture)
        {
            if (!(attribute is TAttributeType))
            {
                throw new ArgumentException(
                    "The resolver attribute must be of type " + typeof(TAttributeType).AssemblyQualifiedName,
                    "attribute");
            }

            return this.ResolveValue(context, (TAttributeType)attribute, culture);
        }

        /// <summary>
        /// Gets the raw value for the current property from Umbraco.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="attribute">
        /// The <see cref="TAttributeType"/> containing additional information 
        /// indicating how to resolve the property.
        /// </param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <returns>
        /// The <see cref="object"/> representing the raw value.
        /// </returns>
        public abstract object ResolveValue(ITypeDescriptorContext context, TAttributeType attribute, CultureInfo culture);
    }
}