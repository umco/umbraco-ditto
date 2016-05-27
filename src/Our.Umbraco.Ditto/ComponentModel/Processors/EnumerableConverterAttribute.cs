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
            object result = this.Value;

            var propertyIsEnumerableType = this.Context.PropertyDescriptor.PropertyType.IsEnumerableType()
                && !this.Context.PropertyDescriptor.PropertyType.IsEnumerableOfKeyValueType()
                && !(this.Context.PropertyDescriptor.PropertyType == typeof(string));

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
}