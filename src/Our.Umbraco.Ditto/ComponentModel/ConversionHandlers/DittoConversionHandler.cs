namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The types of conversion handler.
    /// </summary>
    internal enum DittoConversionHandlerType
    {
        /// <summary>
        /// Used for when Ditto is converting.
        /// </summary>
        OnConverting,

        /// <summary>
        /// Used for when Ditto has converted.
        /// </summary>
        OnConverted
    }

    /// <summary>
    /// The Ditto conversion handler.
    /// Provides a method of running custom code before and after Ditto conversion.
    /// </summary>
    public abstract class DittoConversionHandler
    {
        /// <summary>
        /// Gets or sets the current DittoConversionHandlerContext.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public DittoConversionHandlerContext Context { get; protected set; }

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

        /// <summary>
        /// Runs the conversion handler.
        /// </summary>
        /// <param name="ctx">The context.</param>
        /// <param name="type">The handler type to run.</param>
        internal virtual void Run(DittoConversionHandlerContext ctx, DittoConversionHandlerType type)
        {
            this.Context = ctx;

            this.Run(type);
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
                    this.OnConverting();
                    break;

                case DittoConversionHandlerType.OnConverted:
                    this.OnConverted();
                    break;
            }
        }
    }

    /// <summary>
    /// The Ditto conversion handler provides a method of running custom code before and after Ditto conversion.
    /// </summary>
    /// <typeparam name="TConvertedType">The type of the converted type.</typeparam>
    public abstract class DittoConversionHandler<TConvertedType> : DittoConversionHandler
        where TConvertedType : class
    {
        /// <summary>
        /// Gets or sets the current DittoConversionHandlerContext.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public new DittoConversionHandlerContext<TConvertedType> Context { get; protected set; }

        /// <summary>
        /// Runs the conversion handler.
        /// </summary>
        /// <param name="ctx">The context.</param>
        /// <param name="type">The handler type to run.</param>
        internal override void Run(DittoConversionHandlerContext ctx, DittoConversionHandlerType type)
        {
            this.Context = new DittoConversionHandlerContext<TConvertedType>(ctx);

            this.Run(type);
        }
    }
}