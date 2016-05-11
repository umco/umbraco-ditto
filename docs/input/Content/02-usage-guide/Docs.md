title: Usage Guide
---

## <a name="basic-usage" title="Basic Usage"></a>Basic usage - `As<T>` extension method

For basic use of Ditto, let's start with a standard DocumentType with a few properties:

* Title (_Textstring_)
* BodyText (_Richtext editor_)
* Image (_Upload_)

![Umbraco DocType properties](/assets/img/umbraco-doctype-properties.png)

Using those properties as the structure of our view-model (POCO class), we can manually create a C# class:

```csharp
public class MyTextModel
{
	public string Title { get; set; }
	
	public string BodyText { get; set; }
	
	public string Image { get; set; }
}
```

> Note: in this case **the property names should correspond with the DocumentType's property aliases.** (If you wish to name your properties differently, please [see the documentation about the `UmbracoProperty` attribute.](usage-advanced-attributes/#umbracoproperty))
> 
> The C# class name (`MyTextModel` in this example), does not need to match with the DocumentType alias.

Now you can map your `IPublishedContent` (content node of that DocumentType) to your corresponding POCO model using the `As<T>` extension method.

```csharp
var poco = Model.Content.As<MyTextModel>();
```

Here is an example in the context of an MVC view:

```html
@using Our.Umbraco.Ditto
@inherits UmbracoTemplatePage
@{
	var poco = Model.Content.As<MyTextModel>();
}

<h1>@poco.Title</h1>

@poco.BodyText

<p><img src="@poco.Image" alt="[image]" /></p>

```

### Video demonstrations

* A short screencast giving [a brief introduction to Ditto](https://www.screenr.com/3oRN) (5 mins)
* Lee Kelleher showcases Ditto on [uHangout EP40](https://www.youtube.com/watch?v=L40haIBLNS4)

---------------------------------------------------------------------------------------

## <a name="processors" title="Processors"></a>Processors

To extend the mapping functionality, Ditto offers several attributes to decorate your POCO models:

* [`UmbracoProperty`](#umbracoproperty)
* [`AltUmbracoProperty`](#altumbracoproperty)
* [`UmbracoProperties`](#umbracoproperties)
* [`UmbracoDictionary`](#umbracodictionary)
* [`UmbracoPicker`](#umbracopicker)
* [`CurrentContentAs`](#currentcontentas)
* [`DittoIgnore`](#dittoignore)
* [`AppSetting`](#appsetting)


### `UmbracoProperty`

In situations where you would like to name a property in your POCO model differently to your DocumentType's property alias, you can use the `UmbracoProperty` attribute to define this.

Using the `MyTextModel` class POCO model from the [basic usage example](usage-basic), let's say that you would rather the `BodyText` property to be called `Content` instead.

You can do the following:

```csharp
[UmbracoProperty("bodyText")]
public string Content { get; set; }
```

Now Ditto will know to map the POCO's `Content` property to the DocumentType's "bodyText" property.


### `AltUmbracoProperty`

> // TODO: Add example
> 

### `UmbracoProperties`

> // TODO: Add example


### `UmbracoDictionary`

If you have a dictionary item (*set in the Umbraco back-office Settings section*), you can use this attribute to populate the a property value.

The advantage of using an Umbraco dictionary item is that the value is based on the context of the current page culture.

```csharp
[UmbracoDictionary("Labels.ReadMore")]
public string ReadMoreLabel { get; set; }
```


### `UmbracoPicker`

> // TODO: Add example


### `CurrentContentAs`

This attribute is used for when you would like to re-apply the current `IPublishedContent` object to a nested (inner) property of your POCO model.

For example, on your DocumentType you had properties for SEO/meta-data and that you had a small POCO model to represent that...

```csharp
public class MyMetaDataModel
{
    public string MetaTitle { get; set; }

    public string MetaDescription { get; set; }

    public string MetaKeywords { get; set; }
}
```

... in order to re-use this POCO model within your containing view-model, you can do this:

```csharp
public class MyModel
{
    [CurrentContentAs]
    public MyMetaDataModel MetaData { get; set; }
}
```

The result of this would be that within your MVC view, you could have something like this:

```csharp
var poco = Model.Content.As<MyModel>();

<title>@poco.MetaData.MetaTitle</title>
```


### `DittoIgnore`

For situations where you would rather that Ditto did not attempt to map a DocumentType property with one of your POCO model properties, you can use the `DittoIgnore` attribute:

```csharp
[DittoIgnore]
public string Image { get; set; }
```

When you map your content node, the ignored property (in this example, `Image`) will be set as the default value for that type (in this case that would be `null`).

The `DittoIgnore` attribute is useful for when you want to construct more complex POCO models.


### `AppSetting`

This attribute can be used for when you would like to populate your POCO model property with a value from the `Web.config` `<appSettings>` section. By supplying the key name for the app-setting, the value will be populated.

For example, if you wanted to display the Umbraco version number, you could do this:

```csharp
[AppSetting("umbracoConfigurationStatus")]
public string UmbracoVersion { get; set; }
```

---------------------------------------------------------------------------------------

## <a name="custom-processors" title="Custom Processors"></a>Custom Processors

The key feature of Ditto is the ability to process a value (typically from an `IPublishedContent` property) and set it to the property of the target view-model. To do this we use a Processor (or a combination of Processors).

While Ditto covers the most common types of processing, (via the use of [attributes](usage-advanced-attributes), there may be scenarios where you may need a little help in processing custom (or complex) values.

Traditionally any custom processor logic would be typically done within an MVC controller.  However, if the logic is only relevant to the mapping operation, then it may clutter your controller code and be better suited as a custom `Processor`.

For example, let's look at having a calculated value during mapping, say that you wanted to display the number of days since a piece of content was last updated:

```csharp
public class MyModel
{
    [MyCustomProcessor]
    public int DaysSinceUpdated { get; set; }
}

public class MyCustomProcessor : DittoProcessorAttribute
{
    public override object ProcessValue()
    {
        var content = Value as IPublishedContent;
        if (content == null) return null;

        return (DateTime.UtcNow - content.UpdateDate).Days;
    }
}
```

Once mapped, the value of `DaysSinceUpdated` would contain the number of days difference between the content item's last update date and today's date (UTC now).

---------------------------------------------------------------------------------------

## <a name="chaining-processors" title="Chaining Processors"></a>Chaining Processors

---------------------------------------------------------------------------------------

## <a name="caching-processors" title="Caching Processors"></a>Caching Processors

---------------------------------------------------------------------------------------

## <a name="conversion-handlers" title="Conversion Handlers"></a>Conversion Handlers

---------------------------------------------------------------------------------------

### `DittoConversionHandler` attribute

> // TODO: Add examples of `DittoConversionHandler` attribute

---------------------------------------------------------------------------------------

### `DittoOnConverting` attribute

> // TODO: Add examples of `DittoOnConverting` attribute

---------------------------------------------------------------------------------------


### `DittoOnConverted` attribute

> // TODO: Add examples of `DittoOnConverted` attribute

---------------------------------------------------------------------------------------


### Globally register conversion handlers

> // TODO: Add examples of `Ditto.RegisterConversionHandler`

---------------------------------------------------------------------------------------

## <a name="dittoview" title="DittoView"></a>DittoView

---------------------------------------------------------------------------------------

## <a name="dittocontroller" title="DittoController"></a>DittoController

---------------------------------------------------------------------------------------