using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    public class SetMethodTests
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
        public void SetMethod_Test()
        {
            var value = "foo";
            var content = new MockPublishedContent
            {
                Properties = new[] { new MockPublishedContentProperty("myProperty", value) }
            };

            var model = content.As<MyModel>();

            Assert.That(model.PropertyOmittedSetter, Is.Null);
            Assert.That(model.PropertyPublicSetter, Is.EqualTo(value));
            Assert.That(model.PropertyInternalSetter, Is.Null);
            Assert.That(model.PropertyVirtualOmittedSetter, Is.Null);
            Assert.That(model.PropertyVirtualPublicSetter, Is.EqualTo(value));
            Assert.That(model.PropertyVirtualInternalSetter, Is.Null);
        }
    }
}