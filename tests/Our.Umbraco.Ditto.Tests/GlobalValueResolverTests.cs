using System;
using System.Collections.Generic;
using System.Globalization;
using Umbraco.Core;
using Umbraco.Web.Media.EmbedProviders.Settings;

namespace Our.Umbraco.Ditto.Tests
{
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;

    [TestFixture]
    public class GlobalValueResolverTests
    {
        public class MyValueResolverModel
        {
            public string MyStrProperty { get; set; }

            public int MyIntProperty { get; set; }
        }

        public class MyIntValueResolverAttr : DittoValueResolverAttribute
        {
            public MyIntValueResolverAttr()
                : base(typeof(MyIntValueResolver))
            { }

            public int AttrProp { get; set; }
        }

        public class MyIntValueResolver : DittoValueResolver<DittoValueResolverContext, MyIntValueResolverAttr>
        {
            public override object ResolveValue()
            {
                return Attribute.AttrProp;
            }
        }

        public class MyStrValueResolver : DittoValueResolver
        {
            public override object ResolveValue()
            {
                return "Test";
            }
        }

        [Test]
        public void Global_Value_Converter_Resolves()
        {
            Ditto.RegisterValueResolver<string, MyStrValueResolver>();
            Ditto.RegisterValueResolverAttribute<int, MyIntValueResolverAttr>(new MyIntValueResolverAttr { AttrProp = 5 });

            var content = new PublishedContentMock();

            var model = content.As<MyValueResolverModel>();

            Assert.That(model.MyStrProperty, Is.EqualTo("Test"));
            Assert.That(model.MyIntProperty, Is.EqualTo(5));
        }
    }
}