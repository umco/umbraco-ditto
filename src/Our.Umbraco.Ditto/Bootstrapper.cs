using Umbraco.Core.Components;

namespace Our.Umbraco.Ditto
{
    // TODO : I think this is the correct interface for startup now, need to check
    internal sealed class Bootstrapper : IUmbracoComponent
    {
        public void Compose(Composition composition)
        {
            Ditto.IsDebuggingEnabled = Ditto.GetDebugFlag();
        }

        public void Terminate()
        {

        }
    }
}