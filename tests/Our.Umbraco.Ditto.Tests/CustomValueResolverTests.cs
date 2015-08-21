namespace Our.Umbraco.Ditto.Tests
{
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;

    [TestFixture]
    public class CustomValueResolverTests
    {
        public class MyModel
        {
            [DittoValueResolver(typeof(MyCustomValueResolver))]
            public string Name { get; set; }
        }

        public class MyCustomValueResolver : DittoValueResolver
        {
            public override object ResolveValue()
            {
                return (Content != null) ? Content.Name + " Test" : null;
            }
        }

        [Test]
        public void Custom_ValueResolver_Resolves()
        {
            var content = new PublishedContentMock()
            {
                Name = "MyName"
            };

            var model = content.As<MyModel>();

            Assert.That(model.Name, Is.EqualTo("MyName Test"));
        }
    }
}