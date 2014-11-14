namespace Our.Umbraco.Ditto
{
    using System;

    using global::Umbraco.Core.Models;

    /// <summary>
    /// Provides data for a converted event.
    /// </summary>
    public class ConvertedTypeEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        public IPublishedContent Content { get; set; }

        /// <summary>
        /// Gets or sets the converted object.
        /// </summary>
        public object Converted { get; set; }

        /// <summary>
        /// Gets or sets the converted type.
        /// </summary>
        public Type ConvertedType { get; set; }
    }
}