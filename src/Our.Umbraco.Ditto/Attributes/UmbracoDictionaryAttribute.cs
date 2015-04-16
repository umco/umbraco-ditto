namespace Our.Umbraco.Ditto
{
    using System;

    /// <summary>
    /// The Umbraco dictionary value attribute.
    /// Used for providing Umbraco with additional information about a dictionary item to aid property value conversion.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class UmbracoDictionaryAttribute : DittoValueResolverAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoDictionaryAttribute"/> class.
        /// </summary>
        public UmbracoDictionaryAttribute()
            : base(typeof(UmbracoDictionaryValueResolver))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoDictionaryAttribute"/> class.
        /// </summary>
        /// <param name="dictionaryKey">
        /// The dictionary key.
        /// </param>
        public UmbracoDictionaryAttribute(string dictionaryKey)
            : base(typeof(UmbracoDictionaryValueResolver))
        {
            this.DictionaryKey = dictionaryKey;
        }

        /// <summary>
        /// Gets or sets the dictionary key.
        /// </summary>
        public string DictionaryKey { get; set; }
    }
}