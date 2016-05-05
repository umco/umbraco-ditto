namespace Our.Umbraco.Ditto.Tests
{
    using Newtonsoft.Json;
    using NUnit.Framework;

    [TestFixture]
    public class ImageCropDataSetMappingTests
    {
        public class MyModel
        {
            public Models.ImageCropDataSet MyProperty { get; set; }
        }

        [Test]
        public void ImageCropDataSetMappedTest()
        {
            // JSON test data taken from Umbraco unit-test:
            // https://github.com/umbraco/Umbraco-CMS/blob/dev-v7/src/Umbraco.Tests/PropertyEditors/ImageCropperTest.cs
            var json = "{\"focalPoint\": {\"left\": 0.96,\"top\": 0.80827067669172936},\"src\": \"/media/1005/img_0671.jpg\",\"crops\": [{\"alias\":\"thumb\",\"width\": 100,\"height\": 100,\"coordinates\": {\"x1\": 0.58729977382575338,\"y1\": 0.055768992440203169,\"x2\": 0,\"y2\": 0.32457553600198386}}]}";
            var value = JsonConvert.DeserializeObject<Models.ImageCropDataSet>(json);

            Assert.That(value, Is.Not.Null);

            var property = new Mocks.PublishedContentPropertyMock("myProperty", value);
            var content = new Mocks.PublishedContentMock { Properties = new[] { property } };

            var model = content.As<MyModel>();

            Assert.That(model, Is.Not.Null);
            Assert.That(model.MyProperty, Is.Not.Null);
            Assert.That(model.MyProperty, Is.EqualTo(value));
            Assert.That(model.MyProperty.Crops, Is.Not.Null.Or.Empty);
            Assert.That(model.MyProperty.Src, Is.Not.Null.Or.Empty);
        }
    }
}