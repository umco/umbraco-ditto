namespace Our.Umbraco.Ditto.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;
    using global::Umbraco.Core;
    using global::Umbraco.Web.Media.EmbedProviders.Settings;

    [TestFixture]
    public class GlobalValueResolverTests
    {
        public class MyValueResolverModel
        {
            public MyStringModel MyStrProperty { get; set; }

            public MyIntModel MyIntProperty { get; set; }
        }

        public class MyStringModel
        {
            public string Value { get; set; }
        }

        public class MyIntModel
        {
            public int Value { get; set; }
        }

        public class MyIntValueResolverAttr : DittoValueResolverAttribute
        {
            public MyIntValueResolverAttr()
                : base(typeof(MyIntValueResolver))
            {
            }

            public int AttrProp { get; set; }
        }

        public class MyIntValueResolver : DittoValueResolver<DittoValueResolverContext, MyIntValueResolverAttr>
        {
            public override object ResolveValue()
            {
                return new MyIntModel { Value = Attribute.AttrProp };
            }
        }

        public class MyStrValueResolver : DittoValueResolver
        {
            public override object ResolveValue()
            {
                return new MyStringModel { Value = "Test" };
            }
        }

        [Test]
        public void Global_Value_Converter_Resolves()
        {
            Ditto.RegisterValueResolver<MyStringModel, MyStrValueResolver>();
            Ditto.RegisterValueResolverAttribute<MyIntModel, MyIntValueResolverAttr>(new MyIntValueResolverAttr { AttrProp = 5 });

            var content = new PublishedContentMock();

            var model = content.As<MyValueResolverModel>();

            Assert.That(model.MyStrProperty.Value, Is.EqualTo("Test"));
            Assert.That(model.MyIntProperty.Value, Is.EqualTo(5));
        }
    }
}