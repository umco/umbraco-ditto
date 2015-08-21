namespace Our.Umbraco.Ditto.Tests
{
	using System;
	using System.ComponentModel;
	using System.Globalization;
	using NUnit.Framework;
	using Our.Umbraco.Ditto.Tests.Mocks;

	[TestFixture]
	public class ClassLevelTypeConverterTests
	{
		[TypeConverter(typeof(MyCustomConverter))]
		public class MyCustomModel
		{
			public MyCustomModel(string name)
			{
				Name = name;
			}

			public string Name { get; set; }
		}

		public class MyModel1
		{
			[UmbracoProperty("Name")]
			public MyCustomModel MyProperty { get; set; }
		}

		public class MyModel2
		{
			[DittoValueResolver(typeof(MyCustomValueResolver))]
			public MyCustomModel MyProperty { get; set; }
		}

		public class MyCustomValueResolver : DittoValueResolver
		{
			public override object ResolveValue()
			{
				return new MyCustomModel("MyCustomName");
			}
		}

		public class MyCustomConverter : TypeConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return sourceType == typeof(string);
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				if (value is string)
					return new MyCustomModel((string)value);

				return base.ConvertFrom(context, culture, value);
			}
		}

		[Test]
		public void ClassLevel_TypeConverter_UmbracoProperty()
		{
			// In this test, the `MyProperty` property gets a `string` value
			// via the `UmbracoProperty`. The `string` type/value is passed
			// to the `MyCustomConverter` so to convert the `string` to a
			// `MyCustomModel` type/object.

			var content = new PublishedContentMock() { Name = "MyName" };
			var model = content.As<MyModel1>();

			Assert.IsNotNull(model);
			Assert.IsInstanceOf<MyModel1>(model);

			Assert.IsNotNull(model.MyProperty);
			Assert.IsInstanceOf<MyCustomModel>(model.MyProperty);
			Assert.That(model.MyProperty.Name, Is.EqualTo("MyName"));
		}

		[Test]
		public void ClassLevel_TypeConverter_ValueResolver()
		{
			// In this test, the `MyProperty` property gets its value from
			// the `MyCustomValueResolver` (returning a `MyCustomModel`).
			// The `MyCustomConverter` is called, but fails the
			// `CanConvertFrom` check, so wouldn't try to convert it.
			// Since the value type is the same as the target property type,
			// the property value can be set.

			var content = new PublishedContentMock() { Name = "MyName" };
			var model = content.As<MyModel2>();

			Assert.IsNotNull(model);
			Assert.IsInstanceOf<MyModel2>(model);

			Assert.IsNotNull(model.MyProperty);
			Assert.IsInstanceOf<MyCustomModel>(model.MyProperty);
			Assert.That(model.MyProperty.Name, Is.EqualTo("MyCustomName"));
		}
	}
}