using System.Linq;
using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Mapping")]
    public class SingularityMappingTests
    {
        public class MyModel
        {
            public IPublishedContent MyProperty { get; set; }
        }

        [Test]
        public void Single_PublishedContent_Mapped_From_Collection()
        {
            // make a collection of content node objects
            var items = Enumerable.Repeat<IPublishedContent>(new PublishedContentMock(), 3);

            // set the collection to a content node's property
            var content = new PublishedContentMock
            {
                Properties = new[] { new PublishedContentPropertyMock("myProperty", items) }
            };

            // map the content node to our model
            var model = content.As<MyModel>();

            // check that the model is valid and the property contains a single value (not the collection)
            Assert.That(model, Is.Not.Null);
            Assert.That(model, Is.TypeOf<MyModel>());

            Assert.That(model.MyProperty, Is.Not.Null);
            Assert.That(model.MyProperty, Is.InstanceOf<IPublishedContent>());
        }
    }
}