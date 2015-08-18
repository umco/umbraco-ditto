# Advanced usage

## Type Converters

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
