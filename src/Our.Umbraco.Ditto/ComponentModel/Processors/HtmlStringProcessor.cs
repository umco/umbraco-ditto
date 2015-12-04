using System.Web;
using Umbraco.Core.Dynamics;
using Umbraco.Web.Templates;

namespace Our.Umbraco.Ditto.ComponentModel.Processors
{
    public class HtmlStringProcessor : DittoProcessor<object>
    {
        public override object ProcessValue()
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

            return Value;
        }
    }
}
