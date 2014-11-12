using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.Ditto
{
    public static class PublishedContentModelFactoryResolverExtensions
    {
        public static void SetFactory<T>(this PublishedContentModelFactoryResolver resolver)
            where T : IPublishedContent
        {
            var types = PluginManager.Current.ResolveTypes<T>();
            var factory = new DittoPublishedContentModelFactory(types);

            resolver.SetFactory(factory);
        }
    }
}