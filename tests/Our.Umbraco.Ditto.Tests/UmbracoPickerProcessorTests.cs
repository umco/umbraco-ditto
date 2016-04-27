using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.ObjectResolution;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
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
                PublishedCachesResolver.Current =
                    new PublishedCachesResolver(new PublishedCaches(new MockPublishedContentCache(), new MockPublishedMediaCache()));

            UmbracoContext.EnsureContext(new MockHttpContext(), new ApplicationContext(new CacheHelper()), true);

            Resolution.Freeze();

            NodeId = 1234;

            Content = new PublishedContentMock()
            {
                Properties = new[]
                {
                    new PublishedContentPropertyMock("myProperty", NodeId, true) }
            };
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