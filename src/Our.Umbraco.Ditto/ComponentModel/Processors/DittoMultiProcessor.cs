using System.Web.Configuration;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The web.config app setting value processor.
    /// </summary>
    public class DittoMultiProcessor : DittoProcessor<object, DittoProcessorContext, DittoMultiProcessorAttribute>
    {
        /// <summary>
        /// Gets the value from the web.config app setting
        /// </summary>
        /// <returns>
        /// The <see cref="object"/> representing the processed value.
        /// </returns>
        public override object ProcessValue()
        {
            // We don't actually do anything here, we are just a holder
            // all the magic happen in PublishedContentExtensions.As method
            return null;
        }
    }
}