namespace Our.Umbraco.Ditto.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;

    [TestFixture]
    public class EnumerableDetectionTests
    {
        public class MyModel
        {
            [TypeConverter(typeof(MyConverter))]
            public Dictionary<string, string> MyProperty { get; set; }
        }

        public class MyConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return true;
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                return new Dictionary<string, string>
                {
                    { "hello", "world" },
                    { "foo", "bar" }
                };
            }
        }

        [Test]
        public void GenericDictionaryPropertyIsNotDetectedAsCastableEnumerable()
        {
            var property = new PublishedContentPropertyMock
            {
                Alias = "myProperty",
                Value = "myValue"
            };

            var content = new PublishedContentMock
            {
                Properties = new[] { property }
            };

            var result = content.As<MyModel>();

            Assert.NotNull(result.MyProperty);
            Assert.True(result.MyProperty.Any());
            Assert.AreEqual(result.MyProperty["hello"], "world");
        }
    }
}