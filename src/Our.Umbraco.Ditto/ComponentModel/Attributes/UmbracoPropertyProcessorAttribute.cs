using System;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The Umbraco property processor attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class UmbracoPropertyProcessorAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoPropertyProcessorAttribute"/> class.
        /// </summary>
        public UmbracoPropertyProcessorAttribute()
            : base(typeof(UmbracoPropertyProcessor))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoPropertyProcessorAttribute"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="altPropertyName">Name of the alt property.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <param name="defaultValue">The default value.</param>
        public UmbracoPropertyProcessorAttribute(
            string propertyName,
            string altPropertyName = "",
            bool recursive = false,
            object defaultValue = null)
            : this()
        {
            this.PropertyName = propertyName;
            this.AltPropertyName = altPropertyName;
            this.Recursive = recursive;
            this.DefaultValue = defaultValue;
        }

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the name of the alt property.
        /// </summary>
        /// <value>
        /// The name of the alt property.
        /// </value>
        public string AltPropertyName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UmbracoPropertyProcessorAttribute"/> is recursive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if recursive; otherwise, <c>false</c>.
        /// </value>
        public bool Recursive { get; set; }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>
        /// The default value.
        /// </value>
        public object DefaultValue { get; set; }
    }
}