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
                Properties = new[] { 
                    new PublishedContentPropertyMock
                    {
                        PropertyTypeAlias = "myProperty",
                        Value = "myValue"
                    }
                }
            };

            var result = content.As<MyModel>();

            Assert.NotNull(result.MyProperty);
            Assert.True(result.MyProperty.Any());
            Assert.AreEqual(result.MyProperty["hello"], "world");
        }

        [Test]
        public void EnumerablesCast()
        {
            var content = new PublishedContentMock
            {
                Properties = new[] { 
                    new PublishedContentPropertyMock
                    {
                        PropertyTypeAlias = "enumerableToSingle",
                        Value = new[]{"myVal", "myOtherVal"}
                    },
                    new PublishedContentPropertyMock
                    {
                        PropertyTypeAlias = "singleToEnumerable",
                        Value = "myVal"
                    }
                }
            };

            var result = content.As<MyModel>();

            Assert.NotNull(result.EnumerableToSingle);
            Assert.AreEqual(result.EnumerableToSingle, "myVal");

            Assert.NotNull(result.SingleToEnumerable);
            Assert.IsTrue(result.SingleToEnumerable.Any());
            Assert.AreEqual(result.SingleToEnumerable.First(), "myVal");
        }
    }
}