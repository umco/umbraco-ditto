# Getting started


## What is Ditto?

Ditto is an `IPublishedContent`-to-model mapper, with a goal of having strongly-typed models in your MVC views. It works by transforming an `IPublishedContent` object into an output (model/POCO) object of a different type. Ditto will attempt to automatically map the `IPublishedContent` properties with the properties of the model/POCO object by on the naming convention.


## Why use Ditto?

Strongly-typed models in MVC views are cool! Mapping code is boring! Ditto offers a simple way to achieve this. The real question is "why have strongly-typed models?" Cleaner MVC views, smaller view-models, separation of concerns, but to name a few.


## How do I use Ditto?

First, you need both a destination type to work with, once populated this will be your view-model. The design of the view-model can be influenced by the associated Document Type from Umbraco, but it doesn't necessarily have to, Ditto will attempt to match up the property and member names of the `IPublishedContent` object.  For example if you have a property alias on your Document Type called `"bodyText"`, then this will automatically be mapped to the destination object's property with the name `BodyText`.

Once you have your type, and a reference to Ditto, you can perform a mapping, using the `.As<T>()` extension method.

```csharp
var viewModel = Model.Content.As<MyViewModel>();
```

For more detailed examples, please see the [Basic usage](usage) section, followed by [Advanced usage](usage).


## Where do I configure Ditto?

Ditto does not require any pre-configuration within your website/web-application.  For complex mappings, you can decorate your model type with custom attributes, typically these would be `ValueResolvers` or `TypeConverters`, (or a comibnation of both).
