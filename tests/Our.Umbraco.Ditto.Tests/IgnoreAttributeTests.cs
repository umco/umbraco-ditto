using NUnit.Framework;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Processors")]
    public class IgnoreAttributeTests
    {
        public class MyModel
        {
            [DittoIgnore]
            public string MyProperty { get; set; }
        }

        public string PropertyValue { get; set; }

        public IPublishedContent PublishedContent { get; set; }

        [TestFixtureSetUp]
        public void Init()
        {
            this.PropertyValue = "foo bar";
            this.PublishedContent = new Mocks.PublishedContentMock
            {
                Properties = new[] { new Mocks.PublishedContentPropertyMock("myProperty", this.PropertyValue) }
            };
        }

        [Test]
        public void IgnoreAttribute_Property_Is_Ignored()
        {
            var model = this.PublishedContent.As<MyModel>();

            Assert.That(model.MyProperty, Is.Not.EqualTo(this.PropertyValue));
        }

        [Test]
        public void IgnoreAttribute_Property_With_Set_Value()
        {
            var value = "hello world";

            var model = new MyModel { MyProperty = value };

            this.PublishedContent.As(instance: model);

            Assert.That(model.MyProperty, Is.EqualTo(value));
        }

        [Test]
        public void IgnoreAttribute_Property_Value_Set_Post_Conversion()
        {
            var value = "hello world";

            var model = this.PublishedContent.As<MyModel>();

            // The concern here was that setting the value of an ignored property may cause a runtime error.
            // See @BarryFogarty's comment on this GitHub issue:
            // https://github.com/mattbrailsford/umbraco-ditflo/pull/8#issuecomment-157417916
            model.MyProperty = value;

            Assert.That(model.MyProperty, Is.EqualTo(value));
        }
    }
}