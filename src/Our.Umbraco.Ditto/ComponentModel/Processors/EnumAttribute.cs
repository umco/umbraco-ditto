using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EnumAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override object ProcessValue()
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (Context == null || Context.PropertyDescriptor == null)
            {
                return null;
            }

            var propertyType = Context.PropertyDescriptor.PropertyType;

            if (Value.IsNullOrEmptyString())
            {
                // Value types return default instance.
                return propertyType.GetInstance();
            }

            if (Value is string)
            {
                string strValue = (string)Value;
                if (strValue.IndexOf(',') != -1)
                {
                    long convertedValue = 0;
                    var values = strValue.ToDelimitedList();

                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (string v in values)
                    {
                        // OR assignment. Stolen from ComponentModel EnumConverter.
                        convertedValue |= Convert.ToInt64((Enum)Enum.Parse(propertyType, v, true), Context.Culture);
                    }

                    return Enum.ToObject(propertyType, convertedValue);
                }

                return Enum.Parse(propertyType, strValue, true);
            }

            if (Value is int)
            {
                // Should handle most cases.
                if (Enum.IsDefined(propertyType, Value))
                {
                    return Enum.ToObject(propertyType, Value);
                }
            }

            if (Value != null)
            {
                var valueType = Value.GetType();
                if (valueType.IsEnum)
                {
                    // This should work for most cases where enums base type is int.
                    return Enum.ToObject(propertyType, Convert.ToInt64(Value, Context.Culture));
                }

                if (valueType.IsEnumerableOfType(typeof(string)))
                {
                    long convertedValue = 0;
                    var enumerable = ((IEnumerable<string>)Value).ToList();

                    if (enumerable.Any())
                    {
                        // ReSharper disable once LoopCanBeConvertedToQuery
                        foreach (string v in enumerable)
                        {
                            convertedValue |= Convert.ToInt64((Enum)Enum.Parse(propertyType, v, true), Context.Culture);
                        }

                        return Enum.ToObject(propertyType, convertedValue);
                    }

                    return propertyType.GetInstance();
                }
            }

            var enums = Value as Enum[];
            if (enums != null)
            {
                long convertedValue = 0;
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (Enum e in enums)
                {
                    convertedValue |= Convert.ToInt64(e, Context.Culture);
                }

                return Enum.ToObject(propertyType, convertedValue);
            }

            return null;
        }
    }
}
