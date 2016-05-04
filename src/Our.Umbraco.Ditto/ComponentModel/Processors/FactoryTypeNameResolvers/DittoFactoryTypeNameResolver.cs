using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Base class for a DittoFactoryTypeNameResovler
    /// </summary>
    public abstract class DittoFactoryTypeNameResolver
    {
        /// <summary>
        /// Resolves a type name from the given contextual information
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="currentContent"></param>
        /// <returns></returns>
        public abstract string ResolveTypeName(DittoProcessorContext ctx, IPublishedContent currentContent);
    }
}
