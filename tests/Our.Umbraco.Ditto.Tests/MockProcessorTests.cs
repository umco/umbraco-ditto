using System;
using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Processors")]
    public class MockProcessorTests
    {
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
        public class MockProcessorAttribute : DittoProcessorAttribute
        {
            public object RawValue { get; set; }

            public MockProcessorAttribute(object rawValue)
            {
                RawValue = rawValue;
            }

            public override object ProcessValue()
            {
                return RawValue;
            }
        }

        public class MyMockValueModel
        {
            [MockProcessor("Mock Property Value")]
            public string MyProperty { get; set; }
        }

        [Test]
        public void MockValue_Property_Resolved()
        {
            var content = new MockPublishedContent();

            var model = content.As<MyMockValueModel>();

            Assert.That(model.MyProperty, Is.EqualTo("Mock Property Value"));
        }
    }
}