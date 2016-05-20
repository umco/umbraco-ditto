using System;
using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    public class ValueTypesTests
    {
        public class MyModel
        {
            public bool Boolean { get; set; }

            public int Integer { get; set; }

            public int? NullableInteger { get; set; }
        }

        [Test]
        public void ValueTypes_With_Value()
        {
            var content = new MockPublishedContent
            {
                Properties = new[]
                {
                    new MockPublishedContentProperty("boolean", true),
                    new MockPublishedContentProperty("integer", 1234)
                }
            };

            var model = content.As<MyModel>();

            Assert.That(model, Is.Not.Null);
            Assert.That(model.Boolean, Is.TypeOf<bool>());
            Assert.That(model.Integer, Is.TypeOf<int>());
        }

        [Test]
        public void ValueTypes_With_DefaultValue()
        {
            var content = new MockPublishedContent();

            var model = content.As<MyModel>();

            Assert.That(model, Is.Not.Null);
            Assert.That(model.Boolean, Is.TypeOf<bool>());
            Assert.That(model.Integer, Is.TypeOf<int>());
            Assert.That(model.NullableInteger, Is.Null);
        }
    }
}