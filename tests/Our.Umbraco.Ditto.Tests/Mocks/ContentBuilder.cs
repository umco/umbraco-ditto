using System.Collections.Generic;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto.Tests.Mocks
{
	/// <summary>
	/// The builder gives a nice default item, with methods for overriding what you want.
	/// Extend to set up your items with more data and properties.
	/// </summary>
	public class ContentBuilder
	{
		private int _id = 1234;
		private string _name = "Name";
		private List<IPublishedContent> _children = new List<IPublishedContent>();
		private List<IPublishedContentProperty> _properties = new List<IPublishedContentProperty>();

		public static ContentBuilder Default()
		{
			return new ContentBuilder();
		}

		public ContentBuilder WithId(int id)
		{
			_id = id;
			return this;
		}

		public ContentBuilder WithName(string name)
		{
			_name = name;
			return this;
		}

		public ContentBuilder AddProperty(IPublishedContentProperty property)
		{
			_properties.Add(property);
			return this;
		}

		public ContentBuilder AddChildren(IEnumerable<IPublishedContent> children)
		{
			_children.AddRange(children);
			return this;
		}

		public ContentBuilder AddChild(IPublishedContent child)
		{
			_children.Add(child);
			return this;
		}

		public IPublishedContent Build()
		{
			return new PublishedContentMock(_id, _name, _children, _properties);
		}

	}
}