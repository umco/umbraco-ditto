using System;
using System.ComponentModel;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    public class DittoProcessorContext
    {
        public object Value { get; internal set; }

        public IPublishedContent Content { get; internal set; }

        public Type Type { get; internal set; }

        public PropertyDescriptor PropertyDescriptor { get; internal set; }
    }
}