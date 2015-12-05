namespace Our.Umbraco.Ditto.Tests
{
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;

    [TestFixture]
    public class MultiProcessorTests
    {
        public class MyModel
        {
            [UmbracoPropertyProcessor("Name", Order = 1),
                MyMultiProcessor(Order = 2),
                DittoProcessor(typeof(MyCustomProcessor3), Order = 3)]
            public string MyProperty { get; set; }
        }

        public class MyMultiProcessorAttribute : DittoMultiProcessorAttribute
        {
            public MyMultiProcessorAttribute()
                : base(new[] {
                      new DittoProcessorAttribute(typeof(MyCustomProcessor1)), 
                      new DittoProcessorAttribute(typeof(MyCustomProcessor2)), 
                      new DittoProcessorAttribute(typeof(MyCustomProcessor3))         
                })
            { }
        }

        public class MyCustomProcessor1 : DittoProcessor
        {
            public override object ProcessValue()
            {
                if (Value is string)
                    return ((string)Value) + "Test1";

                return null;
            }
        }

        public class MyCustomProcessor2 : DittoProcessor
        {
            public override object ProcessValue()
            {
                if (Value is string)
                    return ((string)Value) + "Test2";

                return null;
            }
        }

        public class MyCustomProcessor3 : DittoProcessor
        {
            public override object ProcessValue()
            {
                if (Value is string)
                    return ((string)Value) + "Test3";

                return null;
            }
        }

        [Test]
        public void MultiProcessorTests_MultiProcessorProcesses()
        {
            // In this test, the `MyProperty` property gets a `string` value
            // via the `UmbracoProperty`. The `string` type/value is passed
            // to the `MyCustomConverter` so to convert the `string` to a
            // `MyCustomModel` type/object.

            var content = new PublishedContentMock() { Name = "MyName" };
            var model = content.As<MyModel>();

            Assert.IsNotNull(model);
            Assert.IsInstanceOf<MyModel>(model);

            Assert.IsNotNull(model.MyProperty);
            Assert.That(model.MyProperty, Is.EqualTo("MyNameTest1Test2Test3Test3"));
        }
    }
}