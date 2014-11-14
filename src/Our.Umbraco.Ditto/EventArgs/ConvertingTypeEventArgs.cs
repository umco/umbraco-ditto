namespace Our.Umbraco.Ditto
{
    using System.ComponentModel;

    using global::Umbraco.Core.Models;

    /// <summary>
    /// Provides data for a converting event.
    /// </summary>
    public class ConvertingTypeEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        public IPublishedContent Content { get; set; }
    }
}