namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The IProxy interface defines any proxied types.
    /// </summary>
    internal interface IProxy
    {
        /// <summary>
        /// Gets or sets the interceptor.
        /// </summary>
        IInterceptor Interceptor { get; set; }
    }
}
