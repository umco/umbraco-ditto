using Umbraco.Core;
using Umbraco.Core.Components;

namespace Our.Umbraco.Ditto
{
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