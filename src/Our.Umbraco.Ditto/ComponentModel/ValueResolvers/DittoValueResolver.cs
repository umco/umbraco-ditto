using Umbraco.Core.Models;
using Umbraco.Web.PublishedCache;

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
        /// Gets or sets the resovler context.
        /// </summary>
        public ITypeDescriptorContext Context { get; protected set; }

        /// <summary>
        /// Gets or sets the IPublishedContent object.
        /// </summary>
        public IPublishedContent Content { get; protected set; }

        /// <summary>
        /// Gets or sets the associated attribute.
        /// </summary>
        public DittoValueResolverAttribute Attribute { get; protected set; }

        /// <summary>
        /// Gets or sets the culture object.
        /// </summary>
        public CultureInfo Culture { get; protected set; } 

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
        public virtual object ResolveValue(ITypeDescriptorContext context, DittoValueResolverAttribute attribute,
            CultureInfo culture)
        {
            Context = context;
            Content = context.Instance as IPublishedContent;
            Attribute = attribute;
            Culture = culture;

            return ResolveValue();
        }

        /// <summary>
        /// Performs the value resolution.
        /// </summary>
        /// /// <returns>
        /// The <see cref="object"/> representing the raw value.
        /// </returns>
        public abstract object ResolveValue();
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
        /// Gets or sets the associated attribute.
        /// </summary>
        public new TAttributeType Attribute { get; protected set; }

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

            Context = context;
            Content = context.Instance as IPublishedContent;
            Attribute = attribute as TAttributeType;
            Culture = culture;

            return ResolveValue();
        }
    }
}