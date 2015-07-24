using System;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The Ditt conversion handler attribute.
    /// Provides the ability to associate a handler class with a model to handle pre/post conversion custom logic.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class DittoConversionHandlerAttribute : Attribute
    {
        /// <summary>
        /// Gets the type of the handler.
        /// </summary>
        /// <value>
        /// The type of the handler.
        /// </value>
        public Type HandlerType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DittoConversionHandlerAttribute"/> class.
        /// </summary>
        /// <param name="handlerType">Type of the handler.</param>
        /// <exception cref="System.ArgumentException">Handler type must inherit from DittoConversionHandler;handlerType</exception>
        public DittoConversionHandlerAttribute(Type handlerType)
        {
            if (!typeof(DittoConversionHandler).IsAssignableFrom(handlerType))
                throw new ArgumentException("Handler type must inherit from DittoConversionHandler", "handlerType");

            HandlerType = handlerType;
        }
    }
}
