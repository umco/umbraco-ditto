// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DittoIgnoreAttribute.cs" company="Umbrella Inc, Our Umbraco and other contributors">
//   Copyright Umbrella Inc, Our Umbraco and other contributors
// </copyright>
// <summary>
//   The Ditto ignore property attribute. Used for specifying that Umbraco should
//   ignore this property during conversion.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Our.Umbraco.Ditto.Attributes
{
    using System;

    /// <summary>
    /// The Ditto ignore property attribute. Used for specifying that Umbraco should
    /// ignore this property during conversion.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DittoIgnoreAttribute : Attribute
    {
    }
}