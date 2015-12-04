namespace Our.Umbraco.Ditto.Tests
{
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;

    [TestFixture]
    public class CaseSensitivityTests
    {
        public class MyModel
        {
            public string Name { get; set; }

            [UmbracoPropertyProcessor("Name")]
            public string Name_PascalCase { get; set; }

            [UmbracoPropertyProcessor("NAME")]
            public string Name_UpperCase { get; set; }

            [UmbracoPropertyProcessor("name")]
            public string Name_LowerCase { get; set; }

            [UmbracoPropertyProcessor("NaMe")]
            public string Name_MixedCase { get; set; }
        }

        [Test]
        public void CaseSensitivePropertyName_Mapped()
        {
            var name = "MyCustomName";

            var content = new PublishedContentMock
            {
                Name = name
            };

            var model = content.As<MyModel>();

            Assert.That(model.Name, Is.EqualTo(name));
            Assert.That(model.Name_PascalCase, Is.EqualTo(name));
            Assert.That(model.Name_UpperCase, Is.EqualTo(name));
            Assert.That(model.Name_LowerCase, Is.EqualTo(name));
            Assert.That(model.Name_MixedCase, Is.EqualTo(name));
        }
    }
}