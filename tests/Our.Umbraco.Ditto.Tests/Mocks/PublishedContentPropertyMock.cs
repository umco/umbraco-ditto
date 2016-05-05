using System;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto.Tests.Mocks
{
    {
        public PublishedContentPropertyMock()
        { }

        public PublishedContentPropertyMock(string alias, object value)
        {
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