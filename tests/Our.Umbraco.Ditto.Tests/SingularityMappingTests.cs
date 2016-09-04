using System.Linq;
using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto.Tests
{
    using System.Collections.Generic;

    [TestFixture]
    [Category("Mapping")]
    public class SingularityMappingTests
    {
        public class MyModel
        {
            public IPublishedContent MyProperty { get; set; }
        }

        public class MyModel2
        {
            public IEnumerable<IPublishedContent> MyProperty { get; set; }
        }

        /// <summary>
        /// Tests to ensure that collections of items are correctly mapped to a single item.
        /// </summary>
        [Test]
        public void Single_PublishedContent_Mapped_From_Collection()
        {
            // make a collection of content node objects
            var items = Enumerable.Repeat<IPublishedContent>(new MockPublishedContent(), 3);

            // set the collection to a content node's property
            var content = new MockPublishedContent
            {
                Properties = new[] { new MockPublishedContentProperty("myProperty", items) }
            };

            // map the content node to our model
            var model = content.As<MyModel>();

            // check that the model is valid and the property contains a single value (not the collection)
            Assert.That(model, Is.Not.Null);
            Assert.That(model, Is.TypeOf<MyModel>());

            Assert.That(model.MyProperty, Is.Not.Null);
            Assert.That(model.MyProperty, Is.InstanceOf<IPublishedContent>());
        }

        /// <summary>
        /// Tests to ensure that single items are correctly mapped to an enumerable.
        /// </summary>
        [Test]
        public void Collection_PublishedContent_Mapped_From_Single()
        {
            // make a content node
            var item = new MockPublishedContent();

            // set the item to a content node's property
            var content = new MockPublishedContent
            {
                Properties = new[] { new MockPublishedContentProperty("myProperty", item) }
            };

            // map the content node to our model
            var model = content.As<MyModel2>();

            // check that the model is valid and the property contains a collection (not a single item)
            Assert.That(model, Is.Not.Null);
            Assert.That(model, Is.TypeOf<MyModel2>());

            Assert.That(model.MyProperty, Is.Not.Null);
            Assert.That(model.MyProperty, Is.InstanceOf<IEnumerable<IPublishedContent>>());
        }
    }
}