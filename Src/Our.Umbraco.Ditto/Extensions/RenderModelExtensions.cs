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
			using (var t = DisposableTimer.DebugDuration<T>(string.Format("As ({0})", model.Content.DocumentTypeAlias)))
			{
				return model.Content.As<T>(convertingType, convertedType);
			}
		}
	}
}