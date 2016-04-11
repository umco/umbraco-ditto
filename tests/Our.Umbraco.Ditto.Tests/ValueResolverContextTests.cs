namespace Our.Umbraco.Ditto.Tests
{
    using System.Collections.Generic;
    using System.Globalization;
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;
    using global::Umbraco.Core;
    using global::Umbraco.Web.Media.EmbedProviders.Settings;

    [TestFixture]
    public class ValueResolverContextTests
    {
        public class MyValueResolverModel
        {
            [DittoValueResolver(typeof(MyValueResolver))]
            public string MyProperty { get; set; }

            [DittoValueResolver(typeof(MyValueResolver))]
            public virtual string MyLazyProperty { get; set; }
        }

        public class MyValueResolver : DittoValueResolver<MyValueResolverContext>
        {
            public override object ResolveValue()
            {
                return Context.MyContextProperty;
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
        public void ValueResolverContext_Resolves()
        {
            var content = new PublishedContentMock();
            var context = new MyValueResolverContext
            {
                MyContextProperty = "Test"
            };

            var model = content.As<MyValueResolverModel>(valueResolverContexts: new[] { context });

            Assert.That(model.MyProperty, Is.EqualTo("Test"));
            Assert.That(model.MyLazyProperty, Is.EqualTo(model.MyProperty));
        }

        [Test]
        public void ValueResolverContext_Resolves_Without_Registered_Context()
        {
            var content = new PublishedContentMock();

            var model = content.As<MyValueResolverModel>();

            Assert.That(model.MyProperty, Is.EqualTo("Default value"));
            Assert.That(model.MyLazyProperty, Is.EqualTo(model.MyProperty));
        }
    }
}