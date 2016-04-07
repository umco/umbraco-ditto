using System;
using System.Collections.Generic;
using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    public class DictionaryValueTests
    {
        private UmbracoDictionaryProcessorContext DictionaryContext;

        [TestFixtureSetUp]
        public void Init()
        {
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "hello", "world" },
                { "foo", "bar" }
            };

            DictionaryContext = new UmbracoDictionaryProcessorContext
            {
                GetDictionaryValue = (x) => values[x]
            };
        }

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
            var content = new PublishedContentMock();

            var model = content.As<MyModel>(processorContexts: new[] { DictionaryContext });

            Assert.That(model.MyProperty, Is.EqualTo("world"));
            Assert.That(model.Foo, Is.EqualTo("bar"));
        }
    }
}