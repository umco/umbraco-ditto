using System.Web;
using Umbraco.Core.Dynamics;
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
            if (typeof(IHtmlString).IsAssignableFrom(Context.PropertyDescriptor.PropertyType))
            {
                if (Value.IsNullOrEmptyString())
                {
                    return null;
                }

                if (Value is string)
                {
                    var text = Value.ToString();

                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        text = text.Replace("\n", "<br/>\n");
                    }

                    return new HtmlString(text);
                }

                if (Value is HtmlString)
                {
                    var html = Value.ToString();

                    if (!string.IsNullOrWhiteSpace(html))
                    {
                        html = TemplateUtilities.ParseInternalLinks(html);
                    }

                    return new HtmlString(html);
                }

                if (Value is DynamicXml)
                {
                    return ((DynamicXml)Value).ToHtml();
                }
            }

            return Value;
        }
    }
}
