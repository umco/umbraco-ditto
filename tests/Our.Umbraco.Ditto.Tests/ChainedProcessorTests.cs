namespace Our.Umbraco.Ditto.Tests
{
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;

    [TestFixture]
    [Category("Processors")]
    public class ChainedProcessorTests
    {
        public class MyCustomModel
        {
            public MyCustomModel(string name)
            {
                Name = name;
            }

            public string Name { get; set; }
        }

        [UmbracoProperty("Title", Order = 0)]
        [AltUmbracoProperty("Name", Order = 1)]
        [MyCustomProcessor4(Order = 2)]
        [MyCustomProcessor3(Order = 3)]
        public class MyCustomModel2
        {
            public MyCustomModel2(string name)
            {
                Name = name;
            }

            public string Name { get; set; }
        }

        public class MyModel1
        {
            [UmbracoProperty("Title", Order = 0)]
            [AltUmbracoProperty("Name", Order = 1)]
            [MyCustomProcessor2(Order = 2)]
            [MyCustomProcessor3(Order = 3)]
            public MyCustomModel MyProperty { get; set; }
        }

        public class MyModel2
        {
            [MyCustomProcessor]
            public MyCustomModel MyProperty { get; set; }
        }

        public class MyModel3
        {
            public MyCustomModel2 MyProperty { get; set; }
        }

        public class MyCustomProcessorAttribute : DittoProcessorAttribute
        {
            public override object ProcessValue()
            {
                return new MyCustomModel("MyCustomName");
            }
        }

        public class MyCustomProcessor4Attribute : DittoProcessorAttribute
        {
            public override object ProcessValue()
            {
                return new MyCustomModel2("MyCustomName");
            }
        }

        public class MyCustomProcessor2Attribute : DittoProcessorAttribute
        {
            public override object ProcessValue()
            {
                if (Value is string)
                    return new MyCustomModel((string)Value);

                return null;
            }
        }

        public class MyCustomProcessor3Attribute : DittoProcessorAttribute
        {
            public override object ProcessValue()
            {
                return Value;
            }
        }

        [Test]
        public void ChainedProcessor_ChainedProcessor()
        {
            // In this test, the `MyProperty` property gets a `string` value
            // via the `UmbracoProperty`. The `string` type/value is passed
            // to the `MyCustomConverter` so to convert the `string` to a
            // `MyCustomModel` type/object.

            var content = new MockPublishedContent() { Name = "MyName" };
            var model = content.As<MyModel1>();

            Assert.IsNotNull(model);
            Assert.IsInstanceOf<MyModel1>(model);

            Assert.IsNotNull(model.MyProperty);
            Assert.IsInstanceOf<MyCustomModel>(model.MyProperty);
            Assert.That(model.MyProperty.Name, Is.EqualTo("MyName"));
        }

        [Test]
        public void ChainedProcessor_SingleProcessor()
        {
            // In this test, the `MyProperty` property gets its value from
            // the `MyCustomValueResolver` (returning a `MyCustomModel`).
            // The `MyCustomConverter` is called, but fails the
            // `CanConvertFrom` check, so wouldn't try to convert it.
            // Since the value type is the same as the target property type,
            // the property value can be set.

            var content = new MockPublishedContent() { Name = "MyName" };
            var model = content.As<MyModel2>();

            Assert.IsNotNull(model);
            Assert.IsInstanceOf<MyModel2>(model);

            Assert.IsNotNull(model.MyProperty);
            Assert.IsInstanceOf<MyCustomModel>(model.MyProperty);
            Assert.That(model.MyProperty.Name, Is.EqualTo("MyCustomName"));
        }

        [Test]
        public void ChainedProcessor_ChainedClassLevelProcessors()
        {
            // In this test, the `MyProperty` property gets a `string` value
            // via the `UmbracoProperty`. The `string` type/value is passed
            // to the `MyCustomConverter` so to convert the `string` to a
            // `MyCustomModel` type/object.

            var content = new MockPublishedContent() { Name = "MyName" };
            var model = content.As<MyModel3>();

            Assert.IsNotNull(model);
            Assert.IsInstanceOf<MyModel3>(model);

            Assert.IsNotNull(model.MyProperty);
            Assert.IsInstanceOf<MyCustomModel2>(model.MyProperty);
            Assert.That(model.MyProperty.Name, Is.EqualTo("MyCustomName"));
        }
    }
}