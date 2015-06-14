using Our.Umbraco.Ditto.Attributes;
using Umbraco.Web;

namespace Our.Umbraco.Ditto.Tests.Models
{
    public class CalculatedModel : BaseCalculatedModel
    {
        public string Name { get; set; }

        [DittoOnConverted]
        internal void OnConverted2(ConvertedTypeEventArgs e)
        {
            Name = "Test";
        }
    }

    public class BaseCalculatedModel
    {
        [DittoIgnore]
        public string AltText { get; set; }

        [DittoOnConverted]
        internal void OnConverted(ConvertedTypeEventArgs e)
        {
            AltText = e.Content.GetPropertyValue("prop1") + " " +
                e.Content.GetPropertyValue("prop2");
        }
    }
}
