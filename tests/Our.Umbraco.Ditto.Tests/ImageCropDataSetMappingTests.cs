using Newtonsoft.Json;
using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.Models;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Mapping")]
    public class ImageCropDataSetMappingTests
    {
        public class MyModel
        {
            public ImageCropDataSet MyProperty { get; set; }
        }

        private ImageCropDataSet Value;

        private IPublishedContent Content;

        [TestFixtureSetUp]
        public void Init()
        {
            // JSON test data taken from Umbraco unit-test:
            // https://github.com/umbraco/Umbraco-CMS/blob/dev-v7/src/Umbraco.Tests/PropertyEditors/ImageCropperTest.cs
            var json = "{\"focalPoint\": {\"left\": 0.96,\"top\": 0.80827067669172936},\"src\": \"/media/1005/img_0671.jpg\",\"crops\": [{\"alias\":\"thumb\",\"width\": 100,\"height\": 100,\"coordinates\": {\"x1\": 0.58729977382575338,\"y1\": 0.055768992440203169,\"x2\": 0,\"y2\": 0.32457553600198386}}]}";

            Value = JsonConvert.DeserializeObject<ImageCropDataSet>(json);

            Content = new MockPublishedContent
            {
                Properties = new[] { new MockPublishedContentProperty("myProperty", Value) }
            };
        }

        [Test]
        public void ImageCropDataSetMappedTest()
        {
            var model = Content.As<MyModel>();

            Assert.That(model, Is.Not.Null);
            Assert.That(model.MyProperty, Is.Not.Null);
            Assert.That(model.MyProperty, Is.EqualTo(Value));
            Assert.That(model.MyProperty.Crops, Is.Not.Null.Or.Empty);
            Assert.That(model.MyProperty.Src, Is.Not.Null.Or.Empty);
        }
    }
}