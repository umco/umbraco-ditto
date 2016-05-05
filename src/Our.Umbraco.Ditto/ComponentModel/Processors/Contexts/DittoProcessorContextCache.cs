using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Represents a cache of processor contexts created during a single .As call
    /// </summary>
    internal class DittoProcessorContextCache
    {
        /// <summary>
        /// The content
        /// </summary>
        private readonly IPublishedContent content;

        /// <summary>
        /// The target type
        /// </summary>
        private readonly Type targetType;

        /// <summary>
        /// The property descriptor
        /// </summary>
        private readonly PropertyDescriptor propertyDescriptor;

        /// <summary>
        /// The culture
        /// </summary>
        private readonly CultureInfo culture;

        /// <summary>
        /// The lookup
        /// </summary>
        private readonly ConcurrentDictionary<Type, DittoProcessorContext> lookup;

        /// <summary>
        /// Initializes a new instance of the <see cref="DittoProcessorContextCache" /> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <param name="culture">The culture.</param>
        public DittoProcessorContextCache(
            IPublishedContent content,
            Type targetType,
            PropertyDescriptor propertyDescriptor,
            CultureInfo culture)
        {
            this.content = content;
            this.targetType = targetType;
            this.propertyDescriptor = propertyDescriptor;
            this.culture = culture;

            this.lookup = new ConcurrentDictionary<Type, DittoProcessorContext>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DittoProcessorContextCache"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="contexts">The contexts.</param>
        public DittoProcessorContextCache(
            IPublishedContent content,
            Type targetType,
            PropertyDescriptor propertyDescriptor,
            CultureInfo culture,
            IEnumerable<DittoProcessorContext> contexts)
            : this(content, targetType, propertyDescriptor, culture)
        {
            AddContexts(contexts);
        }

        /// <summary>
        /// Adds the contexts.
        /// </summary>
        /// <param name="contexts">The contexts.</param>
        public void AddContexts(IEnumerable<DittoProcessorContext> contexts)
        {
            if (contexts == null)
            {
                return;
            }

            foreach (var ctx in contexts)
            {
                this.AddContext(ctx);
            }
        }

        /// <summary>
        /// Adds the context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void AddContext(DittoProcessorContext context)
        {
            this.lookup.AddOrUpdate(context.GetType(), context.Populate(content, targetType, propertyDescriptor, culture), (type, ctx) => ctx); // Don't override if already exists
        }

        /// <summary>
        /// Gets or creates the processor context.
        /// </summary>
        /// <param name="contexType">Type of the context.</param>
        /// <returns>Returns the Ditto processor context.</returns>
        public DittoProcessorContext GetOrCreateContext(Type contexType)
        {
            return this.lookup.GetOrAdd(
                contexType,
                type => ((DittoProcessorContext)contexType.GetInstance())
                    .Populate(this.content, this.targetType, this.propertyDescriptor, this.culture));
        }
    }
}