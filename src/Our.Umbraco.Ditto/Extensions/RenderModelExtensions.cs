using System.Collections.Generic;

namespace Our.Umbraco.Ditto
{
    using System;

    using global::Umbraco.Core;
    using global::Umbraco.Web.Models;

    /// <summary>
    /// Encapsulates extension methods for <see cref="RenderModel"/>.
    /// </summary>
    public static class RenderModelExtensions
    {
        /// <summary>
        /// Returns the given instance of <see cref="RenderModel"/> as the specified type.
        /// </summary>
        /// <param name="model">
        /// The <see cref="RenderModel"/> to convert.
        /// </param>
        /// <param name="valueResolverContexts">
        /// A collection of <see cref="DittoValueResolverContext"/> entities to use whilst resolving values.
        /// </param>
        /// <param name="onConverting">
        /// The <see cref="Action{ConversionHandlerContext}"/> to fire when converting.
        /// </param>
        /// <param name="onConverted">
        /// The <see cref="Action{ConversionHandlerContext}"/> to fire when converted.
        /// </param>
        /// <typeparam name="T">
        /// The <see cref="Type"/> of items to return.
        /// </typeparam>
        /// <returns>
        /// The resolved <see cref="T"/>.
        /// </returns>
        public static T As<T>(
            this RenderModel model,
            IEnumerable<DittoValueResolverContext> valueResolverContexts = null,
            Action<DittoConversionHandlerContext> onConverting = null,
            Action<DittoConversionHandlerContext> onConverted = null)
            where T : class
        {
            if (model == null)
            {
                return default(T);
            }

            using (DisposableTimer.DebugDuration<T>(string.Format("RenderModel As ({0})", model.Content.DocumentTypeAlias)))
            {
                return model.Content.As<T>(model.CurrentCulture, null, valueResolverContexts, onConverting, onConverted);
            }
        }
    }
}