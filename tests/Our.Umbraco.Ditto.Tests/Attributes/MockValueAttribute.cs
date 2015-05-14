namespace Our.Umbraco.Ditto.Tests.Attributes
{
    using System;

    using Our.Umbraco.Ditto.Tests.ValueResolvers;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MockValueAttribute : DittoValueResolverAttribute
    {
        public MockValueAttribute(object rawValue)
            : base(typeof(MockValueResolver))
        {
            this.RawValue = rawValue;
        }

        public object RawValue { get; set; }
    }
}