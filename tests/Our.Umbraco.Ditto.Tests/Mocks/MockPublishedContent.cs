using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Umbraco.Core;
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
        public MockPublishedContent()
        {
            Id = 1234;
            Name = "Name";
            Children = Enumerable.Empty<IPublishedContent>();
            Properties = new Collection<IPublishedProperty>();
        }

        public MockPublishedContent(
            int id,
            string name,
            IEnumerable<IPublishedContent> children,
            ICollection<IPublishedProperty> properties)
        {
            Properties = properties;
            Id = id;
            Name = name;
            Children = children;
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
            var prop = Properties.SingleOrDefault(p => p.PropertyTypeAlias.InvariantEquals(alias));

            if (prop == null && recurse && Parent != null)
            {
                return Parent.GetProperty(alias, true);
            }

            return prop;
        }

        public IEnumerable<IPublishedContent> ContentSet { get; set; }

        public PublishedContentType ContentType { get; set; }

        public int Id { get; set; }

        public int TemplateId { get; set; }

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

        public PublishedItemType ItemType { get; set; }

        public bool IsDraft { get; set; }

        public IPublishedContent Parent { get; set; }

        public IEnumerable<IPublishedContent> Children { get; set; }

        public ICollection<IPublishedProperty> Properties { get; set; }

        public object this[string alias]
        {
            get
            {
                return GetProperty(alias);
            }
        }
    }
}