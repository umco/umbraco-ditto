using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Interface for a DittoViewModel
    /// </summary>
    public interface IDittoViewModel : IContentModel
    {
        /// <summary>
        /// Gets the current page.
        /// </summary>
        /// <value>
        /// The current page.
        /// </value>
        IPublishedContent CurrentPage { get; }
    }
}