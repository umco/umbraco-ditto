using System;
using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Mapping"), Category("Processing")]
    public class CurrentContentTests
    {
        public class MyModel
        {
            [CurrentContentAs]
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
            [CurrentContentAs]
            public MyCircularReferenceModel MyCircularReferenceProperty { get; set; }
        }

        public void CurrentContent_Property_Mapped()
        {
            var content = new PublishedContentMock
            {
                Properties = new[]
                {
                    new PublishedContentPropertyMock("metaTitle", "This is the meta title"),
                    new PublishedContentPropertyMock("metaDescription", "This is the meta description"),
                    new PublishedContentPropertyMock("metaKeywords", "these,are,meta,keywords")
                }
            };

            var model = content.As<MyModel>();

            Assert.That(model, Is.Not.Null);
            Assert.That(model.MetaData1, Is.Not.Null);
            Assert.That(model.MetaData1, Is.TypeOf<MyMetaDataModel>());
            Assert.That(model.MetaData2, Is.Null);
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