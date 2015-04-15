namespace Our.Umbraco.Ditto
{
    using System;
    using System.Globalization;
    using System.Reflection;

    using global::Umbraco.Core.Models;

    /// <summary>
    /// The Ditto value attribute, defines how a property can get its value.
    /// All other Ditto value attributes should inherit from this class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class DittoValueAttribute : Attribute
    {
        /// <summary>
        /// Returns the value for the given type and property.
        /// </summary>
        /// <param name="content">The <see cref="IPublishedContent"/> to convert.</param>
        /// <param name="type">The <see cref="Type"/> of items to return.</param>
        /// <param name="culture">The <see cref="CultureInfo"/></param>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> property info associated with the type.</param>
        /// <returns>The <see cref="object"/> representing the value.</returns>
        public abstract object GetValue(
            IPublishedContent content,
            Type type,
            CultureInfo culture,
            PropertyInfo propertyInfo);
    }
}