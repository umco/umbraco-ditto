using System;
using Umbraco.Web;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The processor context for the Umbraco dictionary value.
    /// </summary>
    public class UmbracoDictionaryProcessorContext : DittoProcessorContext
    {
        /// <summary>
        /// The function that provides the Umbraco dictionary value.
        /// </summary>
        public Func<string, string> GetDictionaryValue { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoDictionaryProcessorContext" /> class.
        /// </summary>
        public UmbracoDictionaryProcessorContext()
        {
            if (UmbracoContext.Current != null)
            {
                // HACK: [LK:2015-04-14] Resorting to using `UmbracoHelper`, as `CultureDictionaryFactoryResolver` isn't public in v6.2.x.
                GetDictionaryValue = new UmbracoHelper(UmbracoContext.Current).GetDictionaryValue;
            }
        }
    }
}