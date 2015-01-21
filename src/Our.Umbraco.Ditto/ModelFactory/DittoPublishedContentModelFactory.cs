namespace Our.Umbraco.Ditto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Models.PublishedContent;
    using global::Umbraco.Web;

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
                // Fixes possible compiler issues caused by accessing closure in loop.
                var innerType = type;
                Func<IPublishedContent, IPublishedContent> func = x => x.As(innerType) as IPublishedContent;

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
            // HACK: [LK:2014-12-04] It appears that when a Save & Publish is performed in the back-office, the model-factory's `CreateModel` is called.
            // This can cause a null-reference exception in specific cases, as the `UmbracoContext.PublishedContentRequest` might be null.
            // Ref: https://github.com/leekelleher/umbraco-ditto/issues/14
            if (UmbracoContext.Current == null || UmbracoContext.Current.PublishedContentRequest == null)
            {
                return content;
            }

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

            return converter(content);
        }
    }
}