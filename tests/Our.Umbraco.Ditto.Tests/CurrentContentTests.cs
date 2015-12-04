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
            [CurrentContentProcessor]
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
            [CurrentContentProcessor]
            public MyCircularReferenceModel MyCircularReferenceProperty { get; set; }
        }

        [Test]
        public void CurrentContent_Property_Mapped()
        {
            var metaTitle = new PublishedContentPropertyMock
            {
                Alias = "metaTitle",
                Value = "This is the meta title"
            };
            var metaDescription = new PublishedContentPropertyMock
            {
                Alias = "metaDescription",
                Value = "This is the meta description"
            };
            var metaKeywords = new PublishedContentPropertyMock
            {
                Alias = "metaKeywords",
                Value = "these,are,meta,keywords"
            };

            var content = new PublishedContentMock
            {
                Properties = new[] { metaTitle, metaDescription, metaKeywords }
            };

            var model = content.As<MyModel>();

            Assert.That(model, Is.Not.Null);
            Assert.That(model.MetaData1, Is.Not.Null, "We expect the property to be populated.");
            Assert.That(model.MetaData1, Is.TypeOf<MyMetaDataModel>());
            Assert.That(model.MetaData2, Is.Null, "We expect the property not to be populated.");
        }

        [Test]
        public void CurrentContent_InfineLoop_Check()
        {
            var content = new PublishedContentMock();

            TestDelegate code = () => { content.As<MyCircularReferenceModel>(); };

            Assert.Throws<InvalidOperationException>(code);
        }
    }
}