namespace Our.Umbraco.Ditto.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class InheritedClassWithPrefixedPropertyTests
    {
        public class BasicModel
        {
            public string MyProperty { get; set; }
        }

        [UmbracoProperties(Prefix = "inherited_")]
        public class InheritedModel : BasicModel
        {
        }

        [Test]
        public void InheritedClassWithPrefixedProperty_Mapping()
        {
            var value = "foo bar";
            var property = new Mocks.PublishedContentPropertyMock("inherited_myProperty", value);
            var content = new Mocks.PublishedContentMock() { Properties = new[] { property } };

            var model = content.As<InheritedModel>();

            Assert.That(model, Is.Not.Null);
            Assert.That(model.MyProperty, Is.EqualTo(value));
        }
    }
}