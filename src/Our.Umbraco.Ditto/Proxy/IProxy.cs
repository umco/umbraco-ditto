using System.Reflection;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The IProxy interface prides necessary properties to proxy base classes.
    /// </summary>
    /// <remarks>This needs to be public in order for a proxy to implement it.</remarks>
    public interface IProxy
    {
        /// <summary>
        /// Gets or sets the <see cref="IInterceptor"/> for intercepting <see cref="MethodBase"/> calls.
        /// </summary>
        IInterceptor Interceptor { get; set; }
    }
}