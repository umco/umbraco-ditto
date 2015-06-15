﻿using System;
using Our.Umbraco.Ditto.ComponentModel.ConversionHandlers;

namespace Our.Umbraco.Ditto.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DittoConversionHandlerAttribute : Attribute
    {
        public Type HandlerType { get; private set; }

        public DittoConversionHandlerAttribute(Type handlerType)
        {
            if (!typeof(DittoConversionHandler).IsAssignableFrom(handlerType))
                throw new ArgumentException("Handler type must inherit from DittoConversionHandler", "handlerType");

            HandlerType = handlerType;
        }
    }
}