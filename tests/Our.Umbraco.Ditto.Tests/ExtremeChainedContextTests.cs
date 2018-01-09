using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    public class ExtremeChainedContextTests
    {
        /*
         * This unit-test is being used to demonstrate a more complex Ditto context chain.
         * The example here is that we have an content node that contains a property (containing other nodes), think MNTP value.
         * The processor chain is setup to...
         *   1. get nodes from the property
         *   2. then use a factory to look for the corresponding POCO class
         *   3. then call our custom processor
         * The key part is that then the custom processor is called, the Value is evaluated and corresponding processors are then executed.
         * The issue we found during development is that the context was being switched between processors and not set back to the previous context.
         */

        public class MyModel
        {
            [UmbracoProperty(Order = 1)]
            [DittoDocTypeFactory(AllowedTypes = new[] { typeof(MyDifferentModel) }, Order = 2)]
            [MyProcessor(Order = 3)]
            public IEnumerable<MyDifferentModel> MyProperty { get; set; }
        }

        public class MyDifferentModel
        {
            [CurrentContentAs]
            public IPublishedContent Self { get; set; }
        }

        public class MyProcessorAttribute : DittoProcessorAttribute
        {
            public override object ProcessValue()
            {
                Assert.That(this.Context.PropertyInfo.Name, Is.EqualTo("MyProperty"));

                var items = Value as IEnumerable;
                foreach (var item in items)
                {
                    // do nothing, we just need the for-loop so that the collection is evaluated
                }

                Assert.That(this.Context.PropertyInfo.Name, Is.EqualTo("MyProperty"));

                return Value;
            }
        }

        [Test]
        public void ChainedContext_Returns()
        {
            var items = Enumerable.Repeat<IPublishedContent>(new MockPublishedContent() { DocumentTypeAlias = "MyDifferentModel" }, 1);

            var content = new MockPublishedContent
            {
                Properties = new[] { new MockPublishedContentProperty("MyProperty", items) }
            };

            var model = content.As<MyModel>();

            Assert.That(model, Is.Not.Null);
        }
    }
}