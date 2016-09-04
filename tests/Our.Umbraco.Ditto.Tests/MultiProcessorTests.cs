using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Processors")]
    public class MultiProcessorTests
    {
        public class MyModel
        {
            [UmbracoProperty("Name", Order = 1),
                MyMultiProcessor(Order = 2),
                MyCustomProcessor3(Order = 3)]
            public string MyProperty { get; set; }
        }
        public class MyModel2
        {
            [UmbracoProperty("Name", Order = 1),
                MyEmptyBaseConstructorMultiProcessor(Order = 2),
                MyCustomProcessor3(Order = 3)]
            public string MyProperty { get; set; }
        }

        public class MyMultiProcessorAttribute : DittoMultiProcessorAttribute
        {
            public MyMultiProcessorAttribute()
                : base(new DittoProcessorAttribute[]
                {
                      new MyCustomProcessor1Attribute(),
                      new MyCustomProcessor2Attribute(),
                      new MyCustomProcessor3Attribute()
                })
            { }
        }


        public class MyEmptyBaseConstructorMultiProcessorAttribute : DittoMultiProcessorAttribute
        {
            public MyEmptyBaseConstructorMultiProcessorAttribute()
            {
                base.Attributes.AddRange(
                    new DittoProcessorAttribute[]
                    {
                        new MyCustomProcessor1Attribute(),
                        new MyCustomProcessor2Attribute(),
                        new MyCustomProcessor3Attribute()
                    });
            }
        }

        public class MyCustomProcessor1Attribute : DittoProcessorAttribute
        {
            public override object ProcessValue()
            {
                if (Value is string)
                {
                    return ((string)Value) + "Test1";
                }

                return null;
            }
        }

        public class MyCustomProcessor2Attribute : DittoProcessorAttribute
        {
            public override object ProcessValue()
            {
                if (Value is string)
                {
                    return ((string)Value) + "Test2";
                }

                return null;
            }
        }

        public class MyCustomProcessor3Attribute : DittoProcessorAttribute
        {
            public override object ProcessValue()
            {
                if (Value is string)
                {
                    return ((string)Value) + "Test3";
                }

                return null;
            }
        }

        [Test]
        public void MultiProcessor_Processes_Test()
        {
            // In this test, the `MyProperty` property gets a `string` value
            // via the `UmbracoProperty`. The `string` type/value is passed
            // to the `MyCustomConverter` so to convert the `string` to a
            // `MyCustomModel` type/object.

            var content = new MockPublishedContent() { Name = "MyName" };
            var model = content.As<MyModel>();

            Assert.That(model, Is.Not.Null);
            Assert.That(model, Is.InstanceOf<MyModel>());

            Assert.That(model.MyProperty, Is.Not.Null);
            Assert.That(model.MyProperty, Is.EqualTo("MyNameTest1Test2Test3Test3"));
        }


        [Test]
        public void MultiProcessor_EmptyConstructor_Test()
        {
            // In this test, the `MyProperty` property gets a `string` value
            // via the `UmbracoProperty`. The `string` type/value is passed
            // to the `MyCustomConverter` so to convert the `string` to a
            // `MyCustomModel` type/object.

            // Then a second class which uses the `MyModel2` and the `MyEmptyBaseConstructorMultiProcessorAttribute` calling similar code.
            // It then asserts that the output of `MyProperty` for `MyModel` is equal to thw value of `MyProperty` of `MyModel2`.

            var content = new MockPublishedContent() { Name = "MyName" };
            var model = content.As<MyModel>();
            var model2 = content.As<MyModel2>();

            Assert.That(model, Is.Not.Null);
            Assert.That(model, Is.InstanceOf<MyModel>());

            Assert.That(model2, Is.Not.Null);
            Assert.That(model2, Is.InstanceOf<MyModel2>());

            Assert.That(model.MyProperty, Is.Not.Null);
            Assert.That(model2.MyProperty, Is.Not.Null);
            Assert.That(model.MyProperty, Is.EqualTo(model2.MyProperty));
        }
    }
}