using System;
using System.Globalization;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Provides context for conversion events.
    /// </summary>
    public class DittoConversionHandlerContext
    {
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        public IPublishedContent Content { get; protected internal set; }

        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        public CultureInfo Culture { get; protected internal set; }

        /// <summary>
        /// Gets or sets the model object.
        /// </summary>
        public object Model { get; protected internal set; }

        /// <summary>
        /// Gets or sets the model type.
        /// </summary>
        public Type ModelType { get; protected internal set; }
    }
}