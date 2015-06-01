namespace Our.Umbraco.Ditto
{
    using System.ComponentModel;
    using System.Globalization;

    using global::Umbraco.Core.Models;

    public class CurrentContentValueResolver : DittoValueResolver<CurrentContentAttribute>
    {
        public override object ResolveValue(ITypeDescriptorContext context, CurrentContentAttribute attribute, CultureInfo culture)
        {
            return context.Instance as IPublishedContent;
        }
    }
}