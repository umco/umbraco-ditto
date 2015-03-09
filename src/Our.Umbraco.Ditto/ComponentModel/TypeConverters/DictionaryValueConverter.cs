namespace Our.Umbraco.Ditto
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    public class DictionaryValueConverter : TypeConverterWithAttribute<UmbracoDictionaryValueAttribute>
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
                return ConverterHelper.UmbracoHelper.GetDictionaryValue(dictionaryValueAttribute.DictionaryKey);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}