using System;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The Ditto lazy property attribute.
    /// Used for specifying that Ditto should lazy load this property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
    public class DittoLazyAttribute : Attribute
    { }
}