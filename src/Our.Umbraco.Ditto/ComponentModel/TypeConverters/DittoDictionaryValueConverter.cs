namespace Our.Umbraco.Ditto
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    using global::Umbraco.Web;

    public class DittoDictionaryValueConverter : DittoConverterWithAttribute<UmbracoDictionaryValueAttribute>
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var dictionaryValueAttribute = this.GetAttribute(context);

            if (dictionaryValueAttribute != null)
            {
                // HACK: [LK:2015-04-14] Resorting to using `UmbracoHelper`, as `CultureDictionaryFactoryResolver` isn't public in v6.2.x.
                return new UmbracoHelper(UmbracoContext.Current).GetDictionaryValue(dictionaryValueAttribute.DictionaryKey);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}