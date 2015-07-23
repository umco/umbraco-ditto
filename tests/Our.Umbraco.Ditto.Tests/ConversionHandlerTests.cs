using NUnit.Framework;

using Our.Umbraco.Ditto.Tests.Mocks;
using Our.Umbraco.Ditto.Tests.Models;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    public class ConversionHandlerTests
    {
        [Test]
        public void Can_Run_Conversion_Handlers()
        {
            var content = new PublishedContentMock
            {
                Properties = new[]
                {
                    new PublishedContentPropertyMock
                    {
                        Alias = "prop1",
                        Value = "Test1"
                    },
                    new PublishedContentPropertyMock
                    {
                        Alias = "prop2",
                        Value = "Test2"
                    }
                }
            };

            var model = content.As<CalculatedModel>();

            Assert.That(model.AltText, Is.EqualTo("Test1 Test2"));
            Assert.That(model.Name, Is.EqualTo("Test"));
            Assert.That(model.AltText2, Is.EqualTo("Test1 Test2"));
        }

        [Test]
        public void Can_Run_Global_Conversion_Handlers()
        {
            var content = new PublishedContentMock
            {
                Properties = new[]
                {
                    new PublishedContentPropertyMock
                    {
                        Alias = "prop1",
                        Value = "Test1"
                    },
                    new PublishedContentPropertyMock
                    {
                        Alias = "prop2",
                        Value = "Test2"
                    }
                }
            };

            Ditto.RegisterConversionHandler<CalculatedModel2, CalculatedModel2ConversionHandler>();

            var model = content.As<CalculatedModel2>();

            Assert.That(model.AltText2, Is.EqualTo("Test1 Test2"));
        }
    }
}
