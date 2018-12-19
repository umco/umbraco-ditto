using System.Collections.Generic;
using System.Web.Mvc;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Abstract class for a DittoView
    /// </summary>
    public abstract class DittoView : DittoView<IPublishedContent>
    { }

    // TODO : v8 : Culture is no longer on ContentModel (was RenderModel) something about variations I suppose
    /// <summary>
    /// Abstract class for a DittoView
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public abstract class DittoView<TViewModel>
        : UmbracoViewPage<DittoViewModel<TViewModel>>
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
            if (model is DittoViewModel<TViewModel>)
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
            if (model is BaseDittoViewModel baseDittoViewModel)
            {
                processorContexts.AddRange(baseDittoViewModel.ProcessorContexts);

                // Furthermore, check if the model is generic/wrapped; Unwrap the inner view-model
                var modelType = model.GetType();
                if (modelType.IsGenericType && modelType.GetGenericTypeDefinition() == typeof(DittoViewModel<>))
                {
                    var viewProperty = modelType.GetProperty("View", Ditto.MappablePropertiesBindingFlags);
                    model = FastPropertyAccessor.GetValue(viewProperty, model);
                }
            }

            var content = default(IPublishedContent);

            // Get current content / culture
            if (this.UmbracoContext.PublishedRequest != null)
            {
                content = this.UmbracoContext.PublishedRequest.PublishedContent;
            }

            // Process model
            if (model is IPublishedContent publishedContent)
            {
                content = publishedContent;
            }

            if (model is ContentModel renderModel)
            {
                content = renderModel.Content;
            }

            var typedModel = model as TViewModel;

            // We need to give each view its own view data dictionary
            // to allow them to have different model types
            var newViewData = new ViewDataDictionary(viewData)
            {
                Model =
                    new DittoViewModel<TViewModel>(
                    content,
                    processorContexts,
                    typedModel)
            };

            base.SetViewData(newViewData);
        }
    }
}