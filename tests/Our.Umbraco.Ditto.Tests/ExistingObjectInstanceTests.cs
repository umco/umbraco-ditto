namespace Our.Umbraco.Ditto.Tests
{
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;

    [TestFixture]
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
            [UmbracoProperty("prop3")]
            public string MyProperty3 { get; set; }
        }

        [Test]
        public void Existing_Object_Mapped()
        {
            var content = new PublishedContentMock();

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
            var property = new PublishedContentPropertyMock { Alias = "prop3", Value = propertyValue };
            var content = new PublishedContentMock { Properties = new[] { property } };

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
            var property = new PublishedContentPropertyMock { Alias = "prop3", Value = propertyValue };
            var content = new PublishedContentMock { Properties = new[] { property } };

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