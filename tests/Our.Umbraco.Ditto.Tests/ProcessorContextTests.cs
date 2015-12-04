namespace Our.Umbraco.Ditto.Tests
{
    using System.Collections.Generic;
    using System.Globalization;
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;
    using global::Umbraco.Core;
    using global::Umbraco.Web.Media.EmbedProviders.Settings;

    [TestFixture]
    public class ProcessorContextTests
    {
        public class MyValueResolverModel
        {
            [DittoProcessor(typeof(MyProcessor))]
            public string MyProperty { get; set; }
        }

        public class MyProcessor : DittoProcessor<object, MyProcessorContext>
        {
            public override object ProcessValue()
            {
                return Context.MyContextProperty;
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
        public void ValueResolverContext_Resolves()
        {
            var content = new PublishedContentMock();
            var context = new MyProcessorContext
            {
                MyContextProperty = "Test"
            };

            var model = content.As<MyValueResolverModel>(processorContexts: new[] { context });

            Assert.That(model.MyProperty, Is.EqualTo("Test"));
        }

        [Test]
        public void ValueResolverContext_Resolves_Without_Registered_Context()
        {
            var content = new PublishedContentMock();

            var model = content.As<MyValueResolverModel>();

            Assert.That(model.MyProperty, Is.EqualTo("Default value"));
        }
    }
}