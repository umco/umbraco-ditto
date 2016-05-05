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
    internal class EnumerableConverterAttribute : DittoProcessorAttribute
    {
        /// <summary>
        /// Processes the value.
        /// </summary>
        /// <returns>
        /// The <see cref="object" /> representing the processed value.
        /// </returns>
        public override object ProcessValue()
        {
            object result = Value;

            var propertyIsEnumerableType = Context.PropertyDescriptor.PropertyType.IsEnumerableType()
                && !Context.PropertyDescriptor.PropertyType.IsEnumerableOfKeyValueType()
                && !(Context.PropertyDescriptor.PropertyType == typeof(string));

            if (Value != null)
            {
                var valueIsEnumerableType = Value.GetType().IsEnumerableType()
                    && !Value.GetType().IsEnumerableOfKeyValueType()
                    && !(Value is string);

                if (propertyIsEnumerableType)
                {
                    if (!valueIsEnumerableType)
                    {
                        // Property is enumerable, but value isn't, so make enumerable
                        var arr = Array.CreateInstance(Value.GetType(), 1);
                        arr.SetValue(Value, 0);
                        result = arr;
                    }
                }
                else
                {
                    if (valueIsEnumerableType)
                    {
                        // Property is not enumerable, but value is, so grab first item
                        var enumerator = ((IEnumerable)Value).GetEnumerator();
                        result = enumerator.MoveNext() ? enumerator.Current : null;
                    }
                }
            }
            else
            {
                if (propertyIsEnumerableType)
                {
                    // Value is null, but property is enumerable, so return empty enumerable
                    result = EnumerableInvocations.Empty(Context.PropertyDescriptor.PropertyType.GenericTypeArguments.First());
                }
            }

            return result;
        }
    }
}