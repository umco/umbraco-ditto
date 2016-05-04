using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Type name resolver for DittoFactory based on current contents Doc Type
    /// </summary>
    public class DittoDocTypeFactoryTypeNameResolver : DittoFactoryTypeNameResolver
    {
        /// <summary>
        /// Resolves the type name from a content items Doc Type
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="currentContent"></param>
        /// <returns></returns>
        public override string ResolveTypeName(DittoProcessorContext ctx, IPublishedContent currentContent)
        {
            return currentContent.DocumentTypeAlias;
        }
    }
}
