using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// A processor that queries the Umbraco content cache using an XPath expression.
    /// </summary>
    public class UmbracoXPathAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// A dictionary lookup of XPath variable names, along with corresponding functions.
        /// </summary>
        protected Dictionary<string, Func<IPublishedContent, string>> Lookup { get; set; }

        /// <summary>
        /// The XPath expression used to query the Umbraco content cache.
        /// </summary>
        public string XPath { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoXPathAttribute"/> class.
        /// </summary>
        public UmbracoXPathAttribute()
        {
            // This lookup attempts to simplify how Umbraco core handles the XPath syntax parsing
            // ref: https://github.com/umbraco/Umbraco-CMS/blob/dev-v7/src/Umbraco.Core/Xml/UmbracoXPathPathSyntaxParser.cs

            Lookup = new Dictionary<string, Func<IPublishedContent, string>>()
            {
                { "$current", x => string.Format("id({0})", x.Id) },
                { "$parent", x => string.Format("id({0})", x.Parent.Id) },
                { "$site", x => string.Format("id({0})", x.Path.ToDelimitedList().ElementAtOrDefault(1) ?? "-1") },
                { "$root", x => "/root" },
            };
        }

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
            if (string.IsNullOrWhiteSpace(XPath))
                return Value;

            var content = GetPublishedContent(Value);

            if ((content == null || content.Id == 0) && UmbracoContext.Current.PageId.HasValue)
                content = UmbracoContext.Current.ContentCache.GetById(UmbracoContext.Current.PageId.Value);

            var xparam = Lookup.FirstOrDefault(x => XPath.StartsWith(x.Key));

            var xpath = xparam.Key != null
                ? XPath.Replace(xparam.Key, xparam.Value(content))
                : XPath;

            return UmbracoContext
                .Current
                .ContentCache
                .GetByXPath(xpath)
                .OrderBy(x => x.Level)
                .ThenBy(x => x.SortOrder);
        }

        /// <summary>
        /// Attempts to get the current IPublishedContent object from the value.
        /// </summary>
        /// <param name="value">The processor's input value.</param>
        /// <returns>Returns an IPublishedContent object.</returns>
        private IPublishedContent GetPublishedContent(object value)
        {
            if (value is IEnumerable<IPublishedContent>)
                return ((IEnumerable<IPublishedContent>)value).FirstOrDefault();

            if (value is IPublishedContent)
                return (IPublishedContent)value;

            return this.Context.Content;
        }
    }
}