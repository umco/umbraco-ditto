using System;
using System.Linq;
using Moq;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Our.Umbraco.Ditto.Tests.Mocks
{
    public class MockPublishedContentProperty : IPublishedProperty
    {
        public MockPublishedContentProperty()
        { }

        public MockPublishedContentProperty(string alias, object value)
        {
            Alias = alias;
            Value = value;
        }

        
        bool IPublishedProperty.HasValue(string culture, string segment)
        {
            return Value != null;
        }

        public object GetSourceValue(string culture = null, string segment = null)
        {
            return Value;
        }

        public object GetValue(string culture = null, string segment = null)
        {
            return Value;
        }

        public object GetXPathValue(string culture = null, string segment = null)
        {
            throw new System.NotImplementedException();
        }

        public PublishedPropertyType PropertyType => null;

        public string Alias { get; }

        public object Value { get; set; }
    }
}