using System.Collections.Generic;
using System.Globalization;
using Umbraco.Core.Models;
using Umbraco.Web.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Base class for a DittoViewModel
    /// </summary>
    public abstract class BaseDittoViewModel : RenderModel, IDittoViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDittoViewModel"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="processorContexts">The processor contexts.</param>
        protected BaseDittoViewModel(
            IPublishedContent content, 
            CultureInfo culture = null, 
            IEnumerable<DittoProcessorContext> processorContexts = null)
            : base(content, culture)
        {
            ProcessorContexts = processorContexts ?? new List<DittoProcessorContext>();
        }

        /// <summary>
        /// Gets the current page.
        /// </summary>
        /// <value>
        /// The current page.
        /// </value>
        public IPublishedContent CurrentPage { get { return Content; } }

        /// <summary>
        /// Gets or sets the processor contexts.
        /// </summary>
        /// <value>
        /// The processor contexts.
        /// </value>
        internal IEnumerable<DittoProcessorContext> ProcessorContexts { get; set; }
    }

    /// <summary>
    /// Model for a DittoView
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public class DittoViewModel<TViewModel> : BaseDittoViewModel
        where TViewModel : class
    {
        private TViewModel _view;

        /// <summary>
        /// Initializes a new instance of the <see cref="DittoViewModel{TViewModel}"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="processorContexts">The processor contexts.</param>
        /// <param name="viewModel">The view model.</param>
        public DittoViewModel(
            IPublishedContent content,
            CultureInfo culture = null,
            IEnumerable<DittoProcessorContext> processorContexts = null,
            TViewModel viewModel = null)
            : base(content, culture, processorContexts)
        {
            if(viewModel != null)
                View = viewModel;
        }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        public TViewModel View
        {
            get
            {
                if (_view == null)
                {
                    if (Content is TViewModel)
                    {
                        _view = Content as TViewModel;
                    }
                    else
                    {
                        _view = Content.As<TViewModel>(processorContexts: ProcessorContexts);
                    }
                }

                return _view;
            }
            internal set
            {
                _view = value;
            }
        }
        
    }

    /// <summary>
    /// Model for a DittoView
    /// </summary>
    public class DittoViewModel : DittoViewModel<IPublishedContent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DittoViewModel"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="processorContexts">The processor contexts.</param>
        protected DittoViewModel(
            IPublishedContent content,
            CultureInfo culture = null,
            IEnumerable<DittoProcessorContext> processorContexts = null)
            : base(content, culture, processorContexts)
        { }
    }

}