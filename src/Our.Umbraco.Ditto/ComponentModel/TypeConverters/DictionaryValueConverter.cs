namespace Our.Umbraco.Ditto
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;

    public class DictionaryValueConverter : TypeConverterWithAttribute<UmbracoDictionaryValueAttribute>
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (context != null && context.PropertyDescriptor != null && context.PropertyDescriptor.Attributes != null)
            {
                var attributes = context.PropertyDescriptor.Attributes.OfType<UmbracoDictionaryValueAttribute>();

                if (attributes.Any())
                {
                    var dictionaryValueAttribute = attributes.Cast<UmbracoDictionaryValueAttribute>().First();

                    return ConverterHelper.UmbracoHelper.GetDictionaryValue(dictionaryValueAttribute.DictionaryKey);
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}