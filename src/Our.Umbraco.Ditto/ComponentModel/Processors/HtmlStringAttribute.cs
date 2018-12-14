using System.Web;
using Umbraco.Web.Templates;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Provides a unified way of converting objects to a <see cref="HtmlString"/>.
    /// </summary>
    public class HtmlStringAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        public override object ProcessValue()
        {
            if (typeof(IHtmlString).IsAssignableFrom(this.Context.PropertyInfo.PropertyType))
            {
                if (this.Value is string text && string.IsNullOrWhiteSpace(text) == false)
                {
                    text = text
                        .Replace("\r\n", "<br />")
                        .Replace("\n", "<br />")
                        .Replace("\r", "<br />");

                    return new HtmlString(text);
                }

                if (this.Value is HtmlString)
                {
                    var html = this.Value.ToString();

                    if (string.IsNullOrWhiteSpace(html) == false)
                    {
                        // TODO : html = TemplateUtilities.ParseInternalLinks(html);
                    }

                    return new HtmlString(html);
                }
            }

            return this.Value;
        }
    }
}