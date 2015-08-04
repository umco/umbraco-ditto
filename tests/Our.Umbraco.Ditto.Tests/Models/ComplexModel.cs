namespace Our.Umbraco.Ditto.Tests.Models
{
    using System.ComponentModel;
    using global::Umbraco.Core.Models;
    using Our.Umbraco.Ditto.Tests.TypeConverters;

    public class ComplexModel
    {
        public int Id { get; set; }

        [UmbracoProperty("myprop")]
        public IPublishedContent MyProperty { get; set; }

        [UmbracoProperty("Id")]
        [TypeConverter(typeof(MockPublishedContentConverter))]
        public IPublishedContent MyPublishedContent { get; set; }
    }
}