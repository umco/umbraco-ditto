using System;
using System.Collections.Generic;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Represents a multi-ditto processor capable of wrapping multiple attributes into a single attribute definition
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    [DittoProcessorMetaData(ValueType = typeof(object), ContextType = typeof(DittoMultiProcessorContext))]
    public abstract class DittoMultiProcessorAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DittoMultiProcessorAttribute" /> class.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        protected DittoMultiProcessorAttribute(IEnumerable<DittoProcessorAttribute> attributes)
        {
            this.Attributes = new List<DittoProcessorAttribute>(attributes);
        }

        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        public List<DittoProcessorAttribute> Attributes { get; set; }

        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        public override object ProcessValue()
        {
            var ctx = (DittoMultiProcessorContext)Context;

            foreach (var processorAttr in Attributes)
            {
                // Get the right context type
                var newCtx = ctx.ContextCache.GetOrCreateContext(processorAttr.ContextType);

                // Process value
                Value = processorAttr.ProcessValue(Value, newCtx);
            }

            return Value;
        }
    }
}