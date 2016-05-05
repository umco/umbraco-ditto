using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Processors")]
    public class CustomProcessorTests
    {
        public class MyModel
        {
            [MyCustomProcessor]
            public string Name { get; set; }
        }

        public class MyCustomProcessor : DittoProcessorAttribute
        {
            public override object ProcessValue()
            {
                var content = Value as IPublishedContent;

                return content.Name + " Test";
            }
        }

        [Test]
        public void Custom_Processor_Processes()
        {
            var content = new PublishedContentMock() { Name = "MyName" };

            var model = content.As<MyModel>();

            Assert.That(model.Name, Is.EqualTo("MyName Test"));
        }
    }
}