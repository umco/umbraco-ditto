namespace Our.Umbraco.Ditto
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;

    using global::Umbraco.Core;

    /// <summary>
    /// Provides a unified way of converting objects to an <see cref="Enum"/>.
    /// </summary>
    public class DittoEnumConverter : DittoConverter
    {
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter,
        /// using the specified context.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="sourceType">
        /// A <see cref="T:System.Type" /> that represents the type you want to convert from.
        /// </param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (context != null && context.PropertyDescriptor != null)
            {
                var propertyType = context.PropertyDescriptor.PropertyType;

                if ((propertyType.IsEnum && typeof(IConvertible).IsAssignableFrom(propertyType)) &&

                    // We can pass null here.
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    (sourceType == null
                     || sourceType == typeof(string)
                     || sourceType == typeof(int)
                     || sourceType.IsEnum || sourceType.IsEnumerableOfType(typeof(string))
                     || sourceType == typeof(Enum[])))
                {
                    return true;
                }
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted value.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (context == null || context.PropertyDescriptor == null)
            {
                return null;
            }

            var propertyType = context.PropertyDescriptor.PropertyType;

            if (value.IsNullOrEmptyString())
            {
                // Value types return default instance.
                return propertyType.GetInstance();
            }

            if (value is string)
            {
                string strValue = (string)value;
                if (strValue.IndexOf(',') != -1)
                {
                    long convertedValue = 0;
                    var values = strValue.ToDelimitedList();

                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (string v in values)
                    {
                        // OR assignment. Stolen from ComponentModel EnumConverter.
                        convertedValue |= Convert.ToInt64((Enum)Enum.Parse(propertyType, v, true), culture);
                    }

                    return Enum.ToObject(propertyType, convertedValue);
                }

                return Enum.Parse(propertyType, strValue, true);
            }

            if (value is int)
            {
                // Should handle most cases.
                if (Enum.IsDefined(propertyType, value))
                {
                    return Enum.ToObject(propertyType, value);
                }
            }

            if (value != null)
            {
                var valueType = value.GetType();
                if (valueType.IsEnum)
                {
                    // This should work for most cases where enums base type is int.
                    return Enum.ToObject(propertyType, Convert.ToInt64(value, culture));
                }

                if (valueType.IsEnumerableOfType(typeof(string)))
                {
                    long convertedValue = 0;
                    var enumerable = ((IEnumerable<string>)value).ToList();

                    if (enumerable.Any())
                    {
                        // ReSharper disable once LoopCanBeConvertedToQuery
                        foreach (string v in enumerable)
                        {
                            convertedValue |= Convert.ToInt64((Enum)Enum.Parse(propertyType, v, true), culture);
                        }

                        return Enum.ToObject(propertyType, convertedValue);
                    }

                    return propertyType.GetInstance();
                }
            }

            var enums = value as Enum[];
            if (enums != null)
            {
                long convertedValue = 0;
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (Enum e in enums)
                {
                    convertedValue |= Convert.ToInt64(e, culture);
                }

                return Enum.ToObject(propertyType, convertedValue);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
