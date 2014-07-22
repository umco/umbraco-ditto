using System;

namespace Our.Umbraco.Ditto
{
	public static class EventHandlers
	{
		public static event EventHandler<ConvertingTypeEventArgs> ConvertingType;

		public static event EventHandler<ConvertedTypeEventArgs> ConvertedType;

		public static void CallConvertingTypeHandler(ConvertingTypeEventArgs args)
		{
			if (ConvertingType != null)
				ConvertingType(null, args);
		}

		public static void CallConvertedTypeHandler(ConvertedTypeEventArgs args)
		{
			if (ConvertedType != null)
				ConvertedType(null, args);
		}
	}
}