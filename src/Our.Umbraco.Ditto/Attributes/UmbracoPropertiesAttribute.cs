namespace Our.Umbraco.Ditto
{
    using System;

    /// <summary>
    /// The Umbraco properties attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class UmbracoPropertiesAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoPropertiesAttribute"/> class.
        /// </summary>
        public UmbracoPropertiesAttribute()
        {
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
    }
}