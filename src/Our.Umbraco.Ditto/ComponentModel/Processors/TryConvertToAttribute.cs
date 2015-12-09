using Umbraco.Core;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// A TryConvertTo Ditto processor
    /// </summary>
    internal class TryConvertToAttribute : DittoProcessorAttribute
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
            var result = Value;

            if (Value != null && !Context.PropertyDescriptor.PropertyType.IsInstanceOfType(Value))
            {
                // TODO: Maybe support enumerables?
                using (DittoDisposableTimer.DebugDuration<object>(string.Format("TryConvertTo ({0}, {1})", Context.Content.Id, Context.PropertyDescriptor.Name)))
                {
                    var convert = Value.TryConvertTo(Context.PropertyDescriptor.PropertyType);
                    if (convert.Success)
                    {
                        result = convert.Result;
                    }
                }
            }

            return result;
        }
    }
}