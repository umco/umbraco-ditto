using Our.Umbraco.Ditto.Attributes;
using Our.Umbraco.Ditto.ComponentModel.OnConvertedHandlers;
using Umbraco.Web;

namespace Our.Umbraco.Ditto.Tests.Models
{
    [DittoOnConverted(typeof(CalculatedModelOnConvertedHandler))]
    public class CalculatedModel : BaseCalculatedModel
    {
        public string Name { get; set; }

        [DittoIgnore]
        public string AltText2 { get; set; }

        [DittoOnConverted]
        internal void CalculatedModel_OnConverted(ConvertedTypeEventArgs e)
        {
            Name = "Test";
        }
    }

    public class BaseCalculatedModel
    {
        [DittoIgnore]
        public string AltText { get; set; }

        [DittoOnConverted]
        internal void BaseCalculatedModel_OnConverted(ConvertedTypeEventArgs e)
        {
            AltText = e.Content.GetPropertyValue("prop1") + " " +
                e.Content.GetPropertyValue("prop2");
        }
    }

    public class CalculatedModelOnConvertedHandler : DittoOnConvertedHandler<CalculatedModel>
    {
        public CalculatedModelOnConvertedHandler(ConvertedTypeEventArgs e) 
            : base(e)
        { }

        public override void OnConverted()
        {
            Converted.AltText2 = Content.GetPropertyValue("prop1") + " " +
                Content.GetPropertyValue("prop2");
        }
    }
}
