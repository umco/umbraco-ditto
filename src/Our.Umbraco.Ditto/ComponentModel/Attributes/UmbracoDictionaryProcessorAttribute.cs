using System;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The Umbraco dictionary value processor attribute.
    /// Used for providing Umbraco with additional information about a dictionary item to aid property value processing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class UmbracoDictionaryProcessorAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoDictionaryProcessorAttribute"/> class.
        /// </summary>
        public UmbracoDictionaryProcessorAttribute()
            : base(typeof(UmbracoDictionaryProcessorAttribute))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoDictionaryProcessorAttribute" /> class.
        /// </summary>
        /// <param name="dictionaryKey">The dictionary key.</param>
        public UmbracoDictionaryProcessorAttribute(string dictionaryKey)
            : this()
        {
            this.DictionaryKey = dictionaryKey;
        }

        /// <summary>
        /// Gets or sets the dictionary key.
        /// </summary>
        public string DictionaryKey { get; set; }
    }
}