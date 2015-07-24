using System;
using global::Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The current content value resolver.
    /// </summary>
    public class CurrentContentAsValueResolver : DittoValueResolver<DittoValueResolverContext, CurrentContentAsAttribute>
    {
        /// <summary>
        /// Resolves the value.
        /// Gets the current <see cref="IPublishedContent"/> from Umbraco.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/> representing the raw value.
        /// </returns>
        public override object ResolveValue()
        {
            // NOTE: [LK] In order to prevent an infinite loop / stack-overflow, we check if the
            // property's type matches the containing model's type, then we throw an exception.
            if (Context.PropertyDescriptor.PropertyType == Context.PropertyDescriptor.ComponentType)
            {
                throw new InvalidOperationException(
                    string.Format("Unable to process property type '{0}', it is the same as the containing model type.",
                    Context.PropertyDescriptor.PropertyType.Name));
            }

            return Content.As(Context.PropertyDescriptor.PropertyType);
        }
    }
}