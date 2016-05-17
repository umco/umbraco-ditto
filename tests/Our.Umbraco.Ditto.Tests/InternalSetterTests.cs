using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    public class InternalSetterTests
    {
        public class MyModel
        {
            [UmbracoProperty("myProperty")]
            public string PropertyOmittedSetter { get { return null; } }

            [UmbracoProperty("myProperty")]
            public string PropertyPublicSetter { get; set; }

            [UmbracoProperty("myProperty")]
            public string PropertyInternalSetter { get; internal set; }

            [UmbracoProperty("myProperty")]
            public virtual string PropertyVirtualOmittedSetter { get { return null; } }

            [UmbracoProperty("myProperty")]
            public virtual string PropertyVirtualPublicSetter { get; set; }

            [UmbracoProperty("myProperty")]
            public virtual string PropertyVirtualInternalSetter { get; internal set; }
        }

        [Test]
        public void InternalSetter_Test()
        {
            var value = "foo";
            var content = new PublishedContentMock
            {
                Properties = new[] { new PublishedContentPropertyMock("myProperty", value, true) }
            };

            var model = content.As<MyModel>();

            Assert.That(model.PropertyOmittedSetter, Is.Null);
            Assert.That(model.PropertyPublicSetter, Is.EqualTo(value));
            Assert.That(model.PropertyInternalSetter, Is.EqualTo(value));
            Assert.That(model.PropertyVirtualOmittedSetter, Is.Null);
            Assert.That(model.PropertyVirtualPublicSetter, Is.EqualTo(value));
            Assert.That(model.PropertyVirtualInternalSetter, Is.EqualTo(value));
        }
    }
}