namespace Our.Umbraco.Ditto.Tests
{
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;
    using global::Umbraco.Web;

    [TestFixture]
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
                AltText = ctx.Content.GetPropertyValue("prop1") + " " + ctx.Content.GetPropertyValue("prop2");
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
                Model.AltText2 = Content.GetPropertyValue("prop1") + " " + Content.GetPropertyValue("prop2");
            }
        }

        public class CalculatedModel2ConversionHandler : DittoConversionHandler<CalculatedModel2>
        {
            public override void OnConverted()
            {
                Model.AltText2 = Content.GetPropertyValue("prop1") + " " + Content.GetPropertyValue("prop2");
            }
        }

        #endregion

        [Test]
        public void Conversion_Handlers_Can_Run()
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

            Assert.That(model.Name, Is.EqualTo("Test"));
            Assert.That(model.AltText, Is.EqualTo("Test1 Test2"));
            Assert.That(model.AltText2, Is.EqualTo("Test1 Test2"));
        }

        [Test]
        public void Conversion_Handlers_Can_Run_Global()
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