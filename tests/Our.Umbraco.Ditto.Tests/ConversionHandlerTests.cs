using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Web;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("ConversionHandlers")]
    public class ConversionHandlerTests
    {
        #region Models

        public class BaseCalculatedModel
        {
            [DittoIgnore]
            public string AltText { get; set; }

            [DittoOnConverting]
            internal void BaseCalculatedModel_OnConverting(DittoConversionHandlerContext ctx)
            {
                AltText = string.Format("{0} {1}", ctx.Content.GetPropertyValue("prop1"), ctx.Content.GetPropertyValue("prop2"));
            }
        }

        [DittoConversionHandler(typeof(CalculatedModelConversionHandler))]
        public class CalculatedModel : BaseCalculatedModel
        {
            public string Name { get; set; }

            [DittoIgnore]
            public string AltText2 { get; set; }

            [DittoOnConverted]
            internal void CalculatedModel_OnConverted(DittoConversionHandlerContext ctx)
            {
                Name = "Test";
            }
        }

        public class CalculatedModel2 : BaseCalculatedModel
        {
            [DittoIgnore]
            public string AltText2 { get; set; }
        }

        #endregion

        #region Conversion Handlers

        public class CalculatedModelConversionHandler : DittoConversionHandler<CalculatedModel>
        {
            public override void OnConverted()
            {
                Context.Model.AltText2 = string.Format("{0} {1}", Context.Content.GetPropertyValue("prop1"), Context.Content.GetPropertyValue("prop2"));
            }
        }

        public class CalculatedModel2ConversionHandler : DittoConversionHandler<CalculatedModel2>
        {
            public override void OnConverted()
            {
                Context.Model.AltText2 = string.Format("{0} {1}", Context.Content.GetPropertyValue("prop1"), Context.Content.GetPropertyValue("prop2"));
            }
        }

        #endregion

        [Test]
        public void Conversion_Handlers_Can_Run()
        {
            var content = new MockPublishedContent
            {
                Properties = new[]
                {
                    new MockPublishedContentProperty("prop1","Test1"),
                    new MockPublishedContentProperty("prop2","Test2")
                }
            };

            var model = content.As<CalculatedModel>();

            Assert.That(model.Name, Is.EqualTo("Test"));
            Assert.That(model.AltText, Is.EqualTo("Test1 Test2"));
            Assert.That(model.AltText2, Is.EqualTo("Test1 Test2"));
        }

        [Test]
        public void Conversion_Handlers_Can_Run_Global()
        {
            var content = new MockPublishedContent
            {
                Properties = new[]
                {
                    new MockPublishedContentProperty("prop1","Test1"),
                    new MockPublishedContentProperty("prop2","Test2")
                }
            };

            Ditto.RegisterConversionHandler<CalculatedModel2, CalculatedModel2ConversionHandler>();

            var model = content.As<CalculatedModel2>();

            Assert.That(model.AltText2, Is.EqualTo("Test1 Test2"));
        }
    }
}