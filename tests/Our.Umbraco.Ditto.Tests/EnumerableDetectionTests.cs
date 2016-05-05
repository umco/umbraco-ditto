using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Collections"), Category("Processors")]
    public class EnumerableDetectionTests
    {
        public class MyModel
        {
            [MyProcessor]
            public Dictionary<string, string> MyProperty { get; set; }

            public string EnumerableToSingle { get; set; }

            public IEnumerable<string> SingleToEnumerable { get; set; }
        }

        public class MyProcessorAttribute : DittoProcessorAttribute
        {
            public override object ProcessValue()
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
            var content = new PublishedContentMock
            {
                Properties = new[] { new PublishedContentPropertyMock("myProperty", "myValue") }
            };

            var result = content.As<MyModel>();

            Assert.That(result.MyProperty, Is.Not.Null);
            Assert.That(result.MyProperty.Any(), Is.True);
            Assert.That(result.MyProperty["hello"], Is.EqualTo("world"));
        }

        [Test]
        public void EnumerablesCast()
        {
            var propertyValue = "myVal";

            var content = new PublishedContentMock
            {
                Properties = new[]
                {
                    new PublishedContentPropertyMock("enumerableToSingle", new[] { propertyValue, "myOtherVal" }),
                    new PublishedContentPropertyMock("singleToEnumerable", propertyValue)
                }
            };

            var result = content.As<MyModel>();

            Assert.That(result.EnumerableToSingle, Is.Not.Null);
            Assert.That(result.EnumerableToSingle, Is.EqualTo(propertyValue));

            Assert.That(result.SingleToEnumerable, Is.Not.Null);
            Assert.That(result.SingleToEnumerable.Any(), Is.True);
            Assert.That(result.SingleToEnumerable.First(), Is.EqualTo(propertyValue));
        }
    }
}