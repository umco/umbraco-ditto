using System;
using System.Reflection;

namespace Our.Umbraco.Ditto
{
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

        /// <summary>
        /// Checks to see if a model property is mappable by Ditto
        /// </summary>
        /// <param name="source">
        /// The source <see cref="PropertyInfo"/>.
        /// </param>
        /// <returns>
        /// True if the <see cref="PropertyInfo"/> is mappable; otherwise, false.
        /// </returns>
        public static bool IsMappable(this PropertyInfo source)
        {
            // Make sure source is readable
            if (!source.CanRead) return false;

            // Check to make sure the get method has no parameters
            var hasParams = source.GetIndexParameters().GetLength(0) > 0;
            if (hasParams) return false;

            // All checks have passed so allow it to be mapped
            return true;
        }

        /// <summary>
        /// Checks to see if the given poperty should attempt to lazy load
        /// </summary>
        /// <param name="source">The property to check</param>
        /// <returns>True if a lazy load attempt should be make</returns>
        public static bool ShouldAttemptLazyLoad(this PropertyInfo source)
        {
            if (source.HasCustomAttribute<DittoIgnoreAttribute>())
            {
                return false;
            }

            return source.HasCustomAttribute<DittoLazyAttribute>() ||
                ((source.DeclaringType.HasCustomAttribute<DittoLazyAttribute>() || Ditto.LazyLoadStrategy == LazyLoad.AllVirtuals)
                    && source.IsVirtualAndOverridable());
        }
    }
}