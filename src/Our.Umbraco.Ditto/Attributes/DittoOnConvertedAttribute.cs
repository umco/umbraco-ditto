using System;

namespace Our.Umbraco.Ditto.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class DittoOnConvertedAttribute : Attribute
    { }
}
