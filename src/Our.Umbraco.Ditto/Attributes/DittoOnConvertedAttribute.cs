using System;
using Our.Umbraco.Ditto.ComponentModel.OnConvertedHandlers;

namespace Our.Umbraco.Ditto.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DittoOnConvertedAttribute : Attribute
    {
        public Type HandlerType { get; private set; }

        public DittoOnConvertedAttribute()
        { }

        public DittoOnConvertedAttribute(Type handlerType)
        {
            if (!typeof(DittoOnConvertedHandler).IsAssignableFrom(handlerType))
                throw new ArgumentException("Handler type must inherit from DittoOnConvertedHandler", "handlerType");

            HandlerType = handlerType;
        }
    }
}
