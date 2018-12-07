using System;
using System.Collections.Generic;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Represents a multi-Ditto processor capable of wrapping multiple attributes into a single attribute definition.
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
#if DEBUG
                using (ProfilingLogger?.DebugDuration<DittoMultiProcessorAttribute>($"Processor '{processorAttr.GetType().Name}' ({this.Context.Content.Id})"))
                {
#endif
                    // Get the right context type
                    var newCtx = this.ChainContext.ProcessorContexts.GetOrCreate(this.Context, processorAttr.ContextType);

                    // Populate UmbracoContext & ApplicationContext
                    processorAttr.UmbracoContext = this.UmbracoContext;
                    processorAttr.ApplicationContext = this.ApplicationContext;

                    // Process value
                    this.Value = processorAttr.ProcessValue(this.Value, newCtx, this.ChainContext);
#if DEBUG
                }
#endif
            }

            return this.Value;
        }
    }
}