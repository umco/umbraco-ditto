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

            var content = new MockPublishedContent
            {
                Properties = new[]
                {
                    new MockPublishedContentProperty("siteName", value1),
                    new MockPublishedContentProperty("siteDescription", value2),
                    new MockPublishedContentProperty("fallback", value3)
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

            var content = new MockPublishedContent
            {
                Properties = new[]
                {
                    new MockPublishedContentProperty("siteUnprefixedProp", value1),
                    new MockPublishedContentProperty("unprefixedProp", value2)
                }
            };

            var converted = content.As<MyModel>();

            Assert.That(converted.UnprefixedProp, Is.EqualTo(value2));
        }

        [Test]
        public void Prefixed_Properties_Can_Resolve_Recursive()
        {
            var value = "Description";

            var childContent = new MockPublishedContent();

            var parentContent = new MockPublishedContent
            {
                Properties = new[] { new MockPublishedContentProperty("siteDescription", value) },
                Children = new[] { childContent }
            };

            childContent.Parent = parentContent;

            var converted = childContent.As<MyModel>();

            Assert.That(converted.Description, Is.EqualTo(value));
        }
    }
}