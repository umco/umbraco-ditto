namespace Our.Umbraco.Ditto.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class PocoConstructorTests
    {
        public class MyModel
        {
            public MyModel() { }
        }

        public class MyModel2
        {
            public MyModel2(string arg) { }
        }

        public class MyModel3
        {
            public MyModel2 MyProperty { get; set; }
        }

        [Test]
        public void PocoConstructorEmptyTest()
        {
            var content = new Mocks.PublishedContentMock();

            var model = content.As<MyModel>();

            Assert.That(model, Is.Not.Null);
            Assert.That(model, Is.InstanceOf<MyModel>());
        }

        [Test]
        public void PocoConstructorParameterTest()
        {
            var content = new Mocks.PublishedContentMock();

            TestDelegate code = () => { var model = content.As<MyModel2>(); };

            Assert.Throws<InvalidOperationException>(code);
        }

        [Test]
        public void PocoConstructorParameterOnPropertyTest()
        {
            // For this test, the value would have already been resolved by a PropertyValueConverter to its intended type.
            var value = new MyModel2("foo");

            var property = new Mocks.PublishedContentPropertyMock("myProperty", value, true);
            var content = new Mocks.PublishedContentMock { Properties = new[] { property } };

            var model = content.As<MyModel3>();

            Assert.That(model, Is.Not.Null);
            Assert.That(model, Is.InstanceOf<MyModel3>());

            Assert.That(model.MyProperty, Is.Not.Null);
            Assert.That(model.MyProperty, Is.InstanceOf<MyModel2>());
            Assert.That(model.MyProperty, Is.EqualTo(value));
        }
    }
}