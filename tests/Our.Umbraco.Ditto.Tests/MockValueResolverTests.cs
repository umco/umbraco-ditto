namespace Our.Umbraco.Ditto.Tests
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;

    [TestFixture]
    public class MockValueResolverTests
    {
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
        public class MockValueAttribute : DittoValueResolverAttribute
        {
            public MockValueAttribute(object rawValue)
                : base(typeof(MockValueResolver))
            {
                this.RawValue = rawValue;
            }

            public object RawValue { get; set; }
        }

        public class MockValueResolver : DittoValueResolver<DittoValueResolverContext, MockValueAttribute>
        {
            public override object ResolveValue()
            {
                return Attribute.RawValue;
            }
        }

        public class MyMockValueModel
        {
            [MockValue("Mock Property Value")]
            public string MyProperty { get; set; }
        }

        [Test]
        public void MockValue_Property_Resolved()
        {
            var content = new PublishedContentMock();

            var model = content.As<MyMockValueModel>();

            Assert.That(model.MyProperty, Is.EqualTo("Mock Property Value"));
        }
    }
}