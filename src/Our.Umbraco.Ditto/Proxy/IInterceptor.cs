namespace Our.Umbraco.Ditto
{
    using System.Reflection;

    /// <summary>
    /// The IInterceptor interface provides methods for intercepting <see cref="MethodBase"/> implementations
    /// in proxy classes.
    /// </summary>
    /// <remarks>This needs to be public in order for a proxy to implement it.</remarks>
    public interface IInterceptor
    {
        /// <summary>
        /// Intercepts the <see cref="MethodBase"/> in the proxy to return a replaced value.
        /// </summary>
        /// <param name="methodBase">
        /// The <see cref="MethodBase"/> containing information about the current
        /// invoked property.
        /// </param>
        /// <param name="value">
        /// The object to set the <see cref="MethodBase"/> to if it is a setter.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> replacing the original implementation value.
        /// </returns>
        object Intercept(MethodBase methodBase, object value);
    }
}