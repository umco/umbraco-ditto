namespace Our.Umbraco.Ditto
{
    using System.ComponentModel;

    using global::Umbraco.Core.Models;
    using global::Umbraco.Web;

    public class UmbracoDictionaryValueResolver : DittoValueResolver<UmbracoDictionaryAttribute>
    {
        public override object ResolveValue(ITypeDescriptorContext context, UmbracoDictionaryAttribute attribute, System.Globalization.CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(attribute.DictionaryKey))
                return null;

            var content = context.Instance as IPublishedContent;

            // HACK: [LK:2015-04-14] Resorting to using `UmbracoHelper`, as `CultureDictionaryFactoryResolver` isn't public in v6.2.x.
            return new UmbracoHelper(UmbracoContext.Current, content).GetDictionaryValue(attribute.DictionaryKey);
        }
    }
}