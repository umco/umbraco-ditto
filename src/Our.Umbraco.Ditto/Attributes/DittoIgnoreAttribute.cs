using System;

namespace Our.Umbraco.Ditto
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DittoIgnoreAttribute : Attribute
    {
    }
}