using System.Collections.Generic;
using Umbraco.Core.Models;
using Umbraco.Web.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Interface for a DittoViewModel
    /// </summary>
    public interface IDittoViewModel : IRenderModel
    {
        /// <summary>
        /// Gets the current page.
        /// </summary>
        /// <value>
        /// The current page.
        /// </value>
        IPublishedContent CurrentPage { get; }
    }

    internal interface IHasProcessorContexts
    {
        IEnumerable<DittoProcessorContext> ProcessorContexts { get; }
    }
}