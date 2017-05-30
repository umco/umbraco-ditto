using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// A processor that queries the Umbraco content cache using an XPath expression.
    /// </summary>
    public class UmbracoXPathAttribute : UmbracoXPathProcessorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoXPathAttribute"/> class.
        /// </summary>
        public UmbracoXPathAttribute()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoXPathAttribute"/> class.
        /// </summary>
        /// <param name="xpath">The XPath expression used to query the Umbraco content cache.</param>
        public UmbracoXPathAttribute(string xpath)
            : this()
        {
            XPath = xpath;
        }

        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// Returns the objects that match the XPath expression from the Umbraco content cache.
        /// </returns>
        public override object ProcessValue()
        {
            var xpath = GetXPath();

            if (string.IsNullOrWhiteSpace(xpath))
                return Enumerable.Empty<IPublishedContent>();

            return UmbracoContext
                .Current
                .ContentCache
                .GetByXPath(xpath);
        }
    }
}