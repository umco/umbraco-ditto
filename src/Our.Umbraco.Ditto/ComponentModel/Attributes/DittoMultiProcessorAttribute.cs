using System;
using System.Collections.Generic;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Represents a multi-ditto processor capable of wrapping multiple attributes into a single attribute definition
    /// </summary>
    [AttributeUsage(Ditto.ProcessorAttributeTargets, AllowMultiple = true, Inherited = false)]
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
            var ctx = (DittoMultiProcessorContext)this.Context;

            foreach (var processorAttr in this.Attributes)
            {
                // Get the right context type
                var newCtx = ctx.ContextCache.GetOrCreateContext(processorAttr.ContextType);

                // Process value
                this.Value = processorAttr.ProcessValue(this.Value, newCtx);
            }

            return this.Value;
        }
    }
}