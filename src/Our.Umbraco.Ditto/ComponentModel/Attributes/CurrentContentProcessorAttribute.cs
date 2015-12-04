namespace Our.Umbraco.Ditto
{
    using System;
    using global::Umbraco.Core.Models;

    /// <summary>
    /// The current content processor attribute.
    /// Used for providing Ditto with the current <see cref="IPublishedContent"/> object from Umbraco.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CurrentContentProcessorAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentContentProcessorAttribute"/> class.
        /// </summary>
        public CurrentContentProcessorAttribute()
            : this(0)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentContentProcessorAttribute"/> class.
        /// </summary>
        /// <param name="order">The order.</param>
        public CurrentContentProcessorAttribute(int order)
            : base(order, typeof(CurrentContentProcessor))
        { }
    }
}