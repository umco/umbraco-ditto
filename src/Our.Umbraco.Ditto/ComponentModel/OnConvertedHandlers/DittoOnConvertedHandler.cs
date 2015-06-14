using System;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto.ComponentModel.OnConvertedHandlers
{
    public abstract class DittoOnConvertedHandler
    {
        public IPublishedContent Content { get; private set; }
        public Type ConvertedType { get; private set; }
        public object Converted { get; private set; }

        protected DittoOnConvertedHandler(ConvertedTypeEventArgs e)
        {
            Content = e.Content;
            ConvertedType = e.ConvertedType;
            Converted = e.Converted;
        }

        public abstract void OnConverted();
    }

    public abstract class DittoOnConvertedHandler<TConvertedType> : DittoOnConvertedHandler
        where TConvertedType : class
    {
        public new TConvertedType Converted { get; private set; }

        protected DittoOnConvertedHandler(ConvertedTypeEventArgs e)
            : base(e)
        {
            Converted = e.Converted as TConvertedType;
        }
    }
}
