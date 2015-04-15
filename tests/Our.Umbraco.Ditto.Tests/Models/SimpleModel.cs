namespace Our.Umbraco.Ditto.Tests.Models
{
    public class SimpleModel
    {
        public string Name { get; set; }

        [UmbracoProperty("myprop")]
        public string MyProperty { get; set; }
    }
}