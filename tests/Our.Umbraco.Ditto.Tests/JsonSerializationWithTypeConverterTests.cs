using System;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [NUnit.Framework.Category("Type Casting")]
    public class JsonSerializationWithTypeConverterTests
    {
        public class MyModel
        {
            public MyJsonModel MyProperty { get; set; }
        }

        [TypeConverter(typeof(MyJsonModelConverter))]
        public class MyJsonModel
        {
            public string Name { get; set; }
        }

        public class MyJsonModelConverter : TypeConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                // If this returned `true`, then the `ConvertTo` method will need to be implemented, otherwise the conversion would fail.
                return false;
            }

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof(JObject);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                return ((JObject)value).ToObject<MyJsonModel>();
            }
        }

        [Test]
        public void Json_Serialize_Object_Mapping()
        {
            var txt = "foo bar";
            var obj = new MyJsonModel { Name = txt };

            var value = JObject.FromObject(obj);

            Assert.That(value, Is.Not.Null);

            var property = new PublishedContentPropertyMock("myProperty", value);
            var content = new PublishedContentMock { Properties = new[] { property } };

            var model = content.As<MyModel>();

            Assert.That(model, Is.Not.Null);
            Assert.That(model.MyProperty, Is.TypeOf<MyJsonModel>());
            Assert.That(model.MyProperty, Is.Not.Null);
            Assert.That(model.MyProperty.Name, Is.EqualTo(txt));
        }
    }
}