using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Our.Umbraco.Ditto
{
    internal class DittoChainContext
    {
        private static volatile DittoChainContext instance;
        private static object syncRoot = new object();
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

        public static void BeginChainContext(DittoProcessorContext baseProcessorContext)
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
                ++chainCount;
            }

            instance.ProcessorContexts.SetBaseContextAtLevel(chainCount, baseProcessorContext);
            instance.ProcessorContexts.SetLevel(chainCount);
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
                --chainCount;

                instance.ProcessorContexts.SetLevel(chainCount);
            }
        }
    }

    // [MB] not sure how happy I am with this collection having the set base context / level functions
    // seems a bit hacky but can't think of a better solution right now
    internal class ProcessorContextsCollection
    {
        private ConcurrentDictionary<Type, DittoProcessorContext> _processorContexts;
        private ConcurrentDictionary<int, DittoProcessorContext> _baseContexts;
        private int _currentLevel;

        public ProcessorContextsCollection()
        {
            _processorContexts = new ConcurrentDictionary<Type, DittoProcessorContext>();
            _baseContexts = new ConcurrentDictionary<int, DittoProcessorContext>();
            _currentLevel = 0;
        }

        internal void SetBaseContextAtLevel(
            int level,
            DittoProcessorContext baseProcessorContext)
        {
            _baseContexts.AddOrUpdate(level, baseProcessorContext, (lvl, ctx) => baseProcessorContext);
        }

        internal void SetLevel(int level)
        {
            _currentLevel = level;
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

        public DittoProcessorContext GetOrCreate(Type contextType)
        {
            // Get the base context for current level
            var baseCtx = _baseContexts[_currentLevel];

            // Get, clone and populate the relevant context for the given level
            return _processorContexts
                .GetOrAdd(contextType, type => (DittoProcessorContext)contextType.GetInstance())
                .Clone()
                .Populate(baseCtx);
        }
    }
}
