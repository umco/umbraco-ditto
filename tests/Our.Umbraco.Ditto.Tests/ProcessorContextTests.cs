using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Processors")]
    public class ProcessorContextTests
    {
        public class MyValueResolverModel
        {
            [MyProcessor]
            public string MyProperty { get; set; }

            [MyProcessor]
            public virtual string MyLazyProperty { get; set; }
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
        public void ProcessorContext_Resolves()
        {
            var value = "Test";
            var content = new MockPublishedContent();
            var context = new MyProcessorContext { MyContextProperty = value };

            var model = content.As<MyValueResolverModel>(processorContexts: new[] { context });

            Assert.That(model.MyProperty, Is.EqualTo(value));
            Assert.That(model.MyLazyProperty, Is.EqualTo(model.MyProperty));
        }

        [Test]
        public void ProcessorContext_Resolves_Without_Registered_Context()
        {
            var content = new MockPublishedContent();

            var model = content.As<MyValueResolverModel>();

            Assert.That(model.MyProperty, Is.EqualTo("Default value"));
            Assert.That(model.MyLazyProperty, Is.EqualTo(model.MyProperty));
        }
    }
}