using System;

namespace Our.Umbraco.Ditto
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class DittoOnConvertingAttribute : Attribute 
    { }
}
