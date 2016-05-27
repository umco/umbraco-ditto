using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Convenient processor wrapper for resolving a type, based on a content item's Doc Type alias.
    /// </summary>
    public class DittoDocTypeFactoryAttribute : DittoFactoryAttribute
    {
        /// <summary>
        /// Gets or sets a prefix string to prepend onto the resolved type name.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets a  suffix string to append onto the resolved type name.
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// Fetches the type name from the current content item's Doc Type alias.
        /// </summary>
        /// <param name="currentContent">The current published content.</param>
        /// <returns>The name.</returns>
        public override string ResolveTypeName(IPublishedContent currentContent)
        {
            return string.Concat(this.Prefix, currentContent.DocumentTypeAlias, this.Suffix);
        }
    }
}