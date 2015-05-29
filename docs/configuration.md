# Configuration


## `IPublishedContent` Model Factory

> **Warning:** You will receive limited support if you choose to use the Model Factory with Ditto.

As of v7.1.4, Umbraco ships with using a default model factory for `IPublishedContent`.
For more information about the [IPublishedContentModelFactory](https://github.com/zpqrtbnk/Zbu.ModelsBuilder/wiki/IPublishedContentModelFactory) please the "Zbu.ModelsBuilder" wiki:

> [https://github.com/zpqrtbnk/Zbu.ModelsBuilder/wiki/IPublishedContentModelFactory](https://github.com/zpqrtbnk/Zbu.ModelsBuilder/wiki/IPublishedContentModelFactory)

Ditto comes with a custom model factory, to enable this, use the following code:

```csharp
using Our.Umbraco.Ditto;

public class ConfigurePublishedContentModelFactory : ApplicationEventHandler
{
	protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
	{
		var types = PluginManager.Current.ResolveTypes<PublishedContentModel>();
		var factory = new DittoPublishedContentModelFactory(types);
		PublishedContentModelFactoryResolver.Current.SetFactory(factory);
	}
}
```

The factory will look for any POCO models that inherit from the `PublishedContentModel` class and automatically map those to the requested `IPublishedContent` objects.

> **Warning:** If you have a POCO model that contains a reference to a child node, then this may create a circular-reference and cause the model-factory to enter an endless loop!


## Video demonstrations

* [Using Ditto with the `IPublishedContentModelFactory`](https://www.screenr.com/9oRN) (5 mins)
* [Using a custom Umbraco MVC controller with `IPublishedContentModelFactory` and Ditto](https://www.screenr.com/PPRN) (5 mins)

