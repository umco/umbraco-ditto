using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.Ditto
{
	public class DittoPublishedContentModelFactory : IPublishedContentModelFactory
	{
		private readonly Dictionary<string, Func<IPublishedContent, IPublishedContent>> _converters;

		public DittoPublishedContentModelFactory(IEnumerable<Type> types)
		{
			var converters = new Dictionary<string, Func<IPublishedContent, IPublishedContent>>(StringComparer.InvariantCultureIgnoreCase);

			foreach (var type in types.Where(x => typeof(IPublishedContent).IsAssignableFrom(x)))
			{
				Func<IPublishedContent, IPublishedContent> func = (x) =>
				{
					return x.As(type) as IPublishedContent;
				};

				var attribute = type.GetCustomAttribute<PublishedContentModelAttribute>(false);
				var typeName = attribute == null ? type.Name : attribute.ContentTypeAlias;

				if (!converters.ContainsKey(typeName))
					converters.Add(typeName, func);
			}

			_converters = converters.Count > 0 ? converters : null;
		}

		public IPublishedContent CreateModel(IPublishedContent content)
		{
			if (_converters == null)
				return content;

			var contentTypeAlias = content.DocumentTypeAlias;

			Func<IPublishedContent, IPublishedContent> converter;
			return _converters.TryGetValue(contentTypeAlias, out converter)
				? converter(content)
				: content;
		}
	}
}