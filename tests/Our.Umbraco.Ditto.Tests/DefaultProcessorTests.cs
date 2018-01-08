using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Processors")]
    public class DefaultProcessorTests
    {
        public class MyModel
        {
            public string Name { get; set; }
        }

        [DittoDefaultProcessor(typeof(MyCustomProcessorAttribute))]
        public class MyCustomModel
        {
            public string Name { get; set; }
        }

        public class MyCustomProcessorAttribute : DittoProcessorAttribute
        {
            public override object ProcessValue()
            {
                return "MyCustomName";
            }
        }

        public class MyGlobalProcessorAttribute : DittoProcessorAttribute
        {
            public override object ProcessValue()
            {
                return "MyGlobalName";
            }
        }

        private IPublishedContent Content;

        [TestFixtureSetUp]
        public void Init()
        {
            Content = new MockPublishedContent { Name = "MyName" };
        }

        [Test]
        public void Default_Processor_IsUsed()
        {
            var model = Content.As<MyModel>();

            Assert.That(model.Name, Is.EqualTo("MyName"));
        }

        [Test]
        public void Default_Processor_ClassLevel_IsUsed()
        {
            var model = Content.As<MyCustomModel>();

            Assert.That(model.Name, Is.EqualTo("MyCustomName"));
        }

        [Test]
        public void Default_Processor_GlobalLevel_IsUsed()
        {
            Ditto.RegisterDefaultProcessorType<MyGlobalProcessorAttribute>();

            var model = Content.As<MyModel>();

            Assert.That(model.Name, Is.EqualTo("MyGlobalName"));

            // Tidy-up after test, otherwise the new default processor will cause other tests to fail!
            Ditto.RegisterDefaultProcessorType<UmbracoPropertyAttribute>();

            // ...and clear the type-config cache
            DittoTypeConfigCache.Clear<MyModel>();
        }
    }
}