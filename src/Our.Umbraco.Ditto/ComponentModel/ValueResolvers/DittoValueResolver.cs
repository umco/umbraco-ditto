using System.Collections;
using System.Collections.Generic;
using System.Web;
using Umbraco.Core;
using Umbraco.Web.Media.EmbedProviders.Settings;

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
        private const string CONTEXT_KEY_FORMAT = "Ditto_ValueResolverContext_{0}";

        private static IDictionary _contextCache;
        private static IDictionary ContextCache
        {
            get
            {
                if (_contextCache != null)
                    return _contextCache;

                return HttpContext.Current != null
                    ? HttpContext.Current.Items
                    : new Dictionary<string, object>();
            }
        }

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
        public abstract object ResolveValue(DittoValueResolverContext context, 
            DittoValueResolverAttribute attribute, 
            CultureInfo culture);

        /// <summary>
        /// Registers a value resolver context for the current request.
        /// </summary>
        /// <param name="ctx">
        /// The <see cref="DittoValueResolverContext" /> to register.
        /// </param>
        internal static void RegisterContext<TContextType>(TContextType ctx)
            where TContextType : DittoValueResolverContext
        {
            var key = string.Format(CONTEXT_KEY_FORMAT, typeof(TContextType).FullName);

            if (ContextCache.Contains(key))
            {
                ContextCache[key] = ctx;
            }
            else
            {
                ContextCache.Add(key, ctx);
            }
        }

        /// <summary>
        /// Retreieves a value resolver context of the given type if one exists.
        /// </summary>
        /// <param name="contextType">
        /// The <see cref="DittoValueResolverContext" /> type to lookup.
        /// </param>
        /// <returns>
        /// The registered <see cref="DittoValueResolverContext"/> instance.
        /// </returns>
        internal static DittoValueResolverContext GetRegistedContext(Type contextType)
        {
            var key = string.Format(CONTEXT_KEY_FORMAT, contextType.FullName);

            return ContextCache.Contains(key)
                ? (DittoValueResolverContext)ContextCache[key]
                : null;
        }

        /// <summary>
        /// Helper function to allow setting the context cache container manually, used for testing.
        /// </summary>
        internal static void SetContextCache(IDictionary cache)
        {
            _contextCache = cache;
        }
    }

    /// <summary>
    /// A generic value resolver that accepts an implementation of <see cref="DittoValueResolverAttribute"/>
    /// in order to resolve the raw value from Umbraco.
    /// </summary>
    /// <typeparam name="TAttributeType">
    /// The <see cref="T:System.Type"/> of attribute that will resolve the raw value. 
    /// </typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public abstract class DittoValueResolver<TContextType, TAttributeType> : DittoValueResolver
        where TContextType : DittoValueResolverContext
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
        public override object ResolveValue(DittoValueResolverContext context, 
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

            return ResolveValue((TContextType)context, (TAttributeType)attribute, culture);
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
        public abstract object ResolveValue(TContextType context, TAttributeType attribute, CultureInfo culture);
    }

    public abstract class DittoValueResolver<TContextType> :
        DittoValueResolver<TContextType, DittoValueResolverAttribute>
        where TContextType : DittoValueResolverContext
    { }
}