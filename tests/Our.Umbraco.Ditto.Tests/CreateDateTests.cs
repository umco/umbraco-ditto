using System;
using System.Collections.Generic;
using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    public class CreateDateTests
    {
        public class MyModel
        {
            [UmbracoProperty("createDate")]
            public DateTime MyProperty1 { get; set; }

            [UmbracoProperty("pubDate", "createDate")]
            public DateTime MyProperty2 { get; set; }

            [UmbracoProperty("articleDate", "createDate")]
            public DateTime MyProperty3 { get; set; }
        }

        [Test]
        public void CreateDate_Returned()
        {
            var date = DateTime.Now;

            var content = new PublishedContentMock { CreateDate = date };

            var model = content.As<MyModel>();

            Assert.That(model.MyProperty1, Is.EqualTo(date));
        }

        [Test]
        public void CreateDate_Fallback()
        {
            var date = DateTime.Now;
            var otherDate = date.AddDays(2);

            var content = new PublishedContentMock
            {
                CreateDate = date,
                Properties = new List<IPublishedContentProperty>
                {
                    new PublishedContentPropertyMock("pubDate", null, false),
                    new PublishedContentPropertyMock("articleDate", otherDate, true)
                }
            };

            var model = content.As<MyModel>();

            Assert.That(model.MyProperty2, Is.EqualTo(date));
            Assert.That(model.MyProperty3, Is.EqualTo(otherDate));
            Assert.That(model.MyProperty3, !Is.EqualTo(date));
        }
    }
}