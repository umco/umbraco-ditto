using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Moq;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.Ditto.Tests.Mocks
{
    /// <summary>
    /// This class will implement all the methods needed to mock the behavior of an IPublishedContent node.
    /// Add to the constructor as more data is needed.
    /// </summary>
    public class MockPublishedContent : IPublishedContent
    {
        private readonly string _contentTypeAlias;

        public MockPublishedContent()
        {
            Id = 1234;
            Name = "Name";
            _contentTypeAlias = "contentType";
            Children = Enumerable.Empty<IPublishedContent>();
            Properties = new Collection<IPublishedProperty>();
        }

        public MockPublishedContent(
            int id,
            string name,
            string contentTypeAlias,
            IEnumerable<IPublishedContent> children,
            ICollection<IPublishedProperty> properties)
        {
            Properties = properties;
            Id = id;
            Name = name;
            Children = children;
            _contentTypeAlias = contentTypeAlias;
        }

        public int GetIndex()
        {
            throw new NotImplementedException();
        }

        public IPublishedProperty GetProperty(string alias)
        {
            return GetProperty(alias, false);
        }

        public IPublishedProperty GetProperty(string alias, bool recurse)
        {
            var prop = Properties.FirstOrDefault(p => string.Equals(p.Alias, alias, StringComparison.InvariantCultureIgnoreCase));

            if (prop == null && recurse && Parent != null)
            {
                return ((MockPublishedContent)Parent).GetProperty(alias, true);
            }

            return prop;
        }

        public IEnumerable<IPublishedContent> ContentSet { get; set; }

        private PublishedContentType _contentType;
        public PublishedContentType ContentType
        {
            get
            {
                if (_contentType != null)
                    return _contentType;

                _contentType = new PublishedContentType(0, _contentTypeAlias, PublishedItemType.Content, Enumerable.Empty<string>(), Enumerable.Empty<PublishedPropertyType>(),
                    ContentVariation.Nothing);

                return _contentType;
            }
            set
            {
                _contentType = value;
            }
        }

        public Guid Key { get; }

        public string GetUrl(string culture = null)
        {
            throw new NotImplementedException();
        }

        public PublishedCultureInfo GetCulture(string culture = null)
        {
            throw new NotImplementedException();
        }

        public int Id { get; set; }

        public int TemplateId { get; set; }

        public string UrlSegment { get; }

        public int SortOrder { get; set; }

        public string Name { get; set; }

        public string UrlName { get; set; }

        public string DocumentTypeAlias { get; set; }

        public int DocumentTypeId { get; set; }

        public string WriterName { get; set; }

        public string CreatorName { get; set; }

        public int WriterId { get; set; }

        public int CreatorId { get; set; }

        public string Path { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public Guid Version { get; set; }

        public int Level { get; set; }

        public string Url { get; set; }
        public IReadOnlyDictionary<string, PublishedCultureInfo> Cultures { get; }

        public PublishedItemType ItemType { get; set; }

        public bool IsDraft { get; set; }

        public IPublishedContent Parent { get; set; }

        public IEnumerable<IPublishedContent> Children { get; set; }

        public IEnumerable<IPublishedProperty> Properties { get; set; }
    }
}