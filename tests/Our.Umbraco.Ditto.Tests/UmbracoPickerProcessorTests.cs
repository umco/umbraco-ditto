using System.Linq;
using System.Web;
using System.Web.Security;
using Moq;
using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Core;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.ObjectResolution;
using Umbraco.Core.Profiling;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;

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

                PublishedCachesResolver.Current =
                    new PublishedCachesResolver(new PublishedCaches(mockPublishedContentCache.Object, new Mock<IPublishedMediaCache>().Object));
            }

            UmbracoContext.EnsureContext(
                httpContext: Mock.Of<HttpContextBase>(),
                applicationContext: new ApplicationContext(CacheHelper.CreateDisabledCacheHelper(), new ProfilingLogger(Mock.Of<ILogger>(), Mock.Of<IProfiler>())),
                webSecurity: new Mock<WebSecurity>(null, null).Object,
                umbracoSettings: Mock.Of<IUmbracoSettingsSection>(),
                urlProviders: Enumerable.Empty<IUrlProvider>(),
                replaceContext: true);

            Resolution.Freeze();

            NodeId = 1234;

            Content = new MockPublishedContent()
            {
                Properties = new[] { new MockPublishedContentProperty("myProperty", NodeId) }
            };

            UmbracoPickerHelper.GetMembershipHelper = (ctx) => new MembershipHelper(ctx, Mock.Of<MembershipProvider>(), Mock.Of<RoleProvider>());
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