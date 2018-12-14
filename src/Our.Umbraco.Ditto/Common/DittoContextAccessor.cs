using Umbraco.Core;
using Umbraco.Web;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// An interface allowing access to context objects without resulting to using the singletons directly.
    /// </summary>
    public interface IDittoContextAccessor
    {
        /// <summary>
        /// The UmbracoContext instance.
        /// </summary>
        UmbracoContext UmbracoContext { get; }

    }

    internal class DefaultDittoContextAccessor : IDittoContextAccessor
    {
        public UmbracoContext UmbracoContext
        {
            get
            {
                return UmbracoContext.Current;
            }
        }

    }
}