using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Convenient processor wrapper for resolving a type, based on a content item's Doc Type alias.
    /// </summary>
    public class DittoDocTypeFactoryAttribute : DittoFactoryAttribute
    {
        /// <summary>
        /// A prefix string to prepend onto the resolved type name.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// A suffix string to append onto the resolved type name.
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// Fetches the type name from the current content item's Doc Type alias.
        /// </summary>
        /// <param name="currentContent"></param>
        /// <returns></returns>
        public override string ResolveTypeName(IPublishedContent currentContent)
        {
            return string.Concat(Prefix, currentContent.DocumentTypeAlias, Suffix);
        }
    }
}