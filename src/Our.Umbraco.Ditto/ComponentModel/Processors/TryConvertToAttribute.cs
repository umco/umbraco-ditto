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
        public override object ProcessValue()
        {
            var result = this.Value;

            if (this.Value != null && this.Context.PropertyInfo.PropertyType.IsInstanceOfType(this.Value) == false)
            {
#if DEBUG
                using (ProfilingLogger?.DebugDuration<TryConvertToAttribute>($"TryConvertTo '{this.Context.PropertyInfo.Name}' ({this.Context.Content.Id})"))
                {
#endif
                    var convert = this.Value.TryConvertTo(this.Context.PropertyInfo.PropertyType);
                    if (convert.Success)
                    {
                        result = convert.Result;
                    }
#if DEBUG
                }
#endif
            }

            return result;
        }
    }
}