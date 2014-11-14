namespace Our.Umbraco.Ditto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Models.PublishedContent;

    /// <summary>
    /// The Ditto published content model factory for creating strong typed models.
    /// </summary>
    public class DittoPublishedContentModelFactory : IPublishedContentModelFactory
    {
        /// <summary>
        /// The type converter cache.
        /// </summary>
        private readonly Dictionary<string, Func<IPublishedContent, IPublishedContent>> converterCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="DittoPublishedContentModelFactory"/> class.
        /// </summary>
        /// <param name="types">
        /// The <see cref="IEnumerable{Type}"/> to register for creation.
        /// </param>
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
                {
                    converters.Add(typeName, func);
                }
            }

            this.converterCache = converters.Count > 0 ? converters : null;
        }

        /// <summary>
        /// Creates a strongly-typed model representing a published content.
        /// </summary>
        /// <param name="content">The original published content.</param>
        /// <returns>
        /// The strongly-typed model representing the published content, or the published content
        /// itself it the factory has no model for that content type.
        /// </returns>
        public IPublishedContent CreateModel(IPublishedContent content)
        {
            if (this.converterCache == null)
            {
                return content;
            }

            var contentTypeAlias = content.DocumentTypeAlias;
            Func<IPublishedContent, IPublishedContent> converter;

            if (!this.converterCache.TryGetValue(contentTypeAlias, out converter))
            {
                return content;
            }

            // HACK: [LK:2014-07-24] Using the `RequestCache` to store the result, as the model-factory can be called multiple times per request.
            // The cache should only contain models have a corresponding converter, so in theory the result should be the same.
            // Reason for caching, is that Ditto uses reflection to set property values, this can be a performance hit (especially when called multiple times).
            return (IPublishedContent)ApplicationContext.Current.ApplicationCache.RequestCache.GetCacheItem(
                string.Concat("DittoPublishedContentModelFactory.CreateModel_", content.Path),
                () =>
                {
                    return converter(content);
                });
        }
    }
}