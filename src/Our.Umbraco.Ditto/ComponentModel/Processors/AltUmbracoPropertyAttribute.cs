using System;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The alt Umbraco property processor attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class AltUmbracoPropertyAttribute : UmbracoPropertyAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AltUmbracoPropertyAttribute"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <param name="defaultValue">The default value.</param>
        public AltUmbracoPropertyAttribute(string propertyName, bool recursive = false, object defaultValue = null)
            : base(propertyName, recursive, defaultValue)
        { }

        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        public override object ProcessValue()
        {
            if (Value.IsNullOrEmptyString())
            {
                // Reset value to published content
                Value = Context.Content;

                // Run base processor
                return base.ProcessValue();
            }

            return Value;
        }
    }
}