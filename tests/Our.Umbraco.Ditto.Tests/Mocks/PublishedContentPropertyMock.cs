namespace Our.Umbraco.Ditto.Tests.Mocks
{
    using global::Umbraco.Core.Models;

    public class PublishedContentPropertyMock : IPublishedProperty
    {
        public PublishedContentPropertyMock()
        {
            HasValue = true;
            PropertyTypeAlias = "alias";
            Value = null;
        }

        public PublishedContentPropertyMock(string alias, object value, bool hasValue)
        {
            HasValue = hasValue;
            PropertyTypeAlias = alias;
            Value = value;
        }

        public string PropertyTypeAlias { get; set; }

        public bool HasValue { get; set; }

        public object DataValue { get; set; }

        public object Value { get; set; }

        public object XPathValue { get; set; }
    }
}