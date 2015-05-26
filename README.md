# Ditto

[![Build status](https://img.shields.io/appveyor/ci/leekelleher/umbraco-ditto.svg)](https://ci.appveyor.com/project/leekelleher/umbraco-ditto)
[![Documentation Status](https://readthedocs.org/projects/umbraco-ditto/badge/?version=latest)](https://readthedocs.org/projects/umbraco-ditto/?badge=latest)
[![NuGet release](https://img.shields.io/nuget/v/Our.Umbraco.Ditto.svg)](https://www.nuget.org/packages/Our.Umbraco.Ditto)
[![Our Umbraco project page](https://img.shields.io/badge/our-umbraco-orange.svg)](https://our.umbraco.org/projects/developer-tools/ditto)
[![Chat on Gitter](https://img.shields.io/badge/gitter-join_chat-green.svg)](https://gitter.im/leekelleher/umbraco-ditto)

<img align="right" height="300" src="docs/umbraco-ditto.png">

Ditto is a lightweight POCO mapper for Umbraco. It offers a generic solution to the problem of using **strongly-typed models in your MVC views**.
There are no 3<sup>rd</sup> party dependencies, other than Umbraco core itself.

#### Is Ditto a "Yet Another Code-First" approach?

Nope! The goal of Ditto is to provide a simple way to convert your content/media nodes (e.g. `IPublishedContent`) to your desired POCO/model/object.

There is absolutely zero intention of generating Document-Types from your POCO/models.

## Documentation and notes

> Ditto has been developed against **Umbraco v6.2.5** and will support that version and above.

Ditto can be installed from either Our Umbraco or NuGet package repositories, or build manually from the source-code.

For detailed documentation, please visit: http://umbraco-ditto.readthedocs.org/


## Contributing to this project

Anyone and everyone is welcome to contribute. Please take a moment to review the [guidelines for contributing](CONTRIBUTING.md).

* [Bug reports](CONTRIBUTING.md#bugs)
* [Feature requests](CONTRIBUTING.md#features)
* [Pull requests](CONTRIBUTING.md#pull-requests)


## Contact us

Have a question?

* [Ditto Forum](http://our.umbraco.org/projects/developer-tools/ditto/ditto-feedback/) on Our Umbraco
* [Raise an issue](https://github.com/leekelleher/umbraco-ditto/issues) on GitHub


## Ditto Team

* [Lee Kelleher](https://github.com/leekelleher)
* [Matt Brailsford](https://github.com/mattbrailsford)
* [James South](https://github.com/JimBobSquarePants)

### Special thanks

* Thanks to [Darren Ferguson](https://github.com/darrenferguson) for inspiration in his article "[Mapping Umbraco content to POCOs for strongly typed views](http://24days.in/umbraco/2013/mapping-content-to-pocos/)" on the [24 days in Umbraco](http://24days.in/umbraco/) advent calender.
* Thanks to [Jeavon Leopold](https://github.com/Jeavon) for being a rockstar and adding AppVeyor support.
* Thanks to [Hendy Racher](https://github.com/Hendy) for testing and contributions.


## License

Copyright &copy; 2014 Umbrella Inc, Our Umbraco and [other contributors](https://github.com/leekelleher/umbraco-ditto/graphs/contributors)

Licensed under the [MIT License](LICENSE.md)
