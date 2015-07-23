using System;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    public abstract class DittoConversionHandler
    {
        public IPublishedContent Content { get; protected set; }
        public Type ModelType { get; protected set; }
        public object Model { get; protected set; }

        internal virtual void Run(DittoConversionHandlerContext ctx, DittoConversionHandlerType type)
        {
            Content = ctx.Content;
            ModelType = ctx.ModelType;
            Model = ctx.Model;

            Run(type);
        }

        internal virtual void Run(DittoConversionHandlerType type)
        {
            switch (type)
            {
                case DittoConversionHandlerType.OnConverting:
                    OnConverting();
                    break;

                case DittoConversionHandlerType.OnConverted:
                    OnConverted();
                    break;
            }
        }

        public virtual void OnConverting()
        { }

        public virtual void OnConverted()
        { }
    }

    public abstract class DittoConversionHandler<TConvertedType> : DittoConversionHandler
        where TConvertedType : class
    {
        public new TConvertedType Model { get; protected set; }

        internal override void Run(DittoConversionHandlerContext ctx, DittoConversionHandlerType type)
        {
            Content = ctx.Content;
            ModelType = ctx.ModelType;
            Model = ctx.Model as TConvertedType;

            Run(type);
        }
    }

    internal enum DittoConversionHandlerType
    {
        OnConverting,
        OnConverted
    }
}
