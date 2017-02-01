using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Our.Umbraco.Ditto
{
    public class DittoChainContext
    {
        public ProcessorContextsCollection ProcessorContexts { get; }

        internal DittoChainContext(IEnumerable<DittoProcessorContext> processorContexts)
        {
            ProcessorContexts = new ProcessorContextsCollection(processorContexts);
        }

        internal DittoChainContext()
            : this(null)
        { }
    }

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

        public void Add(DittoProcessorContext ctx)
        {
            _processorContexts.AddOrUpdate(ctx.GetType(), ctx, (type, ctx2) => ctx2); // Don't override if already exists
        }

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
