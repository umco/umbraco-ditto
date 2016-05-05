using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Processors")]
    public class InheritedClassWithPrefixedPropertyTests
    {
        public class BasicModel
        {
            public string MyProperty { get; set; }
        }

        [UmbracoProperties(Prefix = "inherited_")]
        public class InheritedModel : BasicModel
        { }

        [Test]
        public void InheritedClassWithPrefixedProperty_Mapping()
        {
            var value = "foo bar";
            var content = new PublishedContentMock()
            {
                Properties = new[] { new PublishedContentPropertyMock("inherited_myProperty", value) }
            };

            var model = content.As<InheritedModel>();

            Assert.That(model, Is.Not.Null);
            Assert.That(model.MyProperty, Is.EqualTo(value));
        }
    }
}