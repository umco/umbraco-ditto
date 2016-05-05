using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    public class CustomPublishedContentPropertyTests
    {
        public class CustomPublishedContentMock : PublishedContentMock
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
            var propertyValue = "foo";
            var objectValue = "bar";

            var content = new CustomPublishedContentMock
            {
                Properties = new[] { new PublishedContentPropertyMock("myProperty", propertyValue) },
                MyProperty = objectValue
            };

            var model = content.As<MyModel>();

            Assert.That(model.MyProperty, Is.Not.EqualTo(propertyValue));
            Assert.That(model.MyProperty, Is.EqualTo(objectValue));
        }
    }
}