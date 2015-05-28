# Usage

## Basic usage - `As<T>` extension method

For basic use of Ditto, let's start with a standard DocumentType with a few properties: Title, Content and Image:

![Umbraco DocType properties](umbraco-doctype-properties.png)

From that we will base the structure of our POCO model on those properties, we'd manually create a C# class:

```csharp
public class MyTextPage : Umbraco.Core.Models.PublishedContent.PublishedContentModel
{
	public MyTextPage(Umbraco.Core.Models.IPublishedContent content) : base(content) { }
	
	public string Title { get; set; }
	
	public string BodyText { get; set; }
	
	public string Image { get; set; }
}
```

> Note, that **the property names should corresponding with the DocumentType's property alias.** (If you wish to name your properties differently, then see the documentation about the `UmbracoProperty` attribute.)
> 
> The C# class name (`MyTextPage` in this example), does not need to match with the DocumentType alias.

Now you can map your `IPublishedContent` (content node of that DocumentType) to your corresponding POCO model using the `As<T>` extension method.

Here is an example of using the `As<T>` method within your Razor view:

```html
@using Our.Umbraco.Ditto
@inherits UmbracoTemplatePage
@{
	var poco = Model.Content.As<MyTextPage>();
}
<h1>@poco.Title</h1>
@poco.Content
<p><img src="@poco.Image" alt="[image]" /></p>
```

### Video demonstrations

* A short screencast giving [a brief introduction to Ditto](https://www.screenr.com/3oRN) (5 mins)
* Lee Kelleher showcases Ditto on [uHangout EP40](https://www.youtube.com/watch?v=L40haIBLNS4)



## Advanced usage - Property attributes

To extend the mapping functionality, Ditto offers several attributes to decorate your POCO models: `UmbracoProperty`, `UmbracoDictionary`, `AppSetting` and `DittoIgnore`:

### `UmbracoProperty`

In situations where you would like to name a property in your POCO model differently to your DocumentType's property alias, you can use the `UmbracoProperty` attribute to define it.

Using the example POCO model from above (`MyTextPage` class), let's say that you wanted the `BodyText` property to be called `Content`.  You can do the following:

	[Our.Umbraco.Ditto.UmbracoProperty("bodyText")]
	public string Content { get; set; }

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

Now when you map your content node, the ignored property (in this example, `Image`) will be set as the default value for that type (in this case it's `null`).

The `DittoIgnore` attribute is useful for when you want to construct more complex POCO models.


## Advanced usage - Type Converters

Sooner or later you'll reach a point where you will want to map a DocumentType property with a complex .NET type (either from within the .NET Framework, or custom).  To map these types with Ditto, you can use a standard .NET `TypeConverter`.

> If you are not familiar with .NET TypeConverters, please read Scott Hanselman's blog post: [TypeConverters: There's not enough TypeDescripter.GetConverter in the world](http://www.hanselman.com/blog/TypeConvertersTheresNotEnoughTypeDescripterGetConverterInTheWorld.aspx). This gives a good 'real-world' understanding of their purpose.
> 
> Then from there, refer to the MSDN documentation on [How to: Implement a Type Converter](http://msdn.microsoft.com/en-gb/library/ayybcxe5.aspx) 

Now with our example, let's say that you wanted the `Content` (formerly `BodyText`) property to be of type `HtmlString` (rather than a basic `string`).  You can reference a custom `TypeConverter` by adding the following attribute to the POCO property:

```csharp
[System.ComponentModel.TypeConverter(typeof(MyCustomConverter))]
[Our.Umbraco.Ditto.UmbracoProperty("bodyText")]
public System.Web.HtmlString Content { get; set; }
```

Then when the POCO property is populated the (raw) value (from `IPublishedContent`) will be processed through the custom `TypeConverter` and converted to the desired .NET type. 

Here is the example code for the `MyCustomConverter`, that converts a `string` to a `HtmlString` object:

```csharp
public class MyCustomConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		if (sourceType == typeof(string))
			return true;

		return base.CanConvertFrom(context, sourceType);
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
			if (value is string)
				return new System.Web.HtmlString((string)value);

			return base.ConvertFrom(context, culture, value);
	}
}
```


## Advanced usage - `As<T>()` Event hooks

When using the `As<T>` extension method, there are two event hooks that you can use to manipulate the processing of the object being mapped: `ConvertingType` and `ConvertedType`.

> // TODO: Add example

These event hooks are called across Ditto mappings, regardless of DocumentType or POCO model.


## Advanced usage - `As<T>()` Func delegates

Further to the `As<T>` event hooks, also available are two `Func` delegates for you to use to manipulate the mappings on a granular/ad-hoc level.

> // TODO: Add example


## Advanced usage - Value Resolvers

> // TODO: Add example
