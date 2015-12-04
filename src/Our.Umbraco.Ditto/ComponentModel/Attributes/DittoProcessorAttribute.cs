using System;
using Umbraco.Core;

namespace Our.Umbraco.Ditto
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class DittoProcessorAttribute : Attribute
    {
        private readonly int order;
        private readonly Type processorType;

        public DittoProcessorAttribute(Type processorType)
            : this(0, processorType)
        { }

        public DittoProcessorAttribute(int order, Type processorType)
        {
            if (processorType == null)
            {
                throw new ArgumentNullException("processorType");
            }

            if (processorType.IsAssignableFrom(typeof(DittoProcessor)))
            {
                throw new ArgumentException("Processor type must inherit from DittoProcessor", "processorType");
            }

            this.order = order;
            this.processorType = processorType;
        }

        public int Order
        {
            get
            {
                return this.order;
            }
        }

        public Type ProcessorType
        {
            get
            {
                return this.processorType;
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as DittoProcessorAttribute;
            return (other != null)
                && other.ProcessorType.AssemblyQualifiedName == this.processorType.AssemblyQualifiedName;
        }

        public override int GetHashCode()
        {
            return this.processorType.AssemblyQualifiedName.GetHashCode();
        }
    }
}