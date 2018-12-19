using System;
using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Mapping")]
    public class PocoConstructorTests
    {
        public class MyModel
        {
            public MyModel() { }
        }

        public class MyModel<T>
        {
            public MyModel(T arg) { }
        }

        [TestCase(typeof(MyModel))]
        [TestCase(typeof(MyModel<IPublishedContent>))]
        [TestCase(typeof(MyModel<string>), ExpectedException = typeof(InvalidOperationException))]
        public void PocoConstructor_Tests(Type modelType)
        {
            var content = new MockPublishedContent();

            var model = content.As(modelType);

            Assert.That(model, Is.Not.Null);
            Assert.That(model, Is.InstanceOf(modelType));
        }
    }
}