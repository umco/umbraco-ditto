using System;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto.ComponentModel.ConversionHandlers
{
    public abstract class DittoConversionHandler
    {
        public IPublishedContent Content { get; private set; }
        public Type ModelType { get; private set; }
        public object Model { get; private set; }

        protected DittoConversionHandler(ConversionHandlerContext ctx)
        {
            Content = ctx.Content;
            ModelType = ctx.ModelType;
            Model = ctx.Model;
        }

        public virtual void OnConverting()
        { }

        public virtual void OnConverted()
        { }
    }

    public abstract class DittoConversionHandler<TConvertedType> : DittoConversionHandler
        where TConvertedType : class
    {
        public new TConvertedType Model { get; private set; }

        protected DittoConversionHandler(ConversionHandlerContext ctx)
            : base(ctx)
        {
            Model = ctx.Model as TConvertedType;
        }
    }
}
