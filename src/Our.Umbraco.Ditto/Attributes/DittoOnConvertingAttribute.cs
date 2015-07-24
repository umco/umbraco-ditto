using System;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The Ditto on converting attribute.
    /// Used to identify a method on a Ditto POCO model to run as a pre conversion handler.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class DittoOnConvertingAttribute : Attribute 
    { }
}
