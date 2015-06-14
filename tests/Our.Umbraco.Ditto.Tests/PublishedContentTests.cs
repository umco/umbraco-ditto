namespace Our.Umbraco.Ditto.Tests
{
    using System.Linq;

    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;
    using Our.Umbraco.Ditto.Tests.Models;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Web;
    using System;

    [TestFixture]
    public class PublishedContentTests
    {
        [Test]
        public void Name_IsMapped()
        {
            var name = "MyCustomName";

            var content = new PublishedContentMock
            {
                Name = name
            };

            var model = content.As<SimpleModel>();

            Assert.That(model.Name, Is.EqualTo(name));
        }

        [Test]
        public void Children_Counted()
        {
            var child = new PublishedContentMock();

            var content = new PublishedContentMock
            {
                Children = new[] {child}
            };

            //Do your Ditto magic here, and assert it maps as it should
            Assert.That(content.Children.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Property_Returned()
        {
            var value = "myval";

            var property = new PublishedContentPropertyMock
            {
                Alias = "myprop",
                Value = value
            };

            var content = new PublishedContentMock
            {
                Properties = new[] { property }
            };

            var model = content.As<SimpleModel>();

            Assert.That(model.MyProperty, Is.EqualTo(value));
        }

        [Test]
        public void Property_Converted()
        {
            // With this kind of mocking, we dont need property value converters, because they would already
            // have run at this point, so we just mock the result of the conversion.

            var value = new PublishedContentMock();

            var property = new PublishedContentPropertyMock
            {
                Alias = "myprop",
                Value = value
            };

            var content = new PublishedContentMock
            {
                Properties = new[] { property }
            };

            var model = content.As<SimpleModel>();

            Assert.That(model.MyProperty, Is.Not.EqualTo(value));
            Assert.That(model.MyProperty, Is.EqualTo(value.ToString()));
        }

        [Test]
        public void Complex_Property_Convertered()
        {
            var value = new PublishedContentMock();

            var property = new PublishedContentPropertyMock
            {
                Alias = "myprop",
                Value = value
            };

            var content = new PublishedContentMock
            {
                Id = 1234,
                Properties = new[] { property }
            };

            var model = content.As<ComplexModel>();

            Assert.That(model.Id, Is.EqualTo(1234));

            Assert.That(model.MyProperty, Is.EqualTo(value));

            Assert.That(model.MyPublishedContent, Is.InstanceOf<IPublishedContent>());
            Assert.That(model.MyPublishedContent.Id, Is.EqualTo(1234));
            Assert.That(model.MyPublishedContent.Name, Is.EqualTo("Mock Published Content"));
        }

        [Test]
        public void Content_To_String()
        {
            var content = new PublishedContentMock();

            TestDelegate code = () => { content.As<string>(); };

            Assert.Throws<InvalidOperationException>(code);
        }
    }
}