namespace Our.Umbraco.Ditto.Tests.Models
{
	using System.ComponentModel;
	using global::Umbraco.Core.Models;
	using Our.Umbraco.Ditto.Tests.TypeConverters;

    public class ComplexModel
    {
        public int Id { get; set; }

        [DittoValueResolver(typeof(NameValueResolver))]
        public string Name { get; set; }

        [UmbracoProperty("myprop")]
        public IPublishedContent MyProperty { get; set; }

        [UmbracoProperty("Id")]
        [TypeConverter(typeof(MockPublishedContentConverter))]
        public IPublishedContent MyPublishedContent { get; set; }
    }

    public class NameValueResolver : DittoValueResolver
    {
        public override object ResolveValue()
        {
            return (Content != null) ? Content.Name + " Test" : null;
        }
    }
}