using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Processors")]
    public class PrefixedPropertyTests
    {
        [UmbracoProperties(Prefix = "Site", Recursive = true)]
        public class MyModel
        {
            public string Name { get; set; }

            public string Description { get; set; }

            public string Fallback { get; set; }

            [UmbracoProperty("UnprefixedProp")]
            public string UnprefixedProp { get; set; }
        }

        [Test]
        public void Prefixed_Properties_Can_Resolve()
        {
            var value1 = "Name";
            var value2 = "Description";
            var value3 = "Fallback";

            var content = new PublishedContentMock
            {
                Properties = new[]
                {
                    new PublishedContentPropertyMock("siteName", value1),
                    new PublishedContentPropertyMock("siteDescription", value2),
                    new PublishedContentPropertyMock("fallback", value3)
                }
            };

            var converted = content.As<MyModel>();

            Assert.That(converted.Name, Is.EqualTo(value1));
            Assert.That(converted.Description, Is.EqualTo(value2));
            Assert.That(converted.Fallback, Is.EqualTo(value3));
        }

        [Test]
        public void Prefixed_Properties_Can_UmbracoPropertyAttribute_Override()
        {
            var value1 = "Site Unprefixed";
            var value2 = "Unprefixed";

            var content = new PublishedContentMock
            {
                Properties = new[]
                {
                    new PublishedContentPropertyMock("siteUnprefixedProp", value1),
                    new PublishedContentPropertyMock("unprefixedProp", value2)
                }
            };

            var converted = content.As<MyModel>();

            Assert.That(converted.UnprefixedProp, Is.EqualTo(value2));
        }

        [Test]
        public void Prefixed_Properties_Can_Resolve_Recursive()
        {
            var value = "Description";

            var childContent = new PublishedContentMock();

            var parentContent = new PublishedContentMock
            {
                Properties = new[] { new PublishedContentPropertyMock("siteDescription", value) },
                Children = new[] { childContent }
            };

            childContent.Parent = parentContent;

            var converted = childContent.As<MyModel>();

            Assert.That(converted.Description, Is.EqualTo(value));
        }
    }
}