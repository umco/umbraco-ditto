namespace Our.Umbraco.Ditto
{
    using System;

    /// <summary>
    /// The Umbraco property prefix attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class UmbracoPropertyPrefixAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoPropertyAttribute"/> class.
        /// </summary>
        /// <param name="prefix">
        /// The property name.
        /// </param>
        public UmbracoPropertyPrefixAttribute(
            string prefix)
        {
            this.Prefix = prefix;
        }

        /// <summary>
        /// Gets or sets the prefix.
        /// </summary>
        public string Prefix { get; set; }
    }
}