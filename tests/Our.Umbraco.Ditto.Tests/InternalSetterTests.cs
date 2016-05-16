using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    public class InternalSetterTests
    {
        public class MyModel
        {
            [UmbracoProperty("myProperty")]
            public string MyProperty1 { get; }

            [UmbracoProperty("myProperty")]
            public string MyProperty2 { get; set; }

            [UmbracoProperty("myProperty")]
            public string MyProperty3 { get; internal set; }
        }

        [Test]
        public void InternalSetter_Test()
        {
            var value = "foo";
            var content = new PublishedContentMock
            {
                Properties = new[] { new PublishedContentPropertyMock("myProperty", value, true) }
            };

            var model = content.As<MyModel>();

            Assert.That(model.MyProperty1, Is.Null);
            Assert.That(model.MyProperty2, Is.EqualTo(value));
            Assert.That(model.MyProperty3, Is.EqualTo(value));
        }
    }
}