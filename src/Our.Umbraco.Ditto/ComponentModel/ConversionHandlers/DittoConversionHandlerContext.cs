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
        public IPublishedContent Content { get; set; }

        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Gets or sets the model object.
        /// </summary>
        public object Model { get; set; }

        /// <summary>
        /// Gets or sets the model type.
        /// </summary>
        public Type ModelType { get; set; }
    }

    /// <summary>
    /// Provides context for conversion events, with a generic model object type.
    /// </summary>
    public class DittoConversionHandlerContext<TConvertedType> : DittoConversionHandlerContext
        where TConvertedType : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DittoConversionHandlerContext"/> class.
        /// </summary>
        public DittoConversionHandlerContext()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DittoConversionHandlerContext"/> class.
        /// </summary>
        /// <param name="ctx">The context.</param>
        internal DittoConversionHandlerContext(DittoConversionHandlerContext ctx)
        {
            this.Content = ctx.Content;
            this.Culture = ctx.Culture;
            this.ModelType = ctx.ModelType;
            this.Model = ctx.Model as TConvertedType;
        }

        /// <summary>
        /// Gets or sets the model object.
        /// </summary>
        public new TConvertedType Model { get; protected internal set; }
    }
}