using System.Globalization;
using Umbraco.Core;

namespace Our.Umbraco.Ditto.Tests
{
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;

    [TestFixture]
    public class ValueResolverContextTests
    {
        public class MyValueResolverModel
        {
            [DittoValueResolver(typeof(MyValueResolver))]
            public string MyProperty { get; set; }
        }

        public class MyValueResolver : DittoValueResolver<MyValueResolverContext>
        {
            public override object ResolveValue(MyValueResolverContext context, 
                DittoValueResolverAttribute attribute, CultureInfo culture)
            {
                return context.MyContextProperty;
            }
        }

        public class MyValueResolverContext : DittoValueResolverContext
        {
            public string MyContextProperty { get; set; }
        }

        [Test]
        public void Value_Converter_Context_Resolves()
        {
            var content = new PublishedContentMock();

            Ditto.RegisterValueResolverContext(new MyValueResolverContext
            {
                MyContextProperty = "Test"
            });

            var model = content.As<MyValueResolverModel>();

            Assert.That(model.MyProperty, Is.EqualTo("Test"));
        }
    }
}