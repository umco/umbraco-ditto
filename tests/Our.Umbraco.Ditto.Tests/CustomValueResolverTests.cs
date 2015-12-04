using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    public class CustomProcessorTests
    {
        public class MyModel
        {
            [DittoProcessor(typeof(MyCustomProcessor))]
            public string Name { get; set; }
        }

        public class MyCustomProcessor : DittoProcessor<IPublishedContent>
        {
            public override object ProcessValue()
            {
                return (Value != null) ? Value.Name + " Test" : null;
            }
        }

        [Test]
        public void Custom_Processor_Processes()
        {
            var content = new PublishedContentMock()
            {
                Name = "MyName"
            };

            var model = content.As<MyModel>();

            Assert.That(model.Name, Is.EqualTo("MyName Test"));
        }
    }
}