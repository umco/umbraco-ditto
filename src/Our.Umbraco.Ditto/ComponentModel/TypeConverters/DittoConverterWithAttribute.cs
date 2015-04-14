namespace Our.Umbraco.Ditto
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// An extension of the <see cref="DittoConverter"/> class which accepts an <see cref="System.Attribute"/> as a generic type.
    /// </summary>
    /// <typeparam name="T">An <see cref="System.Attribute"/> type</typeparam>
    public abstract class DittoConverterWithAttribute<T> : DittoConverter where T : Attribute
    {
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type" /> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (context != null && context.PropertyDescriptor != null && context.PropertyDescriptor.Attributes != null)
            {
                return context.PropertyDescriptor.Attributes
                    .OfType<T>()
                    .Any();
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The <see cref="Type"/> of attribute to return.
        /// </returns>
        protected virtual T GetAttribute(ITypeDescriptorContext context)
        {
            if (context != null && context.PropertyDescriptor != null && context.PropertyDescriptor.Attributes != null)
            {
                var attributes = context.PropertyDescriptor.Attributes.OfType<T>();

                if (attributes.Any())
                {
                    return attributes.FirstOrDefault() as T;
                }
            }

            return default(T);
        }
    }
}