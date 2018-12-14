using System.Linq;
using System.Web;
using Moq;
using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Profiling;
using Umbraco.Core.Xml;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Processors")]
    public class XPathProcessorTests
    {
        //
        // This unit-test is to prove that the XPath processor will parse the variable names
        // and return an IEnumerable<IPublishedContent> from the content cache.
        //

        public class MyModel
        {
            [UmbracoXPath("$current")]
            public IPublishedContent Self { get; set; }

            [UmbracoXPath("$parent")]
            public IPublishedContent Parent { get; set; }

            [UmbracoXPath("$site")]
            public IPublishedContent Site { get; set; }

            [UmbracoXPath("$root")]
            public IPublishedContent Root { get; set; }
        }

        [TestFixtureSetUp]
        public void Init()
        {
            if (PublishedCachesResolver.HasCurrent == false)
            {
                var mockPublishedContentCache = new Mock<IPublishedContentCache>();

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
            }

            UmbracoContext.EnsureContext(
                httpContext: Mock.Of<HttpContextBase>(),
                webSecurity: new Mock<WebSecurity>(null, null).Object,
                umbracoSettings: Mock.Of<IUmbracoSettingsSection>(),
                urlProviders: Enumerable.Empty<IUrlProvider>(),
                replaceContext: true);

            if (!Resolution.IsFrozen)
                Resolution.Freeze();
        }

        [Test]
        public void XPath_Processes_PublishedContent()
        {
            var parent = new MockPublishedContent { Id = 1111, Path = "-1,1111" };
            var content = new MockPublishedContent { Id = 2222, Path = "-1,1111,2222", Parent = parent };

            var model = content.As<MyModel>();

            Assert.That(model, Is.Not.Null);

            Assert.That(model.Self, Is.Not.Null);
            Assert.That(model.Self.Id, Is.EqualTo(content.Id));

            Assert.That(model.Parent, Is.Not.Null);
            Assert.That(model.Parent.Id, Is.EqualTo(parent.Id));

            Assert.That(model.Site, Is.Not.Null);
            Assert.That(model.Site.Id, Is.EqualTo(parent.Id));

            Assert.That(model.Root, Is.Not.Null);
            Assert.That(model.Root.Id, Is.EqualTo(parent.Id));
        }
    }
}