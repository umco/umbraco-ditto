using System.Linq;
using System.Web;
using Moq;
using Umbraco.Core;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Logging;
using Umbraco.Core.Profiling;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;

namespace Our.Umbraco.Ditto.Tests.Mocks
{
    public class MockDittoContextAccessor : IDittoContextAccessor
    {
        private readonly ApplicationContext _applicationContext;
        private readonly UmbracoContext _umbracoContext;

        /// <summary>
        /// All logic here based on https://github.com/garydevenay/Umbraco-Context-Mock/blob/f05d3da7a9442594f0735046aacaf8c7a341ff22/GDev.Umbraco.Test/ContextMocker.cs
        /// Credit goes to the original author, need to discuss whether we include this package into this test project
        /// </summary
        public MockDittoContextAccessor()
        {
            ILogger loggerMock = Mock.Of<ILogger>();
            IProfiler profilerMock = Mock.Of<IProfiler>();
            HttpContextBase contextBaseMock = Mock.Of<HttpContextBase>();
            WebSecurity webSecurityMock = new Mock<WebSecurity>(null, null).Object;
            IUmbracoSettingsSection umbracoSettingsSectionMock = Mock.Of<IUmbracoSettingsSection>();

            _applicationContext = new ApplicationContext(CacheHelper.CreateDisabledCacheHelper(), new ProfilingLogger(loggerMock, profilerMock));
            _umbracoContext = UmbracoContext.EnsureContext(contextBaseMock, ApplicationContext, webSecurityMock, umbracoSettingsSectionMock, Enumerable.Empty<IUrlProvider>(), true);
        }

        public ApplicationContext ApplicationContext
        {
            get
            {
                return _applicationContext;
            }
        }

        public UmbracoContext UmbracoContext
        {
            get
            {
                return _umbracoContext;
            }
        }
    }
}