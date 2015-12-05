using System;
using System.Globalization;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.Security;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public abstract class DittoProcessorAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public object Value { get; internal set; }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public DittoProcessorContext Context { get; internal set; }

        /// <summary>
        /// Gets the type of the context.
        /// </summary>
        /// <value>
        /// The type of the context.
        /// </value>
        public virtual Type ContextType { get { return typeof(DittoProcessorContext); } }

        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        /// <value>
        /// The culture.
        /// </value>
        public CultureInfo Culture { get; internal set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        public int Order { get; set; }

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
        /// <param name="value">The value.</param>
        /// <param name="context">The context.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        internal virtual object ProcessValue(
            object value,
            DittoProcessorContext context,
            CultureInfo culture)
        {
            Value = value;
            Context = context;
            Culture = culture;

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

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as DittoProcessorAttribute;
            return (other != null)
                && other.GetType().GetFullNameWithAssembly() == this.GetType().GetFullNameWithAssembly();
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.GetType().GetFullNameWithAssembly().GetHashCode();
        }
    }

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <typeparam name="TValueType">The type of the value type.</typeparam>
    //[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    //public abstract class DittoProcessorAttribute<TValueType> : DittoProcessorAttribute
    //{
    //    /// <summary>
    //    /// Gets or sets the value.
    //    /// </summary>
    //    /// <value>
    //    /// The value.
    //    /// </value>
    //    public new TValueType Value
    //    {
    //        get
    //        {
    //            //TODO: Check value first?
    //            return (TValueType)base.Value;
    //        }
    //        protected set
    //        {
    //            base.Value = value;
    //        }
    //    }
    //}

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <typeparam name="TValueType">The type of the value type.</typeparam>
    ///// <typeparam name="TContextType">The type of the context type.</typeparam>
    //[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    //public abstract class DittoProcessorAttribute<TValueType, TContextType> : DittoProcessorAttribute<TValueType>
    //    where TContextType : DittoProcessorContext
    //{
    //    /// <summary>
    //    /// Gets or sets the context.
    //    /// </summary>
    //    /// <value>
    //    /// The context.
    //    /// </value>
    //    public new TContextType Context
    //    {
    //        get
    //        {
    //            //TODO: Check value first?
    //            return (TContextType)base.Context;
    //        }
    //        protected set
    //        {
    //            base.Context = value;
    //        }
    //    }
    //}
}