using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto.ComponentModel.OnConvertedHandlers
{
    public abstract class DittoOnConvertedHandler
    {
        public IPublishedContent Content { get; private set; }
        public object Model { get; private set; }

        protected DittoOnConvertedHandler(ConvertedTypeEventArgs e)
        {
            Content = e.Content;
            Model = e.Converted;
        }

        public abstract void OnConverted();
    }

    public abstract class DittoOnConvertedHandler<TConvertedType> : DittoOnConvertedHandler
        where TConvertedType : class
    {
        public new TConvertedType Model { get; private set; }

        protected DittoOnConvertedHandler(ConvertedTypeEventArgs e)
            : base(e)
        {
            Model = e.Converted as TConvertedType;
        }
    }
}
