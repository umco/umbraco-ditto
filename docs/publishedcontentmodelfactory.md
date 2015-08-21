# `IPublishedContent` Model Factory

> **Warning:** The Model Factory implementation for Ditto should be considered as a prototype / proof-of-concept. You will receive limited support (from the Ditto team) if you choose to use it.

As of v7.1.4, Umbraco ships with using a default model factory for `IPublishedContent`.
For more information about the [IPublishedContentModelFactory](https://github.com/zpqrtbnk/Zbu.ModelsBuilder/wiki/IPublishedContentModelFactory) please the "Zbu.ModelsBuilder" wiki:

> [https://github.com/zpqrtbnk/Zbu.ModelsBuilder/wiki/IPublishedContentModelFactory](https://github.com/zpqrtbnk/Zbu.ModelsBuilder/wiki/IPublishedContentModelFactory)


## Configuration

Ditto offers a custom model factory, to enable this, first you need to install the NuGet package:

```bash
PM> Install-Package Our.Umbraco.Ditto.ModelFactory
```

Then, use the following code:

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

> **Warning:** If you have a POCO model that contains a reference to a child node, then this may create a circular-reference and cause the model-factory to enter an endless loop!  This is fixable by marking your property with the `virtual` keyword, as this enables the [lazy-loading feature](usage-advanced-lazyloading).


## Video demonstrations

* [Using Ditto with the `IPublishedContentModelFactory`](https://www.screenr.com/9oRN) (5 mins)
* [Using a custom Umbraco MVC controller with `IPublishedContentModelFactory` and Ditto](https://www.screenr.com/PPRN) (5 mins)

