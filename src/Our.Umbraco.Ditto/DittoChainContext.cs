using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Our.Umbraco.Ditto
{
    internal class DittoChainContext
    {
        private static volatile DittoChainContext instance;
        private static object syncRoot = new Object();
        private static int chainCount { get; set; }

        private DittoChainContext()
        {
            ProcessorContexts = new ProcessorContextsCollection();
        }

        public static DittoChainContext Current
        {
            get
            {
                if (instance == null)
                    throw new ApplicationException("There is no current chain context initialized, you need to call BeginChainContext before accessing the current context.");
                return instance;
            }
        }

        public ProcessorContextsCollection ProcessorContexts { get; }

        public static void BeginChainContext()
        {
            if (chainCount == 0)
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new DittoChainContext();

                        chainCount = 1;
                    }
                }
            }
            else
            {
                chainCount++;
            }
        }

        public static void EndChainContext(bool force = false)
        {
            if (chainCount == 1 || force)
            {
                if (instance != null)
                {
                    lock (syncRoot)
                    {
                        instance = null;
                        chainCount = 0;
                    }
                }
            }
            else
            {
                chainCount--;
            }
        }
    }

    internal class ProcessorContextsCollection
    {
        private ConcurrentDictionary<Type, DittoProcessorContext> _processorContexts;

        public ProcessorContextsCollection()
        {
            _processorContexts = new ConcurrentDictionary<Type, DittoProcessorContext>();
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

        public DittoProcessorContext GetOrCreate(Type contexyType)
        {
            return _processorContexts.GetOrAdd(
                contexyType,
                type => (DittoProcessorContext)contexyType.GetInstance());
        }
    }
}
