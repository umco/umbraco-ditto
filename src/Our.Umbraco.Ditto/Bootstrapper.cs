using Umbraco.Core;

namespace Our.Umbraco.Ditto
{
    internal sealed class Bootstrapper : ApplicationEventHandler
    {
        protected override void ApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            // Add lookups for types with specific Ditto attributes...
            if (PluginManager.Current != null)
            {
                if (AttributedTypeResolver<DittoCacheAttribute>.HasCurrent == false)
                {
                    AttributedTypeResolver<DittoCacheAttribute>.Current = AttributedTypeResolver<DittoCacheAttribute>.Create(PluginManager.Current);
                }

                if (AttributedTypeResolver<DittoDefaultProcessorAttribute>.HasCurrent == false)
                {
                    AttributedTypeResolver<DittoDefaultProcessorAttribute>.Current = AttributedTypeResolver<DittoDefaultProcessorAttribute>.Create(PluginManager.Current);
                }

                if (AttributedTypeResolver<UmbracoPropertiesAttribute>.HasCurrent == false)
                {
                    AttributedTypeResolver<UmbracoPropertiesAttribute>.Current = AttributedTypeResolver<UmbracoPropertiesAttribute>.Create(PluginManager.Current);
                }
            }
        }
    }
}