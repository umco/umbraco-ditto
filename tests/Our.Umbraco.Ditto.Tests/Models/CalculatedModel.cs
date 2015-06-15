using System;
using Our.Umbraco.Ditto.Attributes;
using Our.Umbraco.Ditto.ComponentModel;
using Our.Umbraco.Ditto.ComponentModel.ConversionHandlers;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Ditto.Tests.Models
{
    [DittoConversionHandler(typeof(CalculatedModelOnConvertedHandler))]
    public class CalculatedModel : BaseCalculatedModel
    {
        public string Name { get; set; }

        [DittoIgnore]
        public string AltText2 { get; set; }

        [DittoOnConverted]
        internal void CalculatedModel_OnConverted(ConversionHandlerContext ctx)
        {
            Name = "Test";
        }
    }

    public class BaseCalculatedModel
    {
        [DittoIgnore]
        public string AltText { get; set; }

        [DittoOnConverting]
        internal void BaseCalculatedModel_OnConverting(ConversionHandlerContext ctx)
        {
            AltText = ctx.Content.GetPropertyValue("prop1") + " " +
                ctx.Content.GetPropertyValue("prop2");
        }
    }

    public class CalculatedModelOnConvertedHandler : DittoConversionHandler<CalculatedModel>
    {
        public CalculatedModelOnConvertedHandler(ConversionHandlerContext ctx) 
            : base(ctx)
        { }

        public override void OnConverted()
        {
            Model.AltText2 = Content.GetPropertyValue("prop1") + " " +
                Content.GetPropertyValue("prop2");
        }
    }
}
