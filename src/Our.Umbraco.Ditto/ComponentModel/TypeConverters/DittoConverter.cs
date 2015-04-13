// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DittoConverter.cs" company="Umbrella Inc, Our Umbraco and other contributors">
//   Copyright Umbrella Inc, Our Umbraco and other contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Our.Umbraco.Ditto
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Web;
    using global::Umbraco.Web.Security;

    /// <summary>
    /// The Ditto type converter containing reusable properties and methods for converting <see cref="IPublishedContent"/> instances.
    /// All other Ditto converters should inherit from this class.
    /// </summary>
    public abstract class DittoConverter : TypeConverter
    {
        /// <summary>
        /// Takes a content node ID, gets the corresponding <see cref="T:Umbraco.Core.Models.IPublishedContent"/> object,
        /// then converts the object to the desired type.
        /// </summary>
        /// <param name="id">
        /// The content node ID.
        /// </param>
        /// <param name="targetType">
        /// The property <see cref="Type"/> to convert to.
        /// </param>
        /// <param name="culture">
        /// The <see cref="CultureInfo"/> to use as the current culture.
        /// </param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        protected virtual object ConvertContentFromInt(int id, Type targetType, CultureInfo culture)
        {
            if (id <= 0)
            {
                return null;
            }

            return UmbracoContext.Current.ContentCache.GetById(id).As(targetType, null, null, culture);
        }

        /// <summary>
        /// Takes a media node ID, gets the corresponding <see cref="T:Umbraco.Core.Models.IPublishedContent"/> object,
        /// then converts the object to the desired type.
        /// </summary>
        /// <param name="id">
        /// The media node ID.
        /// </param>
        /// <param name="targetType">
        /// The property <see cref="Type"/> to convert to.
        /// </param>
        /// <param name="culture">
        /// The <see cref="CultureInfo"/> to use as the current culture.
        /// </param>
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
                return media.As(targetType, null, null, culture);
            }

            // It's most likely a folder, try its children.
            // This returns an IEnumerable<T>
            return media.Children().As(targetType, null, null, null, culture);
        }

        /// <summary>
        /// Takes a member node ID, gets the corresponding <see cref="T:Umbraco.Core.Models.IPublishedContent"/> object,
        /// then converts the object to the desired type.
        /// </summary>
        /// <param name="id">
        /// The member node ID.
        /// </param>
        /// <param name="targetType">
        /// The property <see cref="Type"/> to convert to.
        /// </param>
        /// <param name="culture">
        /// The <see cref="CultureInfo"/> to use as the current culture.
        /// </param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        protected virtual object ConvertMemberFromInt(int id, Type targetType, CultureInfo culture)
        {
            if (id <= 0)
            {
                return null;
            }

            return new MembershipHelper(UmbracoContext.Current).GetById(id).As(targetType, null, null, culture);
        }
    }
}
