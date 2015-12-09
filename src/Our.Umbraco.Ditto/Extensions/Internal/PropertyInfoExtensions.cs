namespace Our.Umbraco.Ditto
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Extensions methods for <see cref="PropertyInfo"/>.
    /// </summary>
    internal static class PropertyInfoExtensions
    {
        /// <summary>
        /// Returns a value indicating whether a <see cref="PropertyInfo"/> is
        /// both virtual and overridable.
        /// </summary>
        /// <param name="source">
        /// The source <see cref="PropertyInfo"/>.
        /// </param>
        /// <returns>
        /// True if the <see cref="PropertyInfo"/> meets the conditions; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the given instance is null.
        /// </exception>
        public static bool IsVirtualAndOverridable(this PropertyInfo source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (!source.CanWrite)
            {
                return false;
            }

            var method = source.GetGetMethod();
            return method.IsVirtual && !method.IsFinal;
        }
    }
}