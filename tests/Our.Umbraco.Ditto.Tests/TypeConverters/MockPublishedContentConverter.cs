namespace Our.Umbraco.Ditto.Tests.TypeConverters
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    using Our.Umbraco.Ditto.Tests.Mocks;

    public class MockPublishedContentConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(int) || sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            int id = 0;

            if (value is string)
            {
                int.TryParse((string)value, out id);
            }

            if (value is int)
            {
                id = (int)value;
            }

            return ContentBuilder.Default()
                .WithId(id)
                .WithName("Mock Published Content")
                .Build();
        }
    }
}