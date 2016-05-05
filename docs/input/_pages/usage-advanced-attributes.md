title: Property attributes
permalink: /knowledgebase/property-attributes/
order: 2
---

# Advanced usage - Property attributes

To extend the mapping functionality, Ditto offers several attributes to decorate your POCO models:

* [`UmbracoProperty`](#umbracoproperty)
* [`AltUmbracoProperty`](#altumbracoproperty)
* [`UmbracoProperties`](#umbracoproperties)
* [`UmbracoDictionary`](#umbracodictionary)
* [`UmbracoPicker`](#umbracopicker)
* [`CurrentContentAs`](#currentcontentas)
* [`DittoIgnore`](#dittoignore)
* [`AppSetting`](#appsetting)


---

## `UmbracoProperty`

In situations where you would like to name a property in your POCO model differently to your DocumentType's property alias, you can use the `UmbracoProperty` attribute to define this.

Using the `MyTextModel` class POCO model from the [basic usage example](usage-basic), let's say that you would rather the `BodyText` property to be called `Content` instead.

You can do the following:

```csharp
[UmbracoProperty("bodyText")]
public string Content { get; set; }
```

Now Ditto will know to map the POCO's `Content` property to the DocumentType's "bodyText" property.


## `AltUmbracoProperty`

> // TODO: Add example
> 

## `UmbracoProperties`

> // TODO: Add example


## `UmbracoDictionary`

If you have a dictionary item (*set in the Umbraco back-office Settings section*), you can use this attribute to populate the a property value.

The advantage of using an Umbraco dictionary item is that the value is based on the context of the current page culture.

```csharp
[UmbracoDictionary("Labels.ReadMore")]
public string ReadMoreLabel { get; set; }
```


## `UmbracoPicker`

> // TODO: Add example


## `CurrentContentAs`

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


## `DittoIgnore`

For situations where you would rather that Ditto did not attempt to map a DocumentType property with one of your POCO model properties, you can use the `DittoIgnore` attribute:

```csharp
[DittoIgnore]
public string Image { get; set; }
```

When you map your content node, the ignored property (in this example, `Image`) will be set as the default value for that type (in this case that would be `null`).

The `DittoIgnore` attribute is useful for when you want to construct more complex POCO models.


## `AppSetting`

This attribute can be used for when you would like to populate your POCO model property with a value from the `Web.config` `<appSettings>` section. By supplying the key name for the app-setting, the value will be populated.

For example, if you wanted to display the Umbraco version number, you could do this:

```csharp
[AppSetting("umbracoConfigurationStatus")]
public string UmbracoVersion { get; set; }
```