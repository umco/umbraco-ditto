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
        public class MyModel
        {
            [UmbracoDictionary("hello")]
            public string MyProperty { get; set; }

            [UmbracoDictionary]
            public string Foo { get; set; }
        }

        public class MockUmbracoDictionary : IUmbracoDictionary
        {
            private readonly Dictionary<string, string> Values;

            public MockUmbracoDictionary()
            {
                Values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "hello", "world" },
                    { "foo", "bar" }
                };
            }

            public string GetValue(string key)
            {
                return Values[key];
            }
        }

        [Test]
        public void DictionaryValue_Returned()
        {
            UmbracoDictionaryFactory.Set(new MockUmbracoDictionary());

            var content = new PublishedContentMock();

            var model = content.As<MyModel>();

            Assert.That(model.MyProperty, Is.EqualTo("world"));
            Assert.That(model.Foo, Is.EqualTo("bar"));
        }
    }
}