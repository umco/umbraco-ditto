using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto.Tests.Mocks
{
	public class PropertyBuilder
	{
		private string _alias = "alias";
		private object _value = null;
		private bool _hasValue = true;

		public static PropertyBuilder Default(string alias, object value)
		{
			return new PropertyBuilder
			{
				_alias = alias,
				_value = value
			};
		}


		public IPublishedContentProperty Build()
		{
			return new PublishedContentPropertyMock(_alias, _value, _hasValue);
		}
	}
}