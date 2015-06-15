using System;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto.ComponentModel
{
    /// <summary>
    /// Provides context for conversion events.
    /// </summary>
    public class ConversionHandlerContext
    {
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        public IPublishedContent Content { get; set; }

        /// <summary>
        /// Gets or sets the model object.
        /// </summary>
        public object Model { get; set; }

        /// <summary>
        /// Gets or sets the model type.
        /// </summary>
        public Type ModelType { get; set; }
    }
}
