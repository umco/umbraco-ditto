# Advanced usage

## Property attributes

To extend the mapping functionality, Ditto offers several attributes to decorate your POCO models:

* `UmbracoProperty`
* `UmbracoDictionary`
* `AppSetting`
* `DittoIgnore`
* `CurrentContentAs`


### `UmbracoProperty`

In situations where you would like to name a property in your POCO model differently to your DocumentType's property alias, you can use the `UmbracoProperty` attribute to define it.

Using the example POCO model from the basic usage example (`MyTextModel` class), let's say that you wanted the `BodyText` property to be called `Content`.  You can do the following:

```csharp
[Our.Umbraco.Ditto.UmbracoProperty("bodyText")]
public string Content { get; set; }
```

Now Ditto will know to map the POCO's `Content` property to the DocumentTypes 'bodyText' property.


### `UmbracoDictionary`

If you have a dictionary item (set in the Settings section), you can use this attribute to populate the value. The advantage of using an Umbraco dictionary item is that the value is based on the culture of the current page being loaded.

```csharp
[Our.Umbraco.Ditto.UmbracoDictionary("Labels.ReadMore")]
public string ReadMoreLabel { get; set; }
```


### `AppSetting`

This attribute can be used for when you would like to populate your POCO model property with a value from the `Web.config` `<appSettings>` section. By supplying the key name for the app-setting, the value will be populated.

```csharp
[Our.Umbraco.Ditto.AppSetting("umbracoConfigurationStatus")]
public string UmbracoVersion { get; set; }
```


### `DittoIgnore`

For situations where you would rather that Ditto did not attempt to map a DocumentType property with one of your POCO models properties, you can use the `DittoIgnore` attribute:

```csharp
[Our.Umbraco.Ditto.DittoIgnore]
public string Image { get; set; }
```

When you map your content node, the ignored property (in this example, `Image`) will be set as the default value for that type (in this case it's `null`).

The `DittoIgnore` attribute is useful for when you want to construct more complex POCO models.


### `CurrentContentAs`

This attribute is used for when you would like to reapply the current `IPublishedContent` object to a nested (inner) property of your POCO model.

> // TODO: Add examples

