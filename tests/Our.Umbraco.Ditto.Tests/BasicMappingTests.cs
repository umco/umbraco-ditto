namespace Our.Umbraco.Ditto.Tests
{
    using System;
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;
    using global::Umbraco.Core.Models;

    [TestFixture]
    public class BasicMappingTests
    {
        public class BasicModel
        {
            public string Name { get; set; }
        }

        public class BasicModelWithId
        {
            public int Id { get; set; }

            [UmbracoProperty("Id")]
            public int MyProperty { get; set; }
        }

        public class BasicModelWithStringProperty
        {
            public string MyProperty { get; set; }
        }

        public class BasicModelWithPublishedContentProperty
        {
            public IPublishedContent MyProperty { get; set; }
        }

        [Test]
        public void Basic_Name_IsMapped()
        {
            var name = "MyCustomName";

            var content = new PublishedContentMock
            {
                Name = name
            };

            var model = content.As<BasicModel>();

            Assert.That(model.Name, Is.EqualTo(name));
        }

        [Test]
        public void Basic_Id_And_Property_IsMapped()
        {
            var id = 1234;

            var content = new PublishedContentMock
            {
                Id = id
            };

            var model = content.As<BasicModelWithId>();

            Assert.That(model.Id, Is.EqualTo(id));
            Assert.That(model.MyProperty, Is.EqualTo(id));
        }

        [Test]
        public void Basic_String_Property_IsMapped()
        {
            var value = "myValue";

            var property = new PublishedContentPropertyMock
            {
                Alias = "myProperty",
                Value = value
            };

            var content = new PublishedContentMock
            {
                Properties = new[] { property }
            };

            var model = content.As<BasicModelWithStringProperty>();

            Assert.That(model.MyProperty, Is.EqualTo(value));
        }

        [Test]
        public void Basic_PublishedContent_Property_IsMapped()
        {
            var value = new PublishedContentMock();

            var property = new PublishedContentPropertyMock
            {
                Alias = "myProperty",
                Value = value
            };

            var content = new PublishedContentMock
            {
                Properties = new[] { property }
            };

            var model = content.As<BasicModelWithPublishedContentProperty>();

            Assert.That(model.MyProperty, Is.EqualTo(value));
        }

        [Test]
        public void Basic_Property_To_String_Exception()
        {
            // The source is an `IPublishedContent`, the target is a `string`, type mismatch exception
            var value = new PublishedContentMock();

            var property = new PublishedContentPropertyMock
            {
                Alias = "myProperty",
                Value = value
            };

            var content = new PublishedContentMock
            {
                Properties = new[] { property }
            };

            TestDelegate code = () =>
            {
                // We are passing an `IPublishedContent` object to a property (of type `string`),
                // so we know that internally Ditto will try calling `content.As<string>()`,
                // which will throw an `InvalidOperationException` exception.
                var model = content.As<BasicModelWithStringProperty>();
            };

            Assert.Throws<InvalidOperationException>(code);
        }

        [Test]
        public void Basic_Content_To_String_Exception()
        {
            var content = new PublishedContentMock();

            TestDelegate code = () =>
            {
                // Since a `string` does not have a parameterless constructor,
                // Ditto can not map the `IPublishedContent` object,
                // which will throw an `InvalidOperationException` exception.
                var model = content.As<string>();
            };

            Assert.Throws<InvalidOperationException>(code);
        }
    }
}