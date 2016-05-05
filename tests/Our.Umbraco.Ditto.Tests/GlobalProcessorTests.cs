using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Processors")]
    public class GlobalProcessorTests
    {
        public class MyProcessorModel
        {
            public MyStringModel MyStrProperty { get; set; }

            public MyIntModel MyIntProperty { get; set; }
        }

        public class MyStringModel
        {
            public string Value { get; set; }
        }

        public class MyIntModel
        {
            public int Value { get; set; }
        }

        public class MyIntProcessorAttr : DittoProcessorAttribute
        {
            public int AttrProp { get; set; }

            public override object ProcessValue()
            {
                return new MyIntModel { Value = AttrProp };
            }
        }

        public class MyStrProcessorAttribute : DittoProcessorAttribute
        {
            public override object ProcessValue()
            {
                return new MyStringModel { Value = "Test" };
            }
        }

        [Test]
        public void Global_Value_Converter_Resolves()
        {
            Ditto.RegisterProcessorAttribute<MyStringModel, MyStrProcessorAttribute>();
            Ditto.RegisterProcessorAttribute<MyIntModel, MyIntProcessorAttr>(new MyIntProcessorAttr { AttrProp = 2 });
            Ditto.RegisterProcessorAttribute<MyIntModel, MyIntProcessorAttr>(new MyIntProcessorAttr { AttrProp = 5 });

            var content = new MockPublishedContent();

            var model = content.As<MyProcessorModel>();

            Assert.That(model.MyStrProperty.Value, Is.EqualTo("Test"));
            Assert.That(model.MyIntProperty.Value, Is.EqualTo(5));
        }
    }
}