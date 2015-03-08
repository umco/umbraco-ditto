namespace Our.Umbraco.Ditto
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    public class TypeConverterWithAttribute<T> : TypeConverter where T : Attribute
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (context != null && context.PropertyDescriptor != null && context.PropertyDescriptor.Attributes != null)
            {
                return context.PropertyDescriptor.Attributes
                    .OfType<T>()
                    .Any();
            }

            return base.CanConvertFrom(context, sourceType);
        }
    }
}