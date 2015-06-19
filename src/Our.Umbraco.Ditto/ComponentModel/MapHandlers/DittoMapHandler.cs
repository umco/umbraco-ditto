using System;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    public abstract class DittoMapHandler
    {
        public IPublishedContent Content { get; private set; }
        public Type ModelType { get; private set; }
        public object Model { get; private set; }

        protected DittoMapHandler(DittoMapHandlerContext ctx)
        {
            Content = ctx.Content;
            ModelType = ctx.ModelType;
            Model = ctx.Model;
        }

        public virtual void OnMapping()
        { }

        public virtual void OnMapped()
        { }
    }

    public abstract class DittoMapHandler<TMappedType> : DittoMapHandler
        where TMappedType : class
    {
        public new TMappedType Model { get; private set; }

        protected DittoMapHandler(DittoMapHandlerContext ctx)
            : base(ctx)
        {
            Model = ctx.Model as TMappedType;
        }
    }
}
