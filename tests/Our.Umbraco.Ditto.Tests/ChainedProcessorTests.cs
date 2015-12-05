namespace Our.Umbraco.Ditto.Tests
{
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;

    [TestFixture]
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

        public class MyModel1
        {
            [UmbracoPropertyProcessor("Name", Order = 0), 
                DittoProcessor(typeof(MyCustomProcessor2), Order = 1), 
                DittoProcessor(typeof(MyCustomProcessor3), Order = 2)]
            public MyCustomModel MyProperty { get; set; }
        }   

        public class MyModel2
        {
            [DittoProcessor(typeof(MyCustomProcessor))]
            public MyCustomModel MyProperty { get; set; }
        }

        public class MyCustomProcessor : DittoProcessor
        {
            public override object ProcessValue()
            {
                return new MyCustomModel("MyCustomName");
            }
        }

        public class MyCustomProcessor2 : DittoProcessor
        {
            public override object ProcessValue()
            {
                if (Value is string)
                    return new MyCustomModel((string)Value);

                return null;
            }
        }

        public class MyCustomProcessor3 : DittoProcessor
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

            var content = new PublishedContentMock() { Name = "MyName" };
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