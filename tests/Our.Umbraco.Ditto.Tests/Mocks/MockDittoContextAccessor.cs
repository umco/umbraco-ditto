using System.Linq;
using System.Web;
using Moq;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Profiling;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;

namespace Our.Umbraco.Ditto.Tests.Mocks
{
    public class MockDittoContextAccessor : IDittoContextAccessor
    {
        /// <summary>
        /// All logic here based on https://github.com/garydevenay/Umbraco-Context-Mock/blob/f05d3da7a9442594f0735046aacaf8c7a341ff22/GDev.Umbraco.Test/ContextMocker.cs
        /// Credit goes to the original author, need to discuss whether we include this package into this test project
        /// </summary
        public MockDittoContextAccessor()
        {
            HttpContextBase contextBaseMock = Mock.Of<HttpContextBase>();
            WebSecurity webSecurityMock = new Mock<WebSecurity>(null, null, null).Object;
            IUmbracoContextAccessor contextAccessor = Mock.Of<IUmbracoContextAccessor>();
            IGlobalSettings globalSettings = Mock.Of<IGlobalSettings>();
            IPublishedSnapshotService publishedSnapshotService = Mock.Of<IPublishedSnapshotService>();
            var webRoutingSection = new Mock<IWebRoutingSection>();
            webRoutingSection.Setup(w => w.UrlProviderMode).Returns("Auto");
            var umbracoSettingsSectionMock = new Mock<IUmbracoSettingsSection>();
            umbracoSettingsSectionMock.Setup(s => s.WebRouting).Returns(webRoutingSection.Object);
            IVariationContextAccessor variationContextAccessor = Mock.Of<IVariationContextAccessor>();

            
            UmbracoContext = UmbracoContext.EnsureContext(contextAccessor, contextBaseMock, publishedSnapshotService, webSecurityMock, umbracoSettingsSectionMock.Object, Enumerable.Empty<IUrlProvider>(), globalSettings, variationContextAccessor, true);
        }

        
        public UmbracoContext UmbracoContext { get; }
    }
}