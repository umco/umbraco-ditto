using System;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The Umbraco property processor attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AltUmbracoPropertyProcessorAttribute : UmbracoPropertyProcessorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AltUmbracoPropertyProcessorAttribute"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <param name="defaultValue">The default value.</param>
        public AltUmbracoPropertyProcessorAttribute(string propertyName, bool recursive = false, object defaultValue = null)
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
                return base.ProcessValue(Context.Content, Context, Culture);
            }

            return Value;
        }
    }
}