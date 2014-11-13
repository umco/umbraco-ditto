// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConvertingTypeEventArgs.cs" company="Umbrella Inc, Our Umbraco and other contributors">
//   Copyright Umbrella Inc, Our Umbraco and other contributors
// </copyright>
// <summary>
//   Provides data for a converting event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Our.Umbraco.Ditto.EventArgs
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