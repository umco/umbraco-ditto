namespace Our.Umbraco.Ditto.Tests.Models
{
    using global::Umbraco.Core.Models;

    public class BasicPageViewModel
    {
        public BasicPageViewModel(IPublishedContent content)
        {
            Seo = content.As<SeoViewModel>();
        }

        public SeoViewModel Seo { get; set; }
    }

    public class SeoViewModel
    {
        [UmbracoProperty("metaTitle")]
        public string PageTitle { get; set; }

        [UmbracoProperty("metaDescription")]
        public string MetaDescription { get; set; }

        [UmbracoProperty("metaKeywords")]
        public string MetaKeywords { get; set; }
    }
}