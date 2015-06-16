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
        { }

        /// <summary>
        /// Gets or sets the prefix.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets whether the properties should be retrieved recursively up the tree.
        /// </summary>
        public bool Recursive { get; set; }
    }
}