using System;
using System.Reflection;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Extensions methods for <see cref="MemberInfo"/>.
    /// </summary>
    internal static class MemberInfoExtensions
    {
        /// <summary>
        /// Checks to see if the member has the specified attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool HasCustomAttribute<T>(this MemberInfo element) where T : Attribute
        {
            return element.GetCustomAttribute<T>() != null;
        }
    }
}