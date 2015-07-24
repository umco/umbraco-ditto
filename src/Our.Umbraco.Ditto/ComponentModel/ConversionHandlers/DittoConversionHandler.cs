using System;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The Ditto conversion handler.
    /// Provides a method of running custom code before and after Ditto conversion.
    /// </summary>
    public abstract class DittoConversionHandler
    {
        /// <summary>
        /// Gets or sets the current IPublishedContent.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public IPublishedContent Content { get; protected set; }

        /// <summary>
        /// Gets or sets the type of the model.
        /// </summary>
        /// <value>
        /// The type of the model.
        /// </value>
        public Type ModelType { get; protected set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public object Model { get; protected set; }

        /// <summary>
        /// Runs the conversion handler.
        /// </summary>
        /// <param name="ctx">The context.</param>
        /// <param name="type">The handler type to run.</param>
        internal virtual void Run(DittoConversionHandlerContext ctx, DittoConversionHandlerType type)
        {
            Content = ctx.Content;
            ModelType = ctx.ModelType;
            Model = ctx.Model;

            Run(type);
        }

        /// <summary>
        /// Runs the specified handler type.
        /// </summary>
        /// <param name="type">The handler type.</param>
        internal virtual void Run(DittoConversionHandlerType type)
        {
            switch (type)
            {
                case DittoConversionHandlerType.OnConverting:
                    OnConverting();
                    break;

                case DittoConversionHandlerType.OnConverted:
                    OnConverted();
                    break;
            }
        }

        /// <summary>
        /// Called just before conversion of the model occurs.
        /// </summary>
        public virtual void OnConverting()
        { }

        /// <summary>
        /// Called just after conversion of the model occurs.
        /// </summary>
        public virtual void OnConverted()
        { }
    }

    /// <summary>
    /// The Ditto conversion handler provides a method of running custom code before and after Ditto conversion.
    /// </summary>
    /// <typeparam name="TConvertedType">The type of the converted type.</typeparam>
    public abstract class DittoConversionHandler<TConvertedType> : DittoConversionHandler
        where TConvertedType : class
    {
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public new TConvertedType Model { get; protected set; }

        /// <summary>
        /// Runs the conversion handler.
        /// </summary>
        /// <param name="ctx">The context.</param>
        /// <param name="type">The handler type to run.</param>
        internal override void Run(DittoConversionHandlerContext ctx, DittoConversionHandlerType type)
        {
            Content = ctx.Content;
            ModelType = ctx.ModelType;
            Model = ctx.Model as TConvertedType;

            Run(type);
        }
    }

    internal enum DittoConversionHandlerType
    {
        OnConverting,
        OnConverted
    }
}
