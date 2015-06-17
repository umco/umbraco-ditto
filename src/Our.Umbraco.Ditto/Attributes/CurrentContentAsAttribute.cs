using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    using System;

    /// <summary>
    /// The current content attribute.
    /// Used for providing Ditto with the current <see cref="IPublishedContent"/> object from Umbraco.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CurrentContentAsAttribute : DittoValueResolverAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentContentAsAttribute"/> class.
        /// </summary>
        public CurrentContentAsAttribute()
            : base(typeof(CurrentContentAsValueResolver))
        { }
    }
}