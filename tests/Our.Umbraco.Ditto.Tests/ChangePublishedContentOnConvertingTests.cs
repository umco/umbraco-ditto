using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Mapping"), Category("ConversionHandlers")]
    public class ChangePublishedContentOnConvertingTests
    {
        [DittoConversionHandler(typeof(MyModelConversionHandler))]
        public class MyModel
        {
            public string Name { get; set; }
        }

        public class MyModelConversionHandler : DittoConversionHandler<MyModel>
        {
            public override void OnConverting()
            {
                Content = new MockPublishedContent { Name = "Different" };
            }
        }

        [Test]
        public void ChangePublishedContent_OnConverting_IsMapped()
        {
            var content = new MockPublishedContent { Name = "Original" };

            var model = content.As<MyModel>();

            Assert.That(model.Name, Is.EqualTo("Different"));
        }
    }
}