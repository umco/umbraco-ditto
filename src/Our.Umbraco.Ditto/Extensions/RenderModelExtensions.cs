using System;
using System.Collections.Generic;
using Umbraco.Web.Models;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Encapsulates extension methods for <see cref="ContentModel"/>.
    /// </summary>
    public static class RenderModelExtensions
    {
        /// <summary>Returns the given instance of <see cref="ContentModel"/> as the specified type.</summary>
        /// <param name="model">The <see cref="ContentModel"/> to convert.</param>
        /// <param name="processorContexts">A collection of <see cref="DittoProcessorContext"/> entities to use whilst processing values.</param>
        /// <param name="onConverting">The <see cref="Action{ConversionHandlerContext}"/> to fire when converting.</param>
        /// <param name="onConverted">The <see cref="Action{ConversionHandlerContext}"/> to fire when converted.</param>
        /// <typeparam name="T">The <see cref="Type"/> of items to return.</typeparam>
        /// <returns>The resolved generic <see cref="Type"/>.</returns>
        public static T As<T>(
            this IContentModel model,
            IEnumerable<DittoProcessorContext> processorContexts = null,
            Action<DittoConversionHandlerContext> onConverting = null,
            Action<DittoConversionHandlerContext> onConverted = null)
            where T : class
        {
            if (model?.Content == null)
            {
                return default(T);
            }

            using (DittoDisposableTimer.DebugDuration<T>($"RenderModel As ({model.Content.ContentType.Alias})"))
            {
                return model.Content.As<T>(null, null /* TODO:Culture */ , processorContexts, onConverting, onConverted);
            }
        }
    }
}