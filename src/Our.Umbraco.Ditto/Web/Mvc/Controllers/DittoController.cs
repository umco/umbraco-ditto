using System.Collections.Generic;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Base class for a DittoController
    /// </summary>
    public abstract class DittoController : SurfaceController, IRenderController
    {
        /// <summary>
        /// A field to store a list of Ditto processor contexts.
        /// </summary>
        private List<DittoProcessorContext> processorContexts;

        /// <summary>
        /// Initializes a new instance of the <see cref="DittoController"/> class.
        /// </summary>
        protected DittoController()
        {
            processorContexts = new List<DittoProcessorContext>();
        }

        /// <summary>
        /// Creates a <see cref="T:System.Web.Mvc.ViewResult" /> object using the current action name, and model that renders a view.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// The view result.
        /// </returns>
        protected virtual ActionResult CurrentView(object model = null)
        {
            var viewName = ControllerContext.RouteData.Values["action"].ToString();

            return View(viewName, null, model);
        }

        /// <summary>
        /// Creates a <see cref="T:System.Web.Mvc.ViewResult" /> object using the view name, master-page name, and model that renders a view.
        /// </summary>
        /// <param name="viewName">The name of the view that is rendered to the response.</param>
        /// <param name="masterName">The name of the master page or template to use when the view is rendered.</param>
        /// <param name="model">The model that is rendered by the view.</param>
        /// <returns>
        /// The view result.
        /// </returns>
        protected override ViewResult View(string viewName, string masterName, object model)
        {
            if (model == null)
            {
                model = CurrentPage;
            }

            var transferModel = new DittoTransferModel(model, processorContexts);

            return base.View(viewName, masterName, transferModel);
        }

        /// <summary>
        /// Creates a <see cref="T:System.Web.Mvc.PartialViewResult" /> object that renders a partial view based on the action name, using the given model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// A partial-view result object.
        /// </returns>
        protected virtual PartialViewResult CurrentPartialView(object model = null)
        {
            var viewName = ControllerContext.RouteData.Values["action"].ToString();

            return PartialView(viewName, model);
        }

        /// <summary>
        /// Creates a <see cref="T:System.Web.Mvc.PartialViewResult" /> object that renders a partial view, by using the specified view name and model.
        /// </summary>
        /// <param name="viewName">The name of the view that is rendered to the response.</param>
        /// <param name="model">The model that is rendered by the partial view</param>
        /// <returns>
        /// A partial-view result object.
        /// </returns>
        protected override PartialViewResult PartialView(string viewName, object model)
        {
            if (model == null)
            {
                model = CurrentPage;
            }

            var transferModel = new DittoTransferModel(model, processorContexts);

            return base.PartialView(viewName, transferModel);
        }

        /// <summary>
        /// Registers the supplied processor context with Ditto.
        /// </summary>
        /// <typeparam name="TContextType">The processor context type.</typeparam>
        /// <param name="context">The context.</param>
        protected virtual void RegisterProcessorContext<TContextType>(TContextType context)
            where TContextType : DittoProcessorContext
        {
            processorContexts.Add(context);
        }
    }
}