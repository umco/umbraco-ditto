namespace Our.Umbraco.Ditto.Tests.Models
{
    using global::Umbraco.Web;

    [DittoMapHandler(typeof(CalculatedModelConversionHandler))]
    public class CalculatedModel : BaseCalculatedModel
    {
        public string Name { get; set; }

        [DittoIgnore]
        public string AltText2 { get; set; }

        [DittoOnMapped]
        internal void CalculatedModel_OnMapped(DittoMapHandlerContext ctx)
        {
            Name = "Test";
        }
    }

    public class BaseCalculatedModel
    {
        [DittoIgnore]
        public string AltText { get; set; }

        [DittoOnMapping]
        internal void BaseCalculatedModel_OnMapping(DittoMapHandlerContext ctx)
        {
            AltText = ctx.Content.GetPropertyValue("prop1") + " " +
                ctx.Content.GetPropertyValue("prop2");
        }
    }

    public class CalculatedModelConversionHandler : DittoMapHandler<CalculatedModel>
    {
        public CalculatedModelConversionHandler(DittoMapHandlerContext ctx) 
            : base(ctx)
        { }

        public override void OnMapped()
        {
            Model.AltText2 = Content.GetPropertyValue("prop1") + " " +
                Content.GetPropertyValue("prop2");
        }
    }
}
