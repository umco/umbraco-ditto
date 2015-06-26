namespace Our.Umbraco.Ditto.Tests.Models
{
    using global::Umbraco.Web;

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

    public class BaseCalculatedModel
    {
        [DittoIgnore]
        public string AltText { get; set; }

        [DittoOnConverting]
        internal void BaseCalculatedModel_OnConverting(DittoConversionHandlerContext ctx)
        {
            AltText = ctx.Content.GetPropertyValue("prop1") + " " +
                ctx.Content.GetPropertyValue("prop2");
        }
    }

    public class CalculatedModelConversionHandler : DittoConversionHandler<CalculatedModel>
    {
        public CalculatedModelConversionHandler(DittoConversionHandlerContext ctx) 
            : base(ctx)
        { }

        public override void OnConverted()
        {
            Model.AltText2 = Content.GetPropertyValue("prop1") + " " +
                Content.GetPropertyValue("prop2");
        }
    }

    public class CalculatedModel2ConversionHandler : DittoConversionHandler<CalculatedModel2>
    {
        public CalculatedModel2ConversionHandler(DittoConversionHandlerContext ctx)
            : base(ctx)
        { }

        public override void OnConverted()
        {
            Model.AltText2 = Content.GetPropertyValue("prop1") + " " +
                Content.GetPropertyValue("prop2");
        }
    }
}
