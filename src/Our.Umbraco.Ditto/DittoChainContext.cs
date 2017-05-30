using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The chain collection of processor contexts.
    /// </summary>
    public class DittoChainContext
    {
        /// <summary>
        /// The collection of processor contexts.
        /// </summary>
        public ProcessorContextsCollection ProcessorContexts { get; }

        internal DittoChainContext(IEnumerable<DittoProcessorContext> processorContexts)
        {
            ProcessorContexts = new ProcessorContextsCollection(processorContexts);
        }

        internal DittoChainContext()
            : this(null)
        { }
    }

    /// <summary>
    /// A collection of processor contexts for the chain.
    /// </summary>
    public class ProcessorContextsCollection
    {
        private ConcurrentDictionary<Type, DittoProcessorContext> _processorContexts;

        internal ProcessorContextsCollection()
        {
            _processorContexts = new ConcurrentDictionary<Type, DittoProcessorContext>();
        }

        internal ProcessorContextsCollection(IEnumerable<DittoProcessorContext> processorContexts)
        {
            _processorContexts = new ConcurrentDictionary<Type, DittoProcessorContext>();

            if (processorContexts != null)
                AddRange(processorContexts);
        }

        /// <summary>
        /// Adds a range of processor contexts to the collection chain.
        /// </summary>
        /// <param name="ctxs">An enumerable of processor contexts.</param>
        public void AddRange(IEnumerable<DittoProcessorContext> ctxs)
        {
            if (ctxs == null)
            {
                return;
            }

            foreach (var ctx in ctxs)
            {
                Add(ctx);
            }
        }

        /// <summary>
        /// Adds a processor context to the collection chain.
        /// </summary>
        /// <param name="ctx">The processor context.</param>
        public void Add(DittoProcessorContext ctx)
        {
            _processorContexts.AddOrUpdate(ctx.GetType(), ctx, (type, ctx2) => ctx2); // Don't override if already exists
        }

        /// <summary>
        /// Gets or creates the <see cref="DittoProcessorContext" />.
        /// </summary>
        /// <param name="baseContext">The base context.</param>
        /// <param name="contextType">The object type of the content.</param>
        /// <returns>Returns the the <see cref="DittoProcessorContext" />.</returns>
        public DittoProcessorContext GetOrCreate(DittoProcessorContext baseContext, Type contextType)
        {
            // Get, clone and populate the relevant context for the given level
            var ctx = _processorContexts
                .GetOrAdd(contextType, type => (DittoProcessorContext)contextType.GetInstance())
                .Clone()
                .Populate(baseContext);

            return ctx;
        }
    }
}