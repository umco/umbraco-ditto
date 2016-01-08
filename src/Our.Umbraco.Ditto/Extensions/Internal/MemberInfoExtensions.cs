using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Extension methods for <see cref="MemberInfo"/>.
    /// </summary>
    internal static class MemberInfoExtensions
    {
        /// <summary>
        /// Retrieves a collection of custom attributes of an exact specified type that are applied to a specified member, and optionally inspects the ancestors of that member.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info">The member information.</param>
        /// <param name="inherit">if set to <c>true</c> [inherit].</param>
        /// <returns></returns>
        public static IEnumerable<T> GetCustomAttributesExact<T>(this MemberInfo info, bool inherit) where T : Attribute
        {
            return info.GetCustomAttributes<T>(inherit).Where(x => x.GetType() == typeof(T));
        }

        /// <summary>
        /// Retrieves a custom attributes of the exact specified type that is applied to a specified member, and optionally inspects the ancestors of that member.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info">The member information.</param>
        /// <param name="inherit">if set to <c>true</c> [inherit].</param>
        /// <returns></returns>
        public static T GetCustomAttributeExact<T>(this MemberInfo info, bool inherit) where T : Attribute
        {
            return info.GetCustomAttributesExact<T>(inherit).SingleOrDefault();
        }
    }
}