using System.Collections.Generic;
using System.Globalization;
using Umbraco.Core;
using Umbraco.Web.Media.EmbedProviders.Settings;

namespace Our.Umbraco.Ditto.Tests
{
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;

    [TestFixture]
    public class ValueResolverContextTests
    {
        public class MyValueResolverModel
        {
            [DittoValueResolver(typeof(MyValueResolver))]
            public string MyProperty { get; set; }
        }

        public class MyValueResolver : DittoValueResolver<MyValueResolverContext>
        {
            public override object ResolveValue(MyValueResolverContext context, 
                DittoValueResolverAttribute attribute, CultureInfo culture)
            {
                return context.MyContextProperty;
            }
        }

        public class MyValueResolverContext : DittoValueResolverContext
        {
            public MyValueResolverContext()
            {
                MyContextProperty = "Default value";
            }

            public string MyContextProperty { get; set; }
        }

        [Test]
        public void Value_Converter_Context_Resolves()
        {
            DittoValueResolver.SetContextCache(new Dictionary<string, object>());

            var content = new PublishedContentMock();

            Ditto.RegisterValueResolverContext(new MyValueResolverContext
            {
                MyContextProperty = "Test"
            });

            var model = content.As<MyValueResolverModel>();

            Assert.That(model.MyProperty, Is.EqualTo("Test"));
        }

        [Test]
        public void Value_Converter_Context_Resolves_Without_Registered_Context()
        {
            DittoValueResolver.SetContextCache(new Dictionary<string, object>());

            var content = new PublishedContentMock();

            var model = content.As<MyValueResolverModel>();

            Assert.That(model.MyProperty, Is.EqualTo("Default value"));
        }
    }
}