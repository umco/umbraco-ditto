using System.Collections.ObjectModel;
using NUnit.Framework;

namespace Our.Umbraco.Ditto.Tests.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Models.PublishedContent;

    /// <summary>
    /// This class will implement all the methods needed to moch the behaviour of an IPublishedContent node.
    /// Add to the constructor as more data is needed.
    /// </summary>
    public class PublishedContentMock : IPublishedContent
    {
        public PublishedContentMock()
        {
            Properties = new Collection<IPublishedContentProperty>();
            Children = new List<IPublishedContent>();
        }

        public PublishedContentMock(
            int id,
            string name,
            IPublishedContent _parent,
            IEnumerable<IPublishedContent> children,
            ICollection<IPublishedContentProperty> properties)
        {
            Properties = properties;
            Id = id;
            Name = name;
            Parent = Parent;
            Children = children;
        }

        public int GetIndex()
        {
            throw new NotImplementedException();
        }

        public IPublishedContentProperty GetProperty(string alias)
        {
            return GetProperty(alias, false);
        }

        public IPublishedContentProperty GetProperty(string alias, bool recurse)
        {
            var prop = Properties.SingleOrDefault(p => p.Alias.InvariantEquals(alias));
            if (prop == null && recurse && Parent != null)
            {
                return Parent.GetProperty(alias, recurse);
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

        public ICollection<IPublishedContentProperty> Properties { get; set; }

        public object this[string alias]
        {
            get
            {
                return GetProperty(alias);
            }
        }
    }
}