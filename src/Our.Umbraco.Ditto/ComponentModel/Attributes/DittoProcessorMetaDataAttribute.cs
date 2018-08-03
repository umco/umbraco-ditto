using System;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Meta data attribute for a DittoProcessorAttribute to help validate
    /// the passed in types
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class DittoProcessorMetaDataAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DittoProcessorMetaDataAttribute"/> class.
        /// </summary>
        public DittoProcessorMetaDataAttribute()
        {
            this.ValueType = typeof(object);
            this.ContextType = typeof(DittoProcessorContext);
        }

        /// <summary>
        /// Gets or sets the value type.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public Type ValueType { get; set; }

        /// <summary>
        /// Gets or sets the type of the context.
        /// </summary>
        /// <value>
        /// The type of the context.
        /// </value>
        public Type ContextType { get; set; }
    }
}