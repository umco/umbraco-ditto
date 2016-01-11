using NUnit.Framework;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
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

        public IPublishedContent PublishedContent { get; set; }

        [TestFixtureSetUp]
        public void Init()
        {
            var name = "MyName";
            this.PublishedContent = new Mocks.PublishedContentMock { Name = name };
        }

        [Test]
        public void Default_Processor_IsUsed()
        {
            var model = this.PublishedContent.As<MyModel>();

            Assert.That(model.Name, Is.EqualTo("MyName"));
        }

        [Test]
        public void Default_Processor_ClassLevel_IsUsed()
        {
            var model = this.PublishedContent.As<MyCustomModel>();

            Assert.That(model.Name, Is.EqualTo("MyCustomName"));
        }

        [Test]
        public void Default_Processor_GlobalLevel_IsUsed()
        {
            Ditto.RegisterDefaultProcessorType<MyGlobalProcessorAttribute>();

            var model = this.PublishedContent.As<MyModel>();

            Assert.That(model.Name, Is.EqualTo("MyGlobalName"));

            // tidy-up after test, otherwise the new default processor will cause other tests to fail!
            Ditto.RegisterDefaultProcessorType<UmbracoPropertyAttribute>();
        }
    }
}