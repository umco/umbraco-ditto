using System;
using System.Collections.Generic;
using Umbraco.Web.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Encapsulates extension methods for <see cref="RenderModel"/>.
    /// </summary>
    public static class RenderModelExtensions
    {
        /// <summary>Returns the given instance of <see cref="RenderModel"/> as the specified type.</summary>
        /// <param name="model">The <see cref="RenderModel"/> to convert.</param>
        /// <param name="processorContexts">A collection of <see cref="DittoProcessorContext"/> entities to use whilst processing values.</param>
        /// <param name="onConverting">The <see cref="Action{ConversionHandlerContext}"/> to fire when converting.</param>
        /// <param name="onConverted">The <see cref="Action{ConversionHandlerContext}"/> to fire when converted.</param>
        /// <typeparam name="T">The <see cref="Type"/> of items to return.</typeparam>
        /// <returns>The resolved generic <see cref="Type"/>.</returns>
        public static T As<T>(
            this RenderModel model,
            IEnumerable<DittoProcessorContext> processorContexts = null,
            Action<DittoConversionHandlerContext> onConverting = null,
            Action<DittoConversionHandlerContext> onConverted = null)
            where T : class
        {
            if (model == null || model.Content == null)
            {
                return default(T);
            }

            return model.Content.As<T>(model.CurrentCulture, null, processorContexts, onConverting, onConverted);
        }
    }
}