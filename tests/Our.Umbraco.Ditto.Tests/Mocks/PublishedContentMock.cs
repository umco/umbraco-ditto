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
        public PublishedContentMock(
            int id,
            string name,
            IEnumerable<IPublishedContent> children,
            ICollection<IPublishedContentProperty> properties)
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

        public IPublishedContentProperty GetProperty(string alias)
        {
            return GetProperty(alias, false);
        }

        public IPublishedContentProperty GetProperty(string alias, bool recurse)
        {
            return Properties.SingleOrDefault(p => p.Alias.InvariantEquals(alias));
        }

        public IEnumerable<IPublishedContent> ContentSet { get; private set; }

        public PublishedContentType ContentType { get; private set; }

        public int Id { get; private set; }

        public int TemplateId { get; private set; }

        public int SortOrder { get; private set; }

        public string Name { get; private set; }

        public string UrlName { get; private set; }

        public string DocumentTypeAlias { get; private set; }

        public int DocumentTypeId { get; private set; }

        public string WriterName { get; private set; }

        public string CreatorName { get; private set; }

        public int WriterId { get; private set; }

        public int CreatorId { get; private set; }

        public string Path { get; private set; }

        public DateTime CreateDate { get; private set; }

        public DateTime UpdateDate { get; private set; }

        public Guid Version { get; private set; }

        public int Level { get; private set; }

        public string Url { get; private set; }

        public PublishedItemType ItemType { get; private set; }

        public bool IsDraft { get; private set; }

        public IPublishedContent Parent { get; private set; }

        public IEnumerable<IPublishedContent> Children { get; private set; }

        public ICollection<IPublishedContentProperty> Properties { get; private set; }

        public object this[string alias]
        {
            get
            {
                return GetProperty(alias);
            }
        }
    }
}