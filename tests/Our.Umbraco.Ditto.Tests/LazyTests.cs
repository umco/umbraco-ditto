using System;
using System.Collections.Generic;
using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Mapping")]
    public class LazyTests
    {
        public class LazyModel0
        {
            [DittoLazy] // Should error
            public string Name { get; set; }

            public int Number { get; set; }
        }

        public class LazyModel1
        {
            [DittoLazy]
            public virtual string Name { get; set; }

            public int Number { get; set; }
        }

        [DittoLazy]
        public class LazyModel2
        {
            public virtual string Name { get; set; }

            public int Number { get; set; }
        }

        public class LazyModel4 // Will test using Ditto.LazyLoadStrategy
        {
            public virtual string Name { get; set; }

            public int Number { get; set; }
        }

        private string name = "MyName";
        private int number = 10;
        private IPublishedContent mockContent;

        [SetUp]
        public void Setup()
        {
            mockContent = new MockPublishedContent
            {
                Name = name,
                Properties = new List<IPublishedProperty>
                {
                    new MockPublishedContentProperty("number", number)
                }
            };
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Non_Virtual_Lazy_Should_Throw()
        {
            var model = mockContent.As<LazyModel0>();
        }

        [Test]
        public void Property_Level_Lazy_Should_Map()
        {
            var model = mockContent.As<LazyModel1>();
            var proxy = model as IProxy;

            Assert.IsNotNull(model);
            Assert.IsNotNull(proxy);
            Assert.That(model.Name, Is.EqualTo(name));
            Assert.That(model.Number, Is.EqualTo(number));
        }

        [Test]
        public void Class_Level_Lazy_Should_Map()
        {
            var model = mockContent.As<LazyModel2>();
            var proxy = model as IProxy;

            Assert.IsNotNull(model);
            Assert.IsNotNull(proxy);
            Assert.That(model.Name, Is.EqualTo(name));
            Assert.That(model.Number, Is.EqualTo(number));
        }

        [Test]
        public void Global_Level_Lazy_Should_Map()
        {
            Ditto.LazyLoadStrategy = LazyLoad.AllVirtuals;

            var model = mockContent.As<LazyModel4>();
            var proxy = model as IProxy;

            Assert.IsNotNull(model);
            Assert.IsNotNull(proxy);
            Assert.That(model.Name, Is.EqualTo(name));
            Assert.That(model.Number, Is.EqualTo(number));

            Ditto.LazyLoadStrategy = LazyLoad.AttributedVirtuals;
        }
    }
}