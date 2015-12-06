using System;

namespace Our.Umbraco.Ditto.Tests
{
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;

    [TestFixture]
    public class ProcessorContextTests
    {
        public class MyValueResolverModel
        {
            [MyProcessor]
            public string MyProperty { get; set; }
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