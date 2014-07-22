using System.ComponentModel;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
	public class ConvertingTypeEventArgs : CancelEventArgs
	{
		public IPublishedContent Content { get; set; }
	}
}