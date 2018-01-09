using System.Linq;
using System.Xml.XPath;
using Umbraco.Web;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// A processor that queries the Umbraco content cache using the XPathNavigator.
    /// </summary>
    public abstract class UmbracoXPathNavigatorAttribute : UmbracoXPathProcessorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoXPathNavigatorAttribute"/> class.
        /// </summary>
        public UmbracoXPathNavigatorAttribute()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoXPathNavigatorAttribute"/> class.
        /// </summary>
        /// <param name="xpath"></param>
        public UmbracoXPathNavigatorAttribute(string xpath)
            : this()
        {
            XPath = xpath;
        }

        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// Returns a collection of XPathNavigator objects that match the XPath expression from the Umbraco content cache.
        /// </returns>
        public override object ProcessValue()
        {
            var xpath = GetXPath();

            if (string.IsNullOrWhiteSpace(xpath))
                return Enumerable.Empty<XPathNavigator>();

            return UmbracoContext
                .ContentCache
                .GetXPathNavigator()
                .Select(xpath)
                .Cast<XPathNavigator>();
        }
    }
}