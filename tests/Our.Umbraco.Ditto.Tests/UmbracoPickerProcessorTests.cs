using System.Linq;
using Moq;
using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.ObjectResolution;
using Umbraco.Core.Xml;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Processors")]
    public class UmbracoPickerProcessorTests
    {
        public class MyModel<T>
        {
            [UmbracoProperty(Order = 1)]
            [UmbracoPicker(Order = 2)]
            public T MyProperty { get; set; }
        }

        public class MyTypedModel
        {
            public int Id { get; set; }
        }

        private IPublishedContent Content;

        private int NodeId;

        [TestFixtureSetUp]
        public void Init()
        {
            if (!PublishedCachesResolver.HasCurrent)
            {
                var mockPublishedContentCache = new Mock<IPublishedContentCache>();

                mockPublishedContentCache
                        .Setup(x => x.GetById(It.IsAny<UmbracoContext>(), It.IsAny<bool>(), It.IsAny<int>()))
                        .Returns<UmbracoContext, bool, int>((ctx, preview, id) => new MockPublishedContent { Id = id });

                // NOTE: The `GetByXPath` mock method is duplicated from the XPathProcessorTests.
                // We need to perform a full review of the mocking objects so that they work across all the unit-tests. [LK:2017-02-10]
                mockPublishedContentCache
                    .Setup(x => x.GetByXPath(It.IsAny<UmbracoContext>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<XPathVariable[]>()))
                    .Returns<UmbracoContext, bool, string, XPathVariable[]>(
                        (ctx, preview, xpath, vars) =>
                        {
                            switch (xpath)
                            {
                                case "/root":
                                case "id(1111)":
                                    return new MockPublishedContent { Id = 1111 }.AsEnumerableOfOne();

                                case "id(2222)":
                                    return new MockPublishedContent { Id = 2222 }.AsEnumerableOfOne();

                                default:
                                    return Enumerable.Empty<IPublishedContent>();
                            }
                        });

                PublishedCachesResolver.Current =
                    new PublishedCachesResolver(new PublishedCaches(mockPublishedContentCache.Object, new Mock<IPublishedMediaCache>().Object));

                if (!Resolution.IsFrozen)
                    Resolution.Freeze();
            }

            NodeId = 1234;

            Content = new MockPublishedContent()
            {
                Properties = new[] { new MockPublishedContentProperty("myProperty", NodeId) }
            };

        }


        [TestFixtureTearDown]
        public void Teardown()
        {
            if (Resolution.IsFrozen)
            {
                Resolution.Reset();
            }
        }

        [Test]
        public void UmbracoPicker_Processes_PublishedContent()
        {
            var model = Content.As<MyModel<IPublishedContent>>();

            Assert.That(model.MyProperty, Is.Not.Null);
            Assert.IsInstanceOf<IPublishedContent>(model.MyProperty);
            Assert.That(model.MyProperty.Id, Is.EqualTo(NodeId));
        }

        [Test]
        public void UmbracoPicker_Processes_Typed()
        {
            var model = Content.As<MyModel<MyTypedModel>>();

            Assert.That(model.MyProperty, Is.Not.Null);
            Assert.IsInstanceOf<MyTypedModel>(model.MyProperty);
            Assert.That(model.MyProperty.Id, Is.EqualTo(NodeId));
        }
    }
}