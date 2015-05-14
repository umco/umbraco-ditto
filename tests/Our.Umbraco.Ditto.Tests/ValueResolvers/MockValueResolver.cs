namespace Our.Umbraco.Ditto.Tests.ValueResolvers
{
    using System.ComponentModel;
    using System.Globalization;
    using Our.Umbraco.Ditto.Tests.Attributes;

    public class MockValueResolver : DittoValueResolver<MockValueAttribute>
    {
        public override object ResolveValue(ITypeDescriptorContext context, MockValueAttribute attribute, CultureInfo culture)
        {
            return attribute.RawValue;
        }
    }
}