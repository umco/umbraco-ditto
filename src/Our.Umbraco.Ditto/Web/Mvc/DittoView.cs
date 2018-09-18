using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Abstract class for a DittoView
    /// </summary>
    public abstract class DittoView : DittoView<IPublishedContent>
    { }

    /// <summary>
    /// Abstract class for a DittoView
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public abstract class DittoView<TViewModel> : DittoView<IPublishedContent, TViewModel>
         where TViewModel : class
    { }

    /// <summary>
    /// Abstract class for a DittoView
    /// </summary>
    /// <typeparam name="TContent">The type of the content model.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public abstract class DittoView<TContent, TViewModel> : UmbracoViewPage<DittoViewModel<TContent, TViewModel>>
        where TContent : class, IPublishedContent
        where TViewModel : class
    {
        /// <summary>
        /// Sets the view data.
        /// </summary>
        /// <param name="viewData">The view data.</param>
        protected override void SetViewData(ViewDataDictionary viewData)
        {
            // Gather the Ditto view model
            var model = viewData.Model;

            // If model is already Ditto view-model, use it
            if (model is DittoViewModel<TContent, TViewModel>)
            {
                base.SetViewData(viewData);
                return;
            }

            // Gather any processor contexts
            var processorContexts = new List<DittoProcessorContext>();

            // Check to see if this is a Ditto transfer model
            if (model is DittoTransferModel transferModel)
            {
                model = transferModel.Model;
                processorContexts = transferModel.ProcessorContexts;
            }

            // Check if the model is a Ditto base view-model; Use the assigned properties
            if (model is IDittoViewModel)
            {
                if (model is IHasProcessorContexts foo && foo.ProcessorContexts != null)
                    processorContexts.AddRange(foo.ProcessorContexts);

                // check if the model is generic/wrapped; Unwrap the inner view-model and processor-contexts
                var modelType = model.GetType();
                if (modelType.IsGenericType && modelType.GetGenericTypeDefinition() == typeof(DittoViewModel<,>))
                {
                    var viewProperty = modelType.GetProperty("View", Ditto.MappablePropertiesBindingFlags);
                    model = FastPropertyAccessor.GetValue(viewProperty, model);
                }
            }

            var content = default(TContent);
            var culture = CultureInfo.CurrentCulture;

            // Get current content / culture
            if (this.UmbracoContext.PublishedContentRequest != null)
            {
                content = this.UmbracoContext.PublishedContentRequest.PublishedContent as TContent;
                culture = this.UmbracoContext.PublishedContentRequest.Culture;
            }

            // Process model
            if (model is TContent publishedContent)
            {
                content = publishedContent;
            }

            if (model is RenderModel<TContent> renderModel)
            {
                content = renderModel.Content;
                culture = renderModel.CurrentCulture;
            }

            var typedModel = model as TViewModel;

            // We need to give each view its own view data dictionary
            // to allow them to have different model types
            var newViewData = new ViewDataDictionary(viewData)
            {
                Model =
                    new DittoViewModel<TContent, TViewModel>(
                    content,
                    culture,
                    processorContexts,
                    typedModel)
            };

            base.SetViewData(newViewData);
        }
    }
}