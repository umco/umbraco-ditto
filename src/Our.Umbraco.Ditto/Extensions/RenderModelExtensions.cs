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
        /// <param name="convertingType">
        /// The <see cref="Action{ConvertingTypeEventArgs}"/> to fire when converting.
        /// </param>
        /// <param name="convertedType">
        /// The <see cref="Action{ConvertedTypeEventArgs}"/> to fire when converted.
        /// </param>
        /// <typeparam name="T">
        /// The <see cref="Type"/> of items to return.
        /// </typeparam>
        /// <returns>
        /// The resolved <see cref="T"/>.
        /// </returns>
        public static T As<T>(
            this RenderModel model,
            Action<ConvertingTypeEventArgs> convertingType = null,
            Action<ConvertedTypeEventArgs> convertedType = null)
            where T : class
        {
            if (model == null)
            {
                return default(T);
            }

            using (DisposableTimer.DebugDuration<T>(string.Format("RenderModel As ({0})", model.Content.DocumentTypeAlias)))
            {
                return model.Content.As<T>(convertingType, convertedType, model.CurrentCulture);
            }
        }
    }
}