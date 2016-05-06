title: Getting Started
permalink: /getting-started/
order: 1
---

## The Who, What, Whys
### What is Ditto?

Ditto is a view-model mapper specifically designed to map `IPublishedContent` objects to a custom type. The goal is to easily enable the use of strongly-typed models within your MVC views.

Ditto will automatically map the properties from `IPublishedContent` with the properties of the your model object (POCO class) based on naming conventions.


### Why use Ditto?

Strongly-typed models in MVC views are cool! Mapping code is boring! Ditto offers a simple way to achieve this.

The real question is "why have strongly-typed models?", this leads to cleaner MVC views, smaller simpler view-models, and separation of concerns.


### How do I use Ditto?

First, you need a destination type to work with, once constructed, this will be your view-model. The design of the view-model can be influenced by the associated Document Type from Umbraco, but it doesn't necessarily have to be. Ditto will attempt to match up the property and member names of the `IPublishedContent` object.  For example if you have a property alias on your Document Type called `"bodyText"`, then this will automatically be mapped to the destination object's property with the name `BodyText`.

> Note: The property-aliases are case-insensitive, so you could name your object's property `BoDyTeXt` if you really _really_ wanted to. Ditto would still map it.

Once you have your type and a reference to Ditto, you can perform a mapping, using the `.As<T>()` extension method.

```csharp
var viewModel = Model.Content.As<MyViewModel>();
```

For more detailed examples, please see the [Basic usage](./basic-usage/) section, followed by [Advanced usage](usage-advanced-attributes).


### Where do I configure Ditto?

Ditto does not require any pre-configuration within your website/web-application.  For complex mappings, you can decorate your model type with custom attributes, typically these would be [`Processors`](usage-advanced-processors).
