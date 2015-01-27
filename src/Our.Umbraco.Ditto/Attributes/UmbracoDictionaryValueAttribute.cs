namespace Our.Umbraco.Ditto
{
    using System;

    /// <summary>
    /// The Umbraco dictionary value attribute.
    /// Used for providing Umbraco with additional information about a dictionary item to aid property value conversion.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class UmbracoDictionaryValueAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoDictionaryValueAttributeAttribute"/> class.
        /// </summary>
        /// <param name="dictionaryKey">
        /// The dictionary key.
        /// </param>
        public UmbracoDictionaryValueAttribute(string dictionaryKey)
        {
            this.DictionaryKey = dictionaryKey;
        }

        /// <summary>
        /// Gets or sets the dictionary key.
        /// </summary>
        public string DictionaryKey { get; set; }
    }
}