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
            var value = "myValue";

            var property = new PublishedContentPropertyMock
            {
                Alias = "myProperty",
                Value = value
            };

            var content = new CustomPublishedContentMock
            {
                Properties = new[] { property }
            };

            var model = content.As<MyModel>();

            Assert.That(model.MyProperty, Is.EqualTo(value));
        }
    }
}