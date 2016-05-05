using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Processors")]
    public class CaseSensitivityTests
    {
        public class MyModel
        {
            public string Name { get; set; }

            [UmbracoProperty("Name")]
            public string Name_PascalCase { get; set; }

            [UmbracoProperty("NAME")]
            public string Name_UpperCase { get; set; }

            [UmbracoProperty("name")]
            public string Name_LowerCase { get; set; }

            [UmbracoProperty("NaMe")]
            public string Name_MixedCase { get; set; }
        }

        [Test]
        public void CaseSensitivePropertyName_Mapped()
        {
            var name = "MyCustomName";
            var content = new MockPublishedContent { Name = name };

            var model = content.As<MyModel>();

            Assert.That(model.Name, Is.EqualTo(name));
            Assert.That(model.Name_PascalCase, Is.EqualTo(name));
            Assert.That(model.Name_UpperCase, Is.EqualTo(name));
            Assert.That(model.Name_LowerCase, Is.EqualTo(name));
            Assert.That(model.Name_MixedCase, Is.EqualTo(name));
        }
    }
}