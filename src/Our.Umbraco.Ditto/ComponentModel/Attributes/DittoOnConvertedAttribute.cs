using System;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The Ditto on converted attribute.
    /// Used to identify a method on a Ditto POCO model to run as a post conversion handler.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class DittoOnConvertedAttribute : Attribute
    { }
}