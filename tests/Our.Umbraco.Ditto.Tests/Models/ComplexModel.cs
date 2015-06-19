namespace Our.Umbraco.Ditto.Tests.Models
{
    using System.ComponentModel;

    using Our.Umbraco.Ditto.Tests.TypeConverters;
    using global::Umbraco.Core.Models;

    public class ComplexModel
    {
        public int Id { get; set; }

        [DittoValueResolver(typeof(NameValueResovler))]
        public string Name { get; set; }

        [UmbracoProperty("myprop")]
        public IPublishedContent MyProperty { get; set; }

        [UmbracoProperty("Id")]
        [TypeConverter(typeof(MockPublishedContentConverter))]
        public IPublishedContent MyPublishedContent { get; set; }

        [AppSetting("MyAppSettingKey")]
        public string MyAppSettingProperty { get; set; }
    }

    public class NameValueResovler : DittoValueResolver
    {
        public override object ResolveValue()
        {
            return (Content != null) ? Content.Name + " Test" : null;
        }
    }
}