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
        /// The <see cref="RenderModel"/> to map.
        /// </param>
        /// <param name="onMapping">
        /// The <see cref="Action{DittoMapHandlerContext}"/> to fire when mapping.
        /// </param>
        /// <param name="onMapped">
        /// The <see cref="Action{DittoMapHandlerContext}"/> to fire when mapped.
        /// </param>
        /// <typeparam name="T">
        /// The <see cref="Type"/> of items to return.
        /// </typeparam>
        /// <returns>
        /// The resolved <see cref="T"/>.
        /// </returns>
        public static T As<T>(
            this RenderModel model,
            Action<DittoMapHandlerContext> onMapping = null,
            Action<DittoMapHandlerContext> onMapped = null)
            where T : class
        {
            if (model == null)
            {
                return default(T);
            }

            using (DisposableTimer.DebugDuration<T>(string.Format("RenderModel As ({0})", model.Content.DocumentTypeAlias)))
            {
                return model.Content.As<T>(onMapping, onMapped, model.CurrentCulture);
            }
        }
    }
}