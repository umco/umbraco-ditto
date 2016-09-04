using System;
using System.Collections.Generic;
using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Processors")]
    public class DictionaryValueTests
    {
        public class MyModel
        {
            [UmbracoDictionary("hello")]
            public string MyProperty { get; set; }

            [UmbracoDictionary]
            public string Foo { get; set; }
        }

        [Test]
        public void DictionaryValue_Returned()
        {
            // Create mock dictionary items
            var mockDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "hello", "world" },
                { "foo", "bar" }
            };

            // Replace dictionary helper getter
            UmbracoDictionaryHelper.GetValue = (key) => mockDict[key];

            // Create mock content
            var content = new MockPublishedContent();

            // Run conversion
            var model = content.As<MyModel>();

            // Assert checks
            Assert.That(model.MyProperty, Is.EqualTo("world"));
            Assert.That(model.Foo, Is.EqualTo("bar"));
        }
    }
}