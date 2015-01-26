namespace Our.Umbraco.Ditto
{
    using global::Umbraco.Core;
    using global::Umbraco.Web;

    /// <summary>
    /// Provides helper methods to aid with conversion. Not much in here for now but who knows 
    /// what the future has in store?
    /// </summary>
    internal static class ConverterHelper
    {
        /// <summary>
        /// Gets the <see cref="UmbracoHelper"/> for querying published content or media.
        /// </summary>
        public static UmbracoHelper UmbracoHelper
        {
            get
            {
                // Pull the item from the cache if possible to reduce the db access overhead caused by 
                // multiple reflection iterations for the given type taking place in a single request.
                return (UmbracoHelper)ApplicationContext.Current.ApplicationCache.RequestCache.GetCacheItem(
                        "Ditto.UmbracoHelper",
                        () => new UmbracoHelper(UmbracoContext.Current));
            }
        }
    }
}