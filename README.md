# Ditto

<img align="right" height="300" src="docs/umbraco-ditto.png">

Ditto is a lightweight POCO mapper for Umbraco 7. It offers a generic, unopinionated
solution to the problem of using **strongly-typed models in your MVC views**.
There are no 3<sup>rd</sup> party dependencies, other than Umbraco 7 core itself.

#### Is Ditto a "Yet Another Code-First" approach?

Nope! The goal of Ditto is to provide a simple way to convert your content/media nodes (e.g. `IPublishedContent`) to your desired POCO/model/object.

There is absolutely zero intention of generating Document-Types from your POCO/models.



## Getting Started

### Installation

> *Note:* Ditto has been developed against **Umbraco v7.1.4** and will support that version and above.

Ditto can be installed from either Our Umbraco or NuGet package repositories, or build manually from the source-code:

#### Our Umbraco package repository

To install from Our Umbraco, please download the package from:

> [http://our.umbraco.org/projects/developer-tools/ditto](http://our.umbraco.org/projects/developer-tools/ditto) 

#### NuGet package repository

To [install from NuGet](https://www.nuget.org/packages/Our.Umbraco.Ditto), you can run the following command from within Visual Studio:

	PM> Install-Package Our.Umbraco.Ditto

#### Manual build

If you prefer, you can compile Ditto yourself, you'll need:

* Visual Studio 2012 (or above)

To clone it locally click the "Clone in Windows" button above or run the following git commands.

	git clone https://github.com/leekelleher/umbraco-ditto.git umbraco-ditto
	cd umbraco-ditto
	.\build.cmd


---


### Usage

*...documentation coming soon...*

	@(Model.Content.As<Article>.PageTitle)


#### Video demonstrations

* A short screencast giving [a brief introduction to Ditto](https://www.screenr.com/3oRN) (5 mins)


---


### Configuration


#### `IPublishedContent` Model Factory

As of v7.1.4, Umbraco ships with using a default model factory for `IPublishedContent`.
For more information about the [IPublishedContentModelFactory](https://github.com/zpqrtbnk/Zbu.ModelsBuilder/wiki/IPublishedContentModelFactory) please the "Zbu.ModelsBuilder" wiki:

> [https://github.com/zpqrtbnk/Zbu.ModelsBuilder/wiki/IPublishedContentModelFactory](https://github.com/zpqrtbnk/Zbu.ModelsBuilder/wiki/IPublishedContentModelFactory)

Ditto comes with its own custom model factory, to enable this, use the following code:

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

The factory will look for any POCO models that inherit from the `PublishedContentModel` class and automatically map those to the requested `IPublishedContent` objects.


#### Video demonstrations

* [Using Ditto with the `IPublishedContentModelFactory`](https://www.screenr.com/9oRN) (5 mins)
* [Using a custom Umbraco MVC controller with PublishedContentModelFactory and Ditto]() (5 mins)



## Contributing to this project

Anyone and everyone is welcome to contribute. Please take a moment to review the [guidelines for contributing](CONTRIBUTING.md).

* [Bug reports](CONTRIBUTING.md#bugs)
* [Feature requests](CONTRIBUTING.md#features)
* [Pull requests](CONTRIBUTING.md#pull-requests)


## Contact

Have a question?

* [Ditto Forum](http://our.umbraco.org/projects/developer-tools/ditto/ditto-feedback/) on Our Umbraco
* [Raise an issue](https://github.com/leekelleher/umbraco-ditto/issues) on GitHub


## Ditto Team

* [Lee Kelleher](https://github.com/leekelleher)
* [Matt Brailsford](https://github.com/mattbrailsford)

Special thanks to [Darren Ferguson](https://github.com/darrenferguson) for inspiration in his article "[Mapping Umbraco content to POCOs for strongly typed views](http://24days.in/umbraco/2013/mapping-content-to-pocos/)" on the [24 days in Umbraco](http://24days.in/umbraco/) advent calender.


## License

Copyright &copy; 2014 Umbrella Inc, Our Umbraco and [other contributors](https://github.com/leekelleher/umbraco-ditto/graphs/contributors)

Licensed under the [MIT License](LICENSE.md)
