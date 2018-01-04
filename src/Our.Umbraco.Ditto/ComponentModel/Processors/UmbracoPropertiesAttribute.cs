using System;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The Umbraco properties attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class UmbracoPropertiesAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoPropertiesAttribute"/> class.
        /// </summary>
        public UmbracoPropertiesAttribute()
        {
            PropertySource = Ditto.DefaultPropertySource;
        }

        /// <summary>
        /// Gets or sets the prefix.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the properties should be retrieved recursively up the tree.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [recursive]; otherwise, <c>false</c>.
        /// </value>
        public bool Recursive { get; set; }

        /// <summary>
        /// Gets or sets the property source from which to map values from
        /// </summary>
        public PropertySource PropertySource { get; set; }
    }
}