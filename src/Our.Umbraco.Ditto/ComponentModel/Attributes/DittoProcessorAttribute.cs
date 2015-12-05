using System;
using System.Runtime.CompilerServices;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class DittoProcessorAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DittoProcessorAttribute"/> class.
        /// </summary>
        /// <param name="processorType">Type of the processor.</param>
        /// <exception cref="System.ArgumentNullException">processorType</exception>
        /// <exception cref="System.ArgumentException">Processor type must inherit from DittoProcessor;processorType</exception>
        public DittoProcessorAttribute(Type processorType)
        {
            if (processorType == null)
            {
                throw new ArgumentNullException("processorType");
            }

            if (processorType.IsAssignableFrom(typeof(DittoProcessor)))
            {
                throw new ArgumentException("Processor type must inherit from DittoProcessor", "processorType");
            }

            this.ProcessorType = processorType;
            //this.Order = order;
        }

        /// <summary>
        /// Gets the type of the processor.
        /// </summary>
        /// <value>
        /// The type of the processor.
        /// </value>
        public Type ProcessorType { get; private set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        public int Order { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as DittoProcessorAttribute;
            return (other != null)
                && other.ProcessorType.AssemblyQualifiedName == this.ProcessorType.AssemblyQualifiedName;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.ProcessorType.AssemblyQualifiedName.GetHashCode();
        }
    }
}