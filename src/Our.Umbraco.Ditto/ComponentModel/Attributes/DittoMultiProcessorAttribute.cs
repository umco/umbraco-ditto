using System;
using System.Collections.Generic;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public abstract class DittoMultiProcessorAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        public List<DittoProcessorAttribute> Attributes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DittoMultiProcessorAttribute" /> class.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        protected DittoMultiProcessorAttribute(IEnumerable<DittoProcessorAttribute> attributes)
        {
            this.Attributes = new List<DittoProcessorAttribute>(attributes); 
        }

        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        public override object ProcessValue()
        {
            // We don't actually implement anything here, it all happens in the 
            // published content extension method, so we'll just return null here
            return null;
        }
    }
}