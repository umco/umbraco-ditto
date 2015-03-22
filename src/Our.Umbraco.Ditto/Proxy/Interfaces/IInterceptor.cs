namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The Interceptor interface defining methods for intercepting properties on a type.
    /// </summary>
    internal interface IInterceptor
    {
        /// <summary>
        /// Intercepts the method in the proxy to return a replaced value.
        /// </summary>
        /// <param name="info">
        /// The <see cref="InvocationInfo"/> containing information about the current
        /// invoked method or property.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> replacing the original.
        /// </returns>
        object Intercept(InvocationInfo info);
    }
}
