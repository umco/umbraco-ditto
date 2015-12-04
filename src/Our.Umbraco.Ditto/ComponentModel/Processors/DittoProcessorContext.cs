using System.ComponentModel;

namespace Our.Umbraco.Ditto
{
    public class DittoProcessorContext
    {
        public object Value { get; set; }

        public PropertyDescriptor PropertyDescriptor { get; internal set; }
    }
}