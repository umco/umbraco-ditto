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
        #region Interfaces

        public interface IMyModel
        {
            string Name { get; set; }
        }

        [DittoDocTypeFactory]
        public interface IMyModel2
        {
            string Name { get; set; }
        }

        #endregion

        #region IMyModel Instances

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

        #endregion

        #region IMyModel2 Instances

        public class MyModel4 : IMyModel2
        {
            public string Name { get; set; }
        }

        public class MyModel5 : IMyModel2
        {
            public string Name { get; set; }
        }

        #endregion

        public class MyMainModel
        {
            [UmbracoProperty(Order = 0)]
            [DittoDocTypeFactory(Order = 1)]
            public IEnumerable<IMyModel> MyCollection { get; set; }

            [UmbracoProperty(Order = 0)]
            [DittoDocTypeFactory(Suffix = "Suffixed", Order = 1)]
            public IMyModel MyProperty { get; set; }

            public IEnumerable<IMyModel2> MyCollection2 { get; set; }

            [UmbracoProperty(Order = 0)]
            [DittoDocTypeFactory(AllowedTypes = new [] { typeof(MyModel1) }, Order = 1)]
            public IEnumerable<IMyModel> MyCollection3 { get; set; }

            public IMyModel2 MyProperty2 { get; set; }
        }

        [Test]
        public void Factory_Resolves()
        {
            var content1 = new MockPublishedContent { Name = "Content 1", DocumentTypeAlias = "MyModel1" };
            var content2 = new MockPublishedContent { Name = "Content 2", DocumentTypeAlias = "MyModel2" };
            var content3 = new MockPublishedContent { Name = "Content 3", DocumentTypeAlias = "MyModel3" };
            var content4 = new MockPublishedContent { Name = "Content 4", DocumentTypeAlias = "MyModel4" };
            var content5 = new MockPublishedContent { Name = "Content 5", DocumentTypeAlias = "MyModel5" };

            var content = new MockPublishedContent
            {
                Properties = new List<IPublishedProperty>
                {
                    new MockPublishedContentProperty("MyCollection", new [] { content1, content2, content3  }),
                    new MockPublishedContentProperty("MyProperty", content3),
                    new MockPublishedContentProperty("MyCollection2", new [] { content4, content5  }),
                    new MockPublishedContentProperty("MyProperty2", content4),
                    new MockPublishedContentProperty("MyCollection3", new [] { content1, content2, content3  })
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

            var items2 = model.MyCollection2.ToList();

            Assert.That(items2, Has.Count.EqualTo(2));
            Assert.That(items2[0], Is.TypeOf<MyModel4>());
            Assert.That(items2[1], Is.TypeOf<MyModel5>());

            Assert.That(model.MyProperty2, Is.Not.Null);
            Assert.That(model.MyProperty2, Is.TypeOf<MyModel4>());
            Assert.That(model.MyProperty2.Name, Is.EqualTo(content4.Name));

            var items3 = model.MyCollection3.ToList();

            Assert.That(items3, Has.Count.EqualTo(3));
            Assert.That(items3[0], Is.TypeOf<MyModel1>());
            Assert.That(items3[1], Is.Null);
            Assert.That(items3[2], Is.Null);
        }
    }
}