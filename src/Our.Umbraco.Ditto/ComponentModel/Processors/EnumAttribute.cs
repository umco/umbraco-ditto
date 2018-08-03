using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// An enum Ditto processor
    /// </summary>
    public class EnumAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        public override object ProcessValue()
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (this.Context == null || this.Context.PropertyInfo == null)
            {
                return null;
            }

            var propertyType = this.Context.PropertyInfo.PropertyType;

            if (this.Value == null || (this.Value is string value && string.IsNullOrWhiteSpace(value)))
            {
                // Value types return default instance.
                return propertyType.GetInstance();
            }

            if (this.Value is string strValue)
            {
                if (strValue.IndexOf(',') != -1)
                {
                    long convertedValue = 0;
                    var values = strValue.ToDelimitedList();

                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (string v in values)
                    {
                        // OR assignment. Stolen from ComponentModel EnumConverter.
                        convertedValue |= Convert.ToInt64((Enum)Enum.Parse(propertyType, v, true), this.Context.Culture);
                    }

                    return Enum.ToObject(propertyType, convertedValue);
                }

                return Enum.Parse(propertyType, strValue, true);
            }

            if (this.Value is int)
            {
                // Should handle most cases.
                if (Enum.IsDefined(propertyType, this.Value))
                {
                    return Enum.ToObject(propertyType, this.Value);
                }
            }

            if (this.Value != null)
            {
                var valueType = this.Value.GetType();
                if (valueType.IsEnum)
                {
                    // This should work for most cases where enums base type is int.
                    return Enum.ToObject(propertyType, Convert.ToInt64(this.Value, this.Context.Culture));
                }

                if (valueType.IsEnumerableOfType(typeof(string)))
                {
                    long convertedValue = 0;
                    var enumerable = ((IEnumerable<string>)this.Value).ToList();

                    if (enumerable.Any())
                    {
                        // ReSharper disable once LoopCanBeConvertedToQuery
                        foreach (string v in enumerable)
                        {
                            convertedValue |= Convert.ToInt64((Enum)Enum.Parse(propertyType, v, true), this.Context.Culture);
                        }

                        return Enum.ToObject(propertyType, convertedValue);
                    }

                    return propertyType.GetInstance();
                }
            }

            if (this.Value is Enum[] enums)
            {
                long convertedValue = 0;

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (Enum e in enums)
                {
                    convertedValue |= Convert.ToInt64(e, this.Context.Culture);
                }

                return Enum.ToObject(propertyType, convertedValue);
            }

            return null;
        }
    }
}