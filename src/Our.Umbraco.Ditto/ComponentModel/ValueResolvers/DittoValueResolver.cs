namespace Our.Umbraco.Ditto
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    public abstract class DittoValueResolver
    {
        public abstract object ResolveValue(ITypeDescriptorContext context, DittoValueResolverAttribute attribute, CultureInfo culture);
    }

    public abstract class DittoValueResolver<TAttributeType> : DittoValueResolver
        where TAttributeType : DittoValueResolverAttribute
    {
        public override object ResolveValue(ITypeDescriptorContext context, DittoValueResolverAttribute attribute, CultureInfo culture)
        {
            if (!(attribute is TAttributeType))
                throw new ArgumentException("The resolver attribute must be of type " + typeof(TAttributeType).AssemblyQualifiedName, "attribute");

            return ResolveValue(context, (TAttributeType)attribute, culture);
        }

        public abstract object ResolveValue(ITypeDescriptorContext context, TAttributeType attribute, CultureInfo culture);
    }
}
