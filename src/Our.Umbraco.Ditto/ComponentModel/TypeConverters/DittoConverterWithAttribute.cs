namespace Our.Umbraco.Ditto
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    public abstract class DittoConverterWithAttribute<T> : DittoConverter where T : Attribute
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

        public T GetAttribute(ITypeDescriptorContext context)
        {
            if (context != null && context.PropertyDescriptor != null && context.PropertyDescriptor.Attributes != null)
            {
                var attributes = context.PropertyDescriptor.Attributes.OfType<UmbracoDictionaryValueAttribute>();

                if (attributes.Any())
                {
                    return attributes.FirstOrDefault() as T;
                }
            }

            return default(T);
        }
    }
}