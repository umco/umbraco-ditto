using System.Collections.Generic;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Temporary model for transfering values from a controller to the view
    /// </summary>
    internal class DittoTransferModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DittoTransferModel"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public DittoTransferModel(object model)
        {
            Model = model;
            ProcessorContexts = new List<DittoProcessorContext>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DittoTransferModel"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="processorContexts">The processor contexts.</param>
        public DittoTransferModel(object model, IEnumerable<DittoProcessorContext> processorContexts)
        {
            Model = model;
            ProcessorContexts = new List<DittoProcessorContext>(processorContexts);
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public object Model { get; set; }

        /// <summary>
        /// Gets or sets the processor contexts.
        /// </summary>
        /// <value>
        /// The processor contexts.
        /// </value>
        public List<DittoProcessorContext> ProcessorContexts { get; set; }
    }
}