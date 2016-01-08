using System;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Moq;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;

namespace Our.Umbraco.Ditto.Tests
{
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;

    [TestFixture]
    public class CacheTests
    {
        public class MyValueResolverModel
        {
            [UmbracoProperty(CacheDuration = 10)]
            public string MyProperty1 { get; set; }

            //[MyProcessor(CacheDuration = 30)]
            public string MyProperty2 { get; set; }
        }

        [DittoProcessorMetaData(ContextType = typeof(MyProcessorContext))]
        public class MyProcessorAttribute : DittoProcessorAttribute
        {
            public override object ProcessValue()
            {
                return ((MyProcessorContext)Context).MyContextProperty;
            }
        }

        public class MyProcessorContext : DittoProcessorContext
        {
            public MyProcessorContext()
            {
                MyContextProperty = "Default value";
            }

            public string MyContextProperty { get; set; }
        }

        [Test]
        public void ProcessorCache_Caches()
        {
            var cacheHelper = new CacheHelper(
                new ObjectCacheRuntimeCacheProvider(),
                new StaticCacheProvider(),
                new NullCacheProvider());

            var appCtx = new ApplicationContext(cacheHelper);
            ApplicationContext.EnsureContext(appCtx, true);

            var prop1 = new PublishedContentPropertyMock
            {
                Alias = "myProperty1",
                Value = "Test1"
            };

            var content = new PublishedContentMock
            {
                Id = 1,
                Properties = new[] { prop1 }
            };

            var model = content.As<MyValueResolverModel>();

            Assert.That(model.MyProperty1, Is.EqualTo("Test1"));

            prop1.Value = "Test2";

            model = content.As<MyValueResolverModel>();

            Assert.That(model.MyProperty1, Is.EqualTo("Test1"));
        }
    }
}