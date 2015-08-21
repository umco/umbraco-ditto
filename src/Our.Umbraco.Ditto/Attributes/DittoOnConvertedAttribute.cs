namespace Our.Umbraco.Ditto
{
    using System;

    /// <summary>
    /// The Ditto on converted attribute.
    /// Used to identify a method on a Ditto POCO model to run as a post conversion handler.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class DittoOnConvertedAttribute : Attribute
    {
    }
}