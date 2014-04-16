# Ditto

<img align="right" height="300" src="Docs/umbraco-ditto.png">

Ditto is a lightweight POCO mapper for Umbraco 7. It offers a generic, unopinionated
solution to the problem of using **strongly-typed models in your MVC views**.
There are no 3<sup>rd</sup> party dependencies, other than Umbraco 7 core itself.

### Is Ditto "Yet Another Code-First" approach?

Nope! The goal of Ditto is to provide a simple way to convert your content nodes (`IPublishedContent`) into your desired POCO/model/object.

There is absolutely zero intention of attempting to generate DocumentTypes from your POCO/models.


## Installing Ditto

Ditto can be installed from either Our Umbraco or NuGet package repositories

To install from Our Umbraco, please download the package from:

[http://our.umbraco.org/projects/developer-tools/ditto](http://our.umbraco.org/projects/developer-tools/ditto) 

To install from NuGet, you can run the following command from within Visual Studio:

	PM> Install-Package Our.Umbraco.Ditto


## Usage

*...documentation coming soon...*

	@(Model.Content.As<Article>.PageTitle)


## Configuration

*...documentation coming soon...*

## Contact

Have a question?

* [Ditto Forum](http://our.umbraco.org/projects/developer-tools/ditto/ditto-feedback/) on Our Umbraco
* [Raise an issue](https://github.com/leekelleher/umbraco-ditto/issues) on GitHub


## Contributing to this project

Anyone and everyone is welcome to contribute. Please take a moment to
review the [guidelines for contributing](CONTRIBUTING.md).

* [Bug reports](CONTRIBUTING.md#bugs)
* [Feature requests](CONTRIBUTING.md#features)
* [Pull requests](CONTRIBUTING.md#pull-requests)


## Ditto Team

* [Lee Kelleher](https://github.com/leekelleher)
* [Matt Brailsford](https://github.com/mattbrailsford)

Special thanks to [Darren Ferguson](https://github.com/darrenferguson) for inspiration in his article "[Mapping Umbraco content to POCOs for strongly typed views](http://24days.in/umbraco/2013/mapping-content-to-pocos/)" on the [24 days in Umbraco](http://24days.in/umbraco/) advent calender.


## License

Copyright &copy; 2014 Umbrella Inc, Our Umbraco and other contributors

Licensed under the MIT License
