using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Conveniense processor wrapper for resolving a type based on a content items Doc Type
    /// </summary>
    public class DittoDocTypeFactoryAttribute : DittoFactoryAttribute
    {
        /// <summary>
        /// A prefix string to prepend onto the resolved type name
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// A sufix string to append onto the resolved type name
        /// </summary>
        public string Sufix { get; set; }

        /// <summary>
        /// Fetches the type name from the current contents doc type alias
        /// </summary>
        /// <param name="currentContent"></param>
        /// <returns></returns>
        public override string ResolveTypeName(IPublishedContent currentContent)
        {
            return Prefix + currentContent.DocumentTypeAlias + Sufix;
        }
    }
}