namespace Our.Umbraco.Ditto
{
    using System;

    /// <summary>
    /// The Umbraco property attribute. 
    /// Used for providing Umbraco with additional information about the property to aid conversion.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class UmbracoPropertyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoPropertyAttribute"/> class.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="altPropertyName">
        /// The alternative property name.
        /// </param>
        /// <param name="recursive">
        /// Whether the property should be retrieved recursively up the tree.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        public UmbracoPropertyAttribute(string propertyName, string altPropertyName = "", bool recursive = false, object defaultValue = null)
        {
            this.PropertyName = propertyName;
            this.AltPropertyName = altPropertyName;
            this.Recursive = recursive;
            this.DefaultValue = defaultValue;
        }

        /// <summary>
        /// Gets or sets the property name.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the alternative property name.
        /// </summary>
        public string AltPropertyName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the property should be retrieved recursively up the tree.
        /// </summary>
        public bool Recursive { get; set; }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        public object DefaultValue { get; set; }
    }
}