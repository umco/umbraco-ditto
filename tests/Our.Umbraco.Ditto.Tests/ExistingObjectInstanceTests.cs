using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Mapping")]
    public class ExistingObjectInstanceTests
    {
        public class MyBaseModel
        {
            public string MyProperty1 { get; set; }

            [DittoIgnore]
            public string MyProperty2 { get; set; }
        }

        public class MyInheritedModel : MyBaseModel
        {
            public string MyProperty3 { get; set; }
        }

        [Test]
        public void Existing_Object_Mapped()
        {
            var content = new MockPublishedContent();

            var value = "Hello world";
            var model = new MyBaseModel()
            {
                MyProperty1 = value,
                MyProperty2 = value
            };

            content.As(instance: model);

            Assert.That(model.MyProperty1, Is.Not.EqualTo(value));
            Assert.That(model.MyProperty2, Is.EqualTo(value));
        }

        [Test]
        public void Existing_Base_Object_Mapped()
        {
            var propertyValue = "Foo Bar";
            var content = new MockPublishedContent
            {
                Properties = new[] { new MockPublishedContentProperty("MyProperty3", propertyValue) }
            };

            var value = "Hello world";
            var model = new MyInheritedModel()
            {
                MyProperty3 = value
            };

            // down-cast the model to the base type, so the inherited type properties should not be mapped.
            content.As<MyBaseModel>(instance: model);

            Assert.That(model.MyProperty3, Is.EqualTo(value));
            Assert.That(model.MyProperty3, Is.Not.EqualTo(propertyValue));
        }

        [Test]
        public void Existing_Inherited_Object_Mapped()
        {
            var propertyValue = "Foo Bar";
            var content = new MockPublishedContent
            {
                Properties = new[] { new MockPublishedContentProperty("MyProperty3", propertyValue) }
            };

            var value = "Hello world";
            var model = new MyInheritedModel()
            {
                MyProperty3 = value
            };

            // run through Ditto, the expected behaviour is that the Umbraco property value will overwrite the previous value.
            content.As(instance: model);

            Assert.That(model.MyProperty3, Is.Not.EqualTo(value));
            Assert.That(model.MyProperty3, Is.EqualTo(propertyValue));
        }
    }
}