using System;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The Ditto default processor attribute.
    /// Used for specifying that Ditto should use a default processor during object conversion.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DittoDefaultProcessorAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DittoDefaultProcessorAttribute"/> class.
        /// </summary>
        /// <param name="processorType">Type of the processor</param>
        public DittoDefaultProcessorAttribute(Type processorType)
        {
            this.ProcessorType = processorType;
        }

        /// <summary>
        /// Gets or sets the type of the processor.
        /// </summary>
        /// <value>
        /// The type of the processor.
        /// </value>
        public Type ProcessorType { get; set; }
    }
}