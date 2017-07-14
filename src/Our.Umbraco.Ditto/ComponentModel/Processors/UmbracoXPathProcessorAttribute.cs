using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// An abstract processor that parses an XPath expression.
    /// </summary>
    public abstract class UmbracoXPathProcessorAttribute : DittoProcessorAttribute
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
        /// Initializes a new instance of the <see cref="UmbracoXPathProcessorAttribute"/> class.
        /// </summary>
        public UmbracoXPathProcessorAttribute()
        {
            // This lookup attempts to simplify how Umbraco core handles the XPath syntax parsing
            // ref: https://github.com/umbraco/Umbraco-CMS/blob/dev-v7/src/Umbraco.Core/Xml/UmbracoXPathPathSyntaxParser.cs

            Lookup = new Dictionary<string, Func<IPublishedContent, string>>()
            {
                { "$current", x => string.Format("id({0})", x.Id) },
                { "$parent", x => string.Format("id({0})", x.Path.ToDelimitedList().Reverse().ElementAtOrDefault(1) ?? x.Parent.Id.ToString()) },
                { "$site", x => string.Format("id({0})", x.Path.ToDelimitedList().ElementAtOrDefault(1) ?? "-1") },
                { "$root", x => "/root" },
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoXPathProcessorAttribute"/> class.
        /// </summary>
        /// <param name="xpath">The XPath expression used to query the Umbraco content cache.</param>
        public UmbracoXPathProcessorAttribute(string xpath)
            : this()
        {
            XPath = xpath;
        }

        /// <summary>
        /// Attempts to get the current IPublishedContent object from the value.
        /// </summary>
        /// <returns>Returns an IPublishedContent object.</returns>
        protected IPublishedContent GetPublishedContent()
        {
            var content = Context.Content;

            if (Value is IPublishedContent)
            {
                content = (IPublishedContent)Value;
            }
            else if (Value is IEnumerable<IPublishedContent>)
            {
                content = ((IEnumerable<IPublishedContent>)Value).FirstOrDefault();
            }

            if ((content == null || content.Id == 0) && UmbracoContext.Current.PageId.HasValue)
                content = UmbracoContext.Current.ContentCache.GetById(UmbracoContext.Current.PageId.Value);

            return content;
        }

        /// <summary>
        /// Gets the XPath expression, parsing the look-up placeholder tokens.
        /// </summary>
        /// <returns></returns>
        protected string GetXPath()
        {
            if (string.IsNullOrWhiteSpace(XPath))
                return string.Empty;

            var content = GetPublishedContent();

            var xparam = Lookup.FirstOrDefault(x => XPath.StartsWith(x.Key));

            return xparam.Key != null
                ? XPath.Replace(xparam.Key, xparam.Value(content))
                : XPath;
        }
    }
}