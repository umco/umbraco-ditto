using System;
using System.Collections;
using System.Linq;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// An enumerable Ditto processor that converts values to/from an enumerable based 
    /// upon the properties target type
    /// NB: It won't try to cast the inner values, just convert an enumerable so this
    /// should ideally already have occurred
    /// </summary>
    public class EnumerableConverterAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// Direction of the enumerable conversion
        /// </summary>
        public EnumerableConvertionDirection Direction { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerableConverterAttribute" /> class.
        /// </summary>
        public EnumerableConverterAttribute()
        {
            // Default to automatic 
            Direction = EnumerableConvertionDirection.Automatic;
        }

        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        public override object ProcessValue()
        {
            object result = this.Value;

            var propertyIsEnumerableType = Direction == EnumerableConvertionDirection.Automatic 
                ? this.Context.PropertyDescriptor.PropertyType.IsCastableEnumerableType()
                    && !this.Context.PropertyDescriptor.PropertyType.IsEnumerableOfKeyValueType()
                    && !(this.Context.PropertyDescriptor.PropertyType == typeof(string))
                : Direction == EnumerableConvertionDirection.ToEnumerable;

            if (this.Value != null)
            {
                var valueIsEnumerableType = this.Value.GetType().IsEnumerableType()
                    && !this.Value.GetType().IsEnumerableOfKeyValueType()
                    && !(this.Value is string);

                if (propertyIsEnumerableType)
                {
                    if (!valueIsEnumerableType)
                    {
                        // Property is enumerable, but value isn't, so make enumerable
                        var arr = Array.CreateInstance(this.Value.GetType(), 1);
                        arr.SetValue(this.Value, 0);
                        result = arr;
                    }
                }
                else
                {
                    if (valueIsEnumerableType)
                    {
                        // Property is not enumerable, but value is, so grab first item
                        var enumerator = ((IEnumerable)this.Value).GetEnumerator();
                        result = enumerator.MoveNext() ? enumerator.Current : null;
                    }
                }
            }
            else
            {
                if (propertyIsEnumerableType)
                {
                    // Value is null, but property is enumerable, so return empty enumerable
                    result = EnumerableInvocations.Empty(this.Context.PropertyDescriptor.PropertyType.GenericTypeArguments.First());
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Determines the direction of the numerable conversion
    /// </summary>
    public enum EnumerableConvertionDirection
    {
        /// <summary>
        /// Automatically convert the value based on the target property type
        /// </summary>
        Automatic,

        /// <summary>
        /// Convert to enumerable
        /// </summary>
        ToEnumerable,

        /// <summary>
        /// Convert from enumerable
        /// </summary>
        FromEnumerable
    }
}