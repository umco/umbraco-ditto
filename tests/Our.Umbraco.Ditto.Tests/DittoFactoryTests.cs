using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Processors")]
    public class DittoFactoryTests
    {
        public interface IMyModel
        {
            string Name { get; set; }
        }

        public class MyModel1 : IMyModel
        {
            public string Name { get; set; }
        }

        public class MyModel2 : IMyModel
        {
            public string Name { get; set; }
        }

        public class MyModel3Suffixed : IMyModel
        {
            public string Name { get; set; }
        }

        public class MyMainModel
        {
            [UmbracoProperty(Order = 0)]
            [DittoDocTypeFactory(Order = 1)]
            public IEnumerable<IMyModel> MyCollection { get; set; }

            [UmbracoProperty(Order = 0)]
            [DittoDocTypeFactory(Suffix = "Suffixed", Order = 1)]
            public IMyModel MyProperty { get; set; }
        }

        [Test]
        public void Factory_Resolves()
        {
            var content1 = new MockPublishedContent { Name = "Content 1", DocumentTypeAlias = "MyModel1" };
            var content2 = new MockPublishedContent { Name = "Content 2", DocumentTypeAlias = "MyModel2" };
            var content3 = new MockPublishedContent { Name = "Content 3", DocumentTypeAlias = "MyModel3" };

            var content = new MockPublishedContent
            {
                Properties = new List<IPublishedProperty>
                {
                    new MockPublishedContentProperty("MyCollection", new [] { content1, content2, content3  }),
                    new MockPublishedContentProperty("MyProperty", content3)
                }
            };

            var model = content.As<MyMainModel>();

            Assert.That(model.MyCollection, Is.Not.Null);

            var items = model.MyCollection.ToList();

            Assert.That(items, Has.Count.EqualTo(3));
            Assert.That(items[0], Is.TypeOf<MyModel1>());
            Assert.That(items[1], Is.TypeOf<MyModel2>());
            Assert.That(items[2], Is.Null);

            Assert.That(model.MyProperty, Is.Not.Null);
            Assert.That(model.MyProperty, Is.TypeOf<MyModel3Suffixed>());
        }
    }
}