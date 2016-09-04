using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Mapping")]
    public class CustomPublishedContentPropertyTests
    {
        public class CustomPublishedContentMock : MockPublishedContent
        {
            public string MyProperty { get; set; }
        }

        public class MyModel
        {
            public string MyProperty { get; set; }
        }

        [Test]
        public void CustomPublishedContent_Property_IsMapped()
        {
            // The idea here is that the value from the concrete property
            // on the custom model take presidence over the
            // IPublishedContentProperty value.

            var propertyValue = "foo";
            var objectValue = "bar";

            var content = new CustomPublishedContentMock
            {
                Properties = new[] { new MockPublishedContentProperty("myProperty", propertyValue) },
                MyProperty = objectValue
            };

            var model = content.As<MyModel>();

            Assert.That(model.MyProperty, Is.Not.EqualTo(propertyValue));
            Assert.That(model.MyProperty, Is.EqualTo(objectValue));
        }
    }
}