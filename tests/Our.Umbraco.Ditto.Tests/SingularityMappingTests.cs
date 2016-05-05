using System.Linq;
using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    public class SingularityMappingTests
    {
        public class MyModel
        {
            [UmbracoPicker]
            public IPublishedContent MyProperty { get; set; }
        }

        [Test]
        public void Single_PublishedContent_Mapped_From_Collection()
        {
            var mock = new PublishedContentMock();
            var items = Enumerable.Repeat<IPublishedContent>(mock, 3);

            var content = new PublishedContentMock()
            {
                Properties = new[] { new PublishedContentPropertyMock("myProperty", items) }
            };

            var model = content.As<MyModel>();

            Assert.That(model, Is.Not.Null);
            Assert.That(model.MyProperty, Is.Not.Null);
        }
    }
}