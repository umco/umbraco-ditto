namespace Our.Umbraco.Ditto.Tests
{
    using System;
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;

    [TestFixture]
    public class BasicMappingTests
    {
        public class BasicModel
        {
            public string Name { get; set; }
        }

        public class BasicModelWithProperty
        {
            public string Name { get; set; }

            [UmbracoProperty("myprop")]
            public string MyProperty { get; set; }
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
        public void Basic_Property_Returned()
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

            var model = content.As<BasicModelWithProperty>();

            Assert.That(model.MyProperty, Is.EqualTo(value));
        }

        [Test]
        public void Basic_Property_To_String_Exception()
        {
            // The source is an `IPublishedContent`, the target is a `string`, type mismatch exception
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

            TestDelegate code = () =>
            {
                // We are passing an `IPublishedContent` object to a property (of type `string`),
                // so we know that internally Ditto will try calling `content.As<string>()`,
                // which will throw an `InvalidOperationException` exception.
                var model = content.As<BasicModelWithProperty>();
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