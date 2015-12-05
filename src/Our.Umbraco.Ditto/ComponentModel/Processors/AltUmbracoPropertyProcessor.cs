using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The Umbraco Property processor.
    /// </summary>
    public class AltUmbracoPropertyProcessor : DittoProcessor<IPublishedContent, DittoProcessorContext, AltUmbracoPropertyProcessorAttribute>
    {
        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        public override object ProcessValue()
        {
            if (Value.IsNullOrEmptyString())
            {
                var newContext = new DittoProcessorContext
                {
                    Value = Context.Content,
                    Content = Context.Content,
                    PropertyDescriptor = Context.PropertyDescriptor,
                    TargetType = Context.TargetType
                };

                var attr = new UmbracoPropertyProcessorAttribute(Attribute.PropertyName,
                    Attribute.Recursive,
                    Attribute.DefaultValue);

                return new UmbracoPropertyProcessor().ProcessValue(newContext, attr, Culture);
            }

            return Value;
        }
    }
}