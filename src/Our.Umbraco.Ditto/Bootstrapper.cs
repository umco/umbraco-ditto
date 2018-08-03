using Umbraco.Core;

namespace Our.Umbraco.Ditto
{
    internal sealed class Bootstrapper : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            Ditto.IsDebuggingEnabled = Ditto.GetDebugFlag();
        }
    }
}