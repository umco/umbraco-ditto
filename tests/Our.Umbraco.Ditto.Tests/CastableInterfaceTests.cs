namespace Our.Umbraco.Ditto.Tests
{
    using global::Umbraco.Core.Models;
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;

    [TestFixture]
    public class CastableInterfaceTests
    {
        [Test]
        public void Castable_Interface_Can_Map()
        {
            var content = new PublishedContentMock
            {
                Id = 123,
                Name = "Test"
            };

            var content2 = content.As<IPublishedContent>();

            Assert.That(content2.Id, Is.EqualTo(content.Id));
            Assert.That(content2.Name, Is.EqualTo(content.Name));
        }
    }
}