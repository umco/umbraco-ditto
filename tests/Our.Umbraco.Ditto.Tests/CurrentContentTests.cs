namespace Our.Umbraco.Ditto.Tests
{
    using System;
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;

    [TestFixture]
    public class CurrentContentTests
    {
        public class MyModel
        {
            [CurrentContent]
            public MyMetaDataModel MetaData1 { get; set; }

            public MyMetaDataModel MetaData2 { get; set; }
        }

        public class MyMetaDataModel
        {
            public string MetaTitle { get; set; }

            public string MetaDescription { get; set; }

            public string MetaKeywords { get; set; }
        }

        public class MyCircularReferenceModel
        {
            [CurrentContent]
            public MyCircularReferenceModel MyCircularReferenceProperty { get; set; }
        }

        [Test]
        public void CurrentContent_Property_Mapped()
        {
            var metaTitle = PropertyBuilder.Default("metaTitle", "This is the meta title").Build();
            var metaDescription = PropertyBuilder.Default("metaDescription", "This is the meta description").Build();
            var metaKeywords = PropertyBuilder.Default("metaKeywords", "these,are,meta,keywords").Build();

            var content = ContentBuilder.Default()
                .AddProperty(metaTitle)
                .AddProperty(metaDescription)
                .AddProperty(metaKeywords)
                .Build();

            var model = content.As<MyModel>();

            Assert.That(model, Is.Not.Null);
            Assert.That(model.MetaData1, Is.Not.Null, "We expect the property to be populated.");
            Assert.That(model.MetaData1, Is.TypeOf<MyMetaDataModel>());
            Assert.That(model.MetaData2, Is.Null, "We expect the property not to be populated.");
        }

        [Test]
        public void CurrentContent_InfineLoop_Check()
        {
            var content = ContentBuilder.Default().Build();

            TestDelegate code = () => { content.As<MyCircularReferenceModel>(); };

            Assert.Throws<InvalidOperationException>(code);
        }
    }
}