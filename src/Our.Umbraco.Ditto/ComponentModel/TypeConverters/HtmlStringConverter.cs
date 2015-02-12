namespace Our.Umbraco.Ditto
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Web;

    using global::Umbraco.Web.Templates;

    /// <summary>
    /// Provides a unified way of converting <see cref="String"/>s to <see cref="HtmlString"/>'s.
    /// </summary>
    public class HtmlStringConverter : TypeConverter
    {
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter,
        /// using the specified context.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="sourceType">
        /// A <see cref="T:System.Type" /> that represents the type you want to convert from.
        /// </param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string) || sourceType == typeof(HtmlString))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted value.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                return null;
            }

            var s = value.ToString();
            if (value is HtmlString)
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    s = TemplateUtilities.ParseInternalLinks(s);
                }

                return new HtmlString(s);
            }

            // This lets us convert textbox multiple data type properties to HtmlString's 
            if (value is string)
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    var umbracoHelper = ConverterHelper.UmbracoHelper;
                    s = umbracoHelper.ReplaceLineBreaksForHtml(s);
                }

                return new HtmlString(s);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}