using System;
using System.Collections.Generic;
using System.Linq;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Represents a multi-ditto processor capable of wrapping multiple attributes into a single attribute definition
    /// </summary>
    [AttributeUsage(Ditto.ProcessorAttributeTargets, AllowMultiple = true, Inherited = false)]
    public abstract class DittoMultiProcessorAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DittoMultiProcessorAttribute" /> class.
        /// </summary>
        protected DittoMultiProcessorAttribute()
        {
            this.Attributes = new List<DittoProcessorAttribute>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DittoMultiProcessorAttribute" /> class.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        protected DittoMultiProcessorAttribute(IEnumerable<DittoProcessorAttribute> attributes)
            : this()
        {
            this.Attributes.AddRange(attributes);
        }

        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        protected List<DittoProcessorAttribute> Attributes { get; set; }

        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        public override object ProcessValue()
        {

            foreach (var processorAttr in this.Attributes)
            {
                // Get the right context type
                var newCtx = DittoChainContext.Current.ProcessorContexts.GetOrCreate(processorAttr.ContextType);

                // (Re-)Populate the context properties
                //newCtx.Populate(Context.Content, Context.TargetType, Context.PropertyDescriptor, Context.Culture);

                // Process value
                this.Value = processorAttr.ProcessValue(this.Value, newCtx);
            }

            return this.Value;
        }
    }
}