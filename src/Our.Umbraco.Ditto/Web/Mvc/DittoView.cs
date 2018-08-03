using System.Collections.Generic;
using System.Globalization;
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
            var transferModel = model as DittoTransferModel;
            if (transferModel != null)
            {
                model = transferModel.Model;
                processorContexts = transferModel.ProcessorContexts;
            }

            // Check if the model is a Ditto base view-model; Use the assigned properties
            var baseDittoViewModel = model as BaseDittoViewModel;
            if (baseDittoViewModel != null)
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
            var culture = CultureInfo.CurrentCulture;

            // Get current content / culture
            if (this.UmbracoContext.PublishedContentRequest != null)
            {
                content = this.UmbracoContext.PublishedContentRequest.PublishedContent;
                culture = this.UmbracoContext.PublishedContentRequest.Culture;
            }

            // Process model
            var publishedContent = model as IPublishedContent;
            if (publishedContent != null)
            {
                content = publishedContent;
            }

            var renderModel = model as RenderModel;
            if (renderModel != null)
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
                    new DittoViewModel<TViewModel>(
                    content,
                    culture,
                    processorContexts,
                    typedModel)
            };

            base.SetViewData(newViewData);
        }
    }
}