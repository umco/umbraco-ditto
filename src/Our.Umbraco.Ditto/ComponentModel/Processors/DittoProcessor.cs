using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.Security;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Represents the basline implementation of a Ditto processor, a block of runnable code that can be executed as part of a property conversion.
    /// </summary>
    public abstract class DittoProcessor
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object Value { get; protected set; }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public DittoProcessorContext Context { get; protected set; }

        /// <summary>
        /// Gets or sets the attribute.
        /// </summary>
        /// <value>
        /// The attribute.
        /// </value>
        public DittoProcessorAttribute Attribute { get; protected set; }

        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        /// <value>
        /// The culture.
        /// </value>
        public CultureInfo Culture { get; protected set; }

        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        public abstract object ProcessValue();

        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        internal virtual object ProcessValue(
            DittoProcessorContext context,
            DittoProcessorAttribute attribute,
            CultureInfo culture)
        {
            Context = context;
            Attribute = attribute;
            Culture = culture;

            Value = context.Value;

            return this.ProcessValue();
        }

        /// <summary>
        /// Takes a content node ID, gets the corresponding <see cref="T:Umbraco.Core.Models.IPublishedContent"/> object,
        /// then converts the object to the desired type.
        /// </summary>
        /// <param name="id">The content node ID.</param>
        /// <param name="targetType">The property <see cref="Type"/> to convert to.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> to use as the current culture.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        protected virtual object ConvertContentFromInt(int id, Type targetType, CultureInfo culture)
        {
            if (id <= 0)
            {
                return null;
            }

            return UmbracoContext.Current.ContentCache.GetById(id).As(targetType, culture);
        }

        /// <summary>
        /// Takes a media node ID, gets the corresponding <see cref="T:Umbraco.Core.Models.IPublishedContent"/> object,
        /// then converts the object to the desired type.
        /// </summary>
        /// <param name="id">The media node ID.</param>
        /// <param name="targetType">The property <see cref="Type"/> to convert to. </param>
        /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        protected virtual object ConvertMediaFromInt(int id, Type targetType, CultureInfo culture)
        {
            if (id <= 0)
            {
                return null;
            }

            var media = UmbracoContext.Current.MediaCache.GetById(id);

            // Ensure we are actually returning a media file.
            if (media.HasProperty(Constants.Conventions.Media.File))
            {
                return media.As(targetType, culture);
            }

            // It's most likely a folder, try its children.
            // This returns an IEnumerable<T>
            return media.Children().As(targetType, culture);
        }

        /// <summary>
        /// Takes a member node ID, gets the corresponding <see cref="T:Umbraco.Core.Models.IPublishedContent"/> object,
        /// then converts the object to the desired type.
        /// </summary>
        /// <param name="id">The media node ID.</param>
        /// <param name="targetType">The property <see cref="Type"/> to convert to. </param>
        /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        protected virtual object ConvertMemberFromInt(int id, Type targetType, CultureInfo culture)
        {
            if (id <= 0)
            {
                return null;
            }

            return new MembershipHelper(UmbracoContext.Current).GetById(id).As(targetType, culture);
        }

    }

    /// <summary>
    /// Represents the basline implementation of a Ditto processor, a block of runnable code that can be executed as part of a property conversion.
    /// </summary>
    /// <typeparam name="TValueType">The type of the value.</typeparam>
    public abstract class DittoProcessor<TValueType> : DittoProcessor
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public new TValueType Value 
        {
            get
            {
                return (TValueType)base.Value;
            }
            protected set
            {
                base.Value = value;
            }
        }
    }

    /// <summary>
    /// Represents the basline implementation of a Ditto processor, a block of runnable code that can be executed as part of a property conversion.
    /// </summary>
    /// <typeparam name="TValueType">The type of the value type.</typeparam>
    /// <typeparam name="TContextType">The type of the context type.</typeparam>
    public abstract class DittoProcessor<TValueType, TContextType> :
        DittoProcessor<TValueType, TContextType, DittoProcessorAttribute>
        where TContextType : DittoProcessorContext
    { }

    /// <summary>
    /// Represents the basline implementation of a Ditto processor, a block of runnable code that can be executed as part of a property conversion.
    /// </summary>
    /// <typeparam name="TValueType">The type of the value type.</typeparam>
    /// <typeparam name="TContextType">The type of the context type.</typeparam>
    /// <typeparam name="TAttributeType">The type of the attribute type.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public abstract class DittoProcessor<TValueType, TContextType, TAttributeType> : DittoProcessor<TValueType>
        where TContextType : DittoProcessorContext
        where TAttributeType : DittoProcessorAttribute
    {
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public new TContextType Context
        {
            get
            {
                return (TContextType)base.Context;
            }
            protected set
            {
                base.Context = value;
            }
        }

        /// <summary>
        /// Gets or sets the attribute.
        /// </summary>
        /// <value>
        /// The attribute.
        /// </value>
        public new TAttributeType Attribute
        {
            get
            {
                return (TAttributeType)base.Attribute;
            }
            protected set
            {
                base.Attribute = value;
            }
        }
    }
}