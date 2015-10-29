namespace Our.Umbraco.Ditto.Tests
{
    using Mocks;
    using NUnit.Framework;

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

            var property = new PublishedContentPropertyMock
            {
                Alias = "myProperty",
                Value = propertyValue
            };

            var content = new CustomPublishedContentMock
            {
                Properties = new[] { property },
                MyProperty = objectValue
            };

            var model = content.As<MyModel>();

            Assert.That(model.MyProperty, Is.Not.EqualTo(propertyValue));
            Assert.That(model.MyProperty, Is.EqualTo(objectValue));
        }
    }
}