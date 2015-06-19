using System;

namespace Our.Umbraco.Ditto
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DittoMapHandlerAttribute : Attribute
    {
        public Type HandlerType { get; private set; }

        public DittoMapHandlerAttribute(Type handlerType)
        {
            if (!typeof(DittoMapHandler).IsAssignableFrom(handlerType))
                throw new ArgumentException("Handler type must inherit from DittoMapHandler", "handlerType");

            HandlerType = handlerType;
        }
    }
}
