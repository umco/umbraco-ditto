namespace Our.Umbraco.Ditto
{
    using System;

    /// <summary>
    /// Provides methods to handle firing specific conversion events.
    /// </summary>
    public static class EventHandlers
    {
        /// <summary>
        /// The converting type method handler.
        /// </summary>
        public static event EventHandler<ConvertingTypeEventArgs> ConvertingType;

        /// <summary>
        /// The converted type method handler.
        /// </summary>
        public static event EventHandler<ConvertedTypeEventArgs> ConvertedType;

        /// <summary>
        /// Calls any methods designed to handle <see cref="ConvertingTypeEventArgs"/> events.
        /// </summary>
        /// <param name="args">
        /// The <see cref="ConvertingTypeEventArgs"/>.
        /// </param>
        public static void CallConvertingTypeHandler(ConvertingTypeEventArgs args)
        {
            if (ConvertingType != null)
            {
                ConvertingType(null, args);
            }
        }

        /// <summary>
        /// Calls any methods designed to handle <see cref="ConvertedTypeEventArgs"/> events.
        /// </summary>
        /// <param name="args">
        /// The <see cref="ConvertedTypeEventArgs"/>.
        /// </param>
        public static void CallConvertedTypeHandler(ConvertedTypeEventArgs args)
        {
            if (ConvertedType != null)
            {
                ConvertedType(null, args);
            }
        }
    }
}