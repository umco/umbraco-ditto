namespace Our.Umbraco.Ditto
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Collections;
    using System.Web;

    using global::Umbraco.Core.Models;

    /// <summary>
    /// The Ditto value resolver provides reusable methods for returning property values from Umbraco.
    /// </summary>
    public abstract class DittoValueResolver
    {
        /// <summary>
        /// Gets or sets the IPublishedContent object.
        /// </summary>
        public IPublishedContent Content { get; protected set; }

        /// <summary>
        /// Gets or sets the resovler context.
        /// </summary>
        public DittoValueResolverContext Context { get; protected set; }

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
        /// An <see cref="DittoValueResolverContext" /> that provides a context.
        /// </param>
        /// <param name="attribute">
        /// The <see cref="DittoValueResolverAttribute"/> containing additional information 
        /// indicating how to resolve the property.
        /// </param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <returns>
        /// The <see cref="object"/> representing the raw value.
        /// </returns>
        internal virtual object ResolveValue(DittoValueResolverContext context, DittoValueResolverAttribute attribute,
            CultureInfo culture)
        {
            Content = context.Instance as IPublishedContent;
            Context = context;
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
    /// A generic value resolver to resolve a raw value.
    /// </summary>
    /// <typeparam name="TContextType">
    /// The <see cref="T:System.Type"/> of context object that provides dynamic context to this resovler. 
    /// </typeparam>
    /// <typeparam name="TAttributeType">
    /// The <see cref="T:System.Type"/> of attribute that provides static config for this resolver. 
    /// </typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public abstract class DittoValueResolver<TContextType, TAttributeType> : DittoValueResolver
        where TContextType : DittoValueResolverContext
        where TAttributeType : DittoValueResolverAttribute
    {
        /// <summary>
        /// Gets or sets the resovler context.
        /// </summary>
        public new TContextType Context { get; protected set; }

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
        internal override object ResolveValue(DittoValueResolverContext context, 
            DittoValueResolverAttribute attribute, CultureInfo culture)
        {
            if (!(context is TContextType))
            {
                throw new ArgumentException(
                    "The resolver context must be of type " + typeof(TContextType).AssemblyQualifiedName,
                    "context");
            }

            if (!(attribute is TAttributeType))
            {
                throw new ArgumentException(
                    "The resolver attribute must be of type " + typeof(TAttributeType).AssemblyQualifiedName,
                    "attribute");
            }

            Content = context.Instance as IPublishedContent;
            Context = context as TContextType;
            Attribute = attribute as TAttributeType;
            Culture = culture;

            return ResolveValue();
        }
    }

    /// <summary>
    /// A generic value resolver to resolve a raw value.
    /// </summary>
    /// <typeparam name="TContextType">
    /// The <see cref="T:System.Type"/> of context object that provides dynamic context to this resovler. 
    /// </typeparam>
    public abstract class DittoValueResolver<TContextType> :
        DittoValueResolver<TContextType, DittoValueResolverAttribute>
        where TContextType : DittoValueResolverContext
    { }
}