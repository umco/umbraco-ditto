using System;
using Umbraco.Core;
using Umbraco.Web.Models;

namespace Our.Umbraco.Ditto
{
	public static class RenderModelExtensions
	{
		public static T As<T>(this RenderModel model,
			Action<ConvertingTypeEventArgs> convertingType = null,
			Action<ConvertedTypeEventArgs> convertedType = null)
			where T : RenderModel
		{
			if (model == null)
				return default(T);

			using (DisposableTimer.DebugDuration<T>(string.Format("RenderModel As ({0})", model.Content.DocumentTypeAlias)))
			{
				return model.Content.As<T>(convertingType, convertedType);
			}
		}
	}
}