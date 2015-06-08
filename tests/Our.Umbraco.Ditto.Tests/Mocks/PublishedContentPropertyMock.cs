namespace Our.Umbraco.Ditto.Tests.Mocks
{
    using System;
    using global::Umbraco.Core.Models;

    public class PublishedContentPropertyMock : IPublishedContentProperty
    {
        public PublishedContentPropertyMock(string alias, object value, bool hasValue)
        {
            HasValue = hasValue;
            Alias = alias;
            Value = value;
        }

        public string PropertyTypeAlias { get; private set; }

        public bool HasValue { get; private set; }

        public object DataValue { get; private set; }

        public object Value { get; private set; }

        public object XPathValue { get; private set; }

        public string Alias { get; private set; }

        public Guid Version { get; private set; }
    }
}