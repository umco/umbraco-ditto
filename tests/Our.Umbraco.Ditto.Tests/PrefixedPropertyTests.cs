namespace Our.Umbraco.Ditto.Tests
{
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;

    [TestFixture]
    public class PrefixedPropertyTests
    {
        [UmbracoPropertiesProcessor(Prefix = "Site", Recursive = true)]
        public class MyModel
        {
            public string Name { get; set; }

            public string Description { get; set; }

            public string Fallback { get; set; }

            [UmbracoPropertyProcessor("UnprefixedProp")]
            public string UnprefixedProp { get; set; }
        }

        [Test]
        public void Prefixed_Properties_Can_Resolve()
        {
            var prop1 = new PublishedContentPropertyMock
            {
                Alias = "siteName",
                Value = "Name"
            };
            var prop2 = new PublishedContentPropertyMock
            {
                Alias = "siteDescription",
                Value = "Description"
            };
            var prop3 = new PublishedContentPropertyMock
            {
                Alias = "fallback",
                Value = "Fallback"
            };

            var content = new PublishedContentMock
            {
                Properties = new[] { prop1, prop2, prop3 }
            };

            var converted = content.As<MyModel>();

            Assert.That(converted.Name, Is.EqualTo("Name"));
            Assert.That(converted.Description, Is.EqualTo("Description"));
            Assert.That(converted.Fallback, Is.EqualTo("Fallback"));
        }

        [Test]
        public void Prefixed_Properties_Can_UmbracoPropertyAttribute_Override()
        {
            var prop1 = new PublishedContentPropertyMock
            {
                Alias = "siteUnprefixedProp",
                Value = "Site Unprefixed"
            };
            var prop2 = new PublishedContentPropertyMock
            {
                Alias = "unprefixedProp",
                Value = "Unprefixed"
            };

            var content = new PublishedContentMock
            {
                Properties = new[] { prop1, prop2 }
            };

            var converted = content.As<MyModel>();

            Assert.That(converted.UnprefixedProp, Is.EqualTo("Unprefixed"));
        }

        [Test]
        public void Prefixed_Properties_Can_Resolve_Recursive()
        {
            var childContent = new PublishedContentMock();

            var parentContent = new PublishedContentMock
            {
                Properties = new[]
                {
                    new PublishedContentPropertyMock
                    {
                        Alias = "siteDescription",
                        Value = "Description"
                    }
                },
                Children = new[]
                {
                    childContent
                }
            };

            childContent.Parent = parentContent;

            var converted = childContent.As<MyModel>();

            Assert.That(converted.Description, Is.EqualTo("Description"));
        }
    }
}