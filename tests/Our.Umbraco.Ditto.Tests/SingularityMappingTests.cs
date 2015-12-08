namespace Our.Umbraco.Ditto.Tests
{
    using System.ComponentModel;
    using System.Linq;
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;
    using global::Umbraco.Core.Models;

    [TestFixture]
    public class SingularityMappingTests
    {
        public class MyModel
        {
            [TypeConverter(typeof(DittoPickerConverter))]
            public IPublishedContent MyProperty { get; set; }
        }

        [Test]
        public void Single_PublishedContent_Mapped_From_Collection()
        {
            var mock = new PublishedContentMock();
            var items = Enumerable.Repeat<IPublishedContent>(mock, 3);

            var property = new PublishedContentPropertyMock()
            {
                Alias = "myProperty",
                Value = items
            };

            var content = new PublishedContentMock()
            {
                Properties = new[] { property }
            };

            var model = content.As<MyModel>();

            Assert.That(model, Is.Not.Null);
            Assert.That(model.MyProperty, Is.Not.Null);
        }
    }
}