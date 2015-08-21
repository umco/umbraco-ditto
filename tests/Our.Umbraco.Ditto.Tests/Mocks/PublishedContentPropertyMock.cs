namespace Our.Umbraco.Ditto.Tests.Mocks
{
    using System;
    using global::Umbraco.Core.Models;

    public class PublishedContentPropertyMock : IPublishedContentProperty
    {
        public PublishedContentPropertyMock()
        {
            HasValue = true;
            Alias = "alias";
            Value = null;
        }

        public PublishedContentPropertyMock(string alias, object value, bool hasValue)
        {
            HasValue = hasValue;
            Alias = alias;
            Value = value;
        }

        public string PropertyTypeAlias { get; set; }

        public bool HasValue { get; set; }

        public object DataValue { get; set; }

        public object Value { get; set; }

        public object XPathValue { get; set; }

        public string Alias { get; set; }

        public Guid Version { get; set; }
    }
}