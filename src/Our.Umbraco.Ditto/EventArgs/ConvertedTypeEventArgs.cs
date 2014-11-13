using System;
using Umbraco.Core.Models;

namespace Our.Umbraco.Ditto
{
    public class ConvertedTypeEventArgs : EventArgs
    {
        public IPublishedContent Content { get; set; }

        public object Converted { get; set; }

        public Type ConvertedType { get; set; }
    }
}