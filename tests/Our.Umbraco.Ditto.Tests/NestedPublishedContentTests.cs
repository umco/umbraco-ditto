using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    public class NestedPublishedContentTests
    {
        public class MyModel
        {
            public MyNestedModel MyContent { get; set; }

            public IEnumerable<MyNestedModel> MyContentList { get; set; }
        }

        public class MyNestedModel
        {
            public string MyProperty { get; set; }
        }

        [Test]
        public void Nested_PublishedContent_Property_Value()
        {
            var value = "My Inner Property Value";

            var content1 = new PublishedContentMock
            {
                Properties = new[]
                {
                    new PublishedContentPropertyMock( "myProperty", value)
                }
            };

            var content2 = new PublishedContentMock
            {
                Properties = new[]
                {
                    new PublishedContentPropertyMock("myContent", content1)
                }
            };

            var model = content2.As<MyModel>();

            Assert.That(model.MyContent.MyProperty, Is.EqualTo(value));
        }

        [Test]
        public void Nested_Enumerable_PublishedContent_Property_Value()
        {
            var value = "My Inner Property Value";

            var contentList = new List<IPublishedContent>();

            for (var i = 0; i < 5; i++)
            {
                contentList.Add(new PublishedContentMock
                {
                    Id = i,
                    Name = "Node " + i,
                    Properties = new[]
                    {
                        new PublishedContentPropertyMock("myProperty", value)
                    }
                });
            }

            var content2 = new PublishedContentMock
            {
                Properties = new[]
                {
                    new PublishedContentPropertyMock("myContentList", contentList)
                }
            };

            var model = content2.As<MyModel>();

            Assert.That(model.MyContentList.Count(), Is.EqualTo(5));
            Assert.That(model.MyContentList.All(x => x.MyProperty == value), Is.True);
        }
    }
}