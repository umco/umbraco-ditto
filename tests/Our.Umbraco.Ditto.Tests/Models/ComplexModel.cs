namespace Our.Umbraco.Ditto.Tests.Models
{
    using System.ComponentModel;

    using Our.Umbraco.Ditto.Tests.TypeConverters;
    using Our.Umbraco.Ditto.Tests.Attributes;
    using Our.Umbraco.Ditto.Tests.ValueResolvers;
    using global::Umbraco.Core.Models;

    public class ComplexModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [UmbracoProperty("myprop")]
        public IPublishedContent MyProperty { get; set; }

        [UmbracoProperty("Id")]
        [TypeConverter(typeof(MockPublishedContentConverter))]
        public IPublishedContent MyPublishedContent { get; set; }

        [MockValue("Mock Property Value")]
        public string MyResolvedProperty { get; set; }
    }
}