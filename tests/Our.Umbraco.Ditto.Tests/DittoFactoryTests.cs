﻿using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    public class DittoFactoryTests
    {
        public interface IMyModel
        {
            string Name { get; set; }
            string MyProperty { get; set; }
        }

        public class MyModel1 : IMyModel
        {
            public string Name { get; set; }
            public string MyProperty { get; set; }
        }

        public class MyModel2 : IMyModel
        {
            public string Name { get; set; }
            public string MyProperty { get; set; }
        }

        public class MyMainModel
        {
            [UmbracoProperty(Order = 0)]
            [DittoDocTypeFactory(Order = 1)]
            public IEnumerable<IMyModel> MyCollection { get; set; }

            [UmbracoProperty(Order = 0)]
            [DittoDocTypeFactory(Order = 1)]
            public IMyModel MyProp { get; set; } 
        }

        [Test]
        public void Factory_Resolves()
        {
            var content1 = new PublishedContentMock
            {
                Name = "Content 1",
                DocumentTypeAlias = "MyModel1",
                Properties = new List<IPublishedContentProperty>
                {
                    new PublishedContentPropertyMock("MyProperty", "My Property 1", true)
                }
            };

            var content2 = new PublishedContentMock
            {
                Name = "Content 2",
                DocumentTypeAlias = "MyModel2",
                Properties = new List<IPublishedContentProperty>
                {
                    new PublishedContentPropertyMock("MyProperty", "My Property 2", true)
                }
            };

            var content3 = new PublishedContentMock
            {
                Name = "Content 3",
                DocumentTypeAlias = "MyModel3",
                Properties = new List<IPublishedContentProperty>
                {
                    new PublishedContentPropertyMock("MyProperty", "My Property 3", true)
                }
            };


            var content = new PublishedContentMock
            {
                Properties = new List<IPublishedContentProperty>
                {
                    new PublishedContentPropertyMock("MyCollection", new [] { content1, content2, content3  }, true),
                    new PublishedContentPropertyMock("MyProp", content1, true)
                }
            };

            var model = content.As<MyMainModel>();

            Assert.That(model.MyCollection.Count(), Is.EqualTo(3));

            var items = model.MyCollection.ToList();

            Assert.That(items[0].GetType(), Is.EqualTo(typeof(MyModel1)));
            Assert.That(items[1].GetType(), Is.EqualTo(typeof(MyModel2)));
            Assert.Null(items[2]);

            Assert.NotNull(model.MyProp);
            Assert.That(model.MyProp.GetType(), Is.EqualTo(typeof(MyModel1)));
        }
    }
}