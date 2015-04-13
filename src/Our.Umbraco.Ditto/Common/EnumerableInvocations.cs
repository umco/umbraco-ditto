namespace Our.Umbraco.Ditto
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Provides ways to return methods from <see cref="Enumerable"/> without knowing the type at runtime.
    /// Once a method is invoked for a given type then it is cached so that subsequent calls do not require
    /// any overhead compilation costs.
    /// </summary>
    internal static class EnumerableInvocations
    {
        /// <summary>
        /// The method cache for storing function implementations.
        /// </summary>
        private static readonly ConcurrentDictionary<MethodBaseCacheItem, Func<object, object>> Cache
            = new ConcurrentDictionary<MethodBaseCacheItem, Func<object, object>>();

        /// <summary>
        /// The cast method.
        /// </summary>
        private static readonly MethodInfo CastMethod =
        ((Func<IEnumerable, IEnumerable<object>>)Enumerable.Cast<object>)
        .Method
        .GetGenericMethodDefinition();

        /// <summary>
        /// The empty method.
        /// </summary>
        private static readonly MethodInfo EmptyMethod =
        ((Func<IEnumerable<object>>)Enumerable.Empty<object>)
        .Method
        .GetGenericMethodDefinition();

        /// <summary>
        /// The first or default method.
        /// </summary>
        private static readonly MethodInfo FirstOrDefaultMethod =
        ((Func<IEnumerable<object>, object>)Enumerable.FirstOrDefault)
        .Method
        .GetGenericMethodDefinition();

        /// <summary>
        /// Casts the elements of the given <see cref="IEnumerable"/> to the specified type.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to cast to.</param>
        /// <param name="value">
        /// The <see cref="IEnumerable"/> to cast the items of.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> representing the cast enumerable.
        /// </returns>
        public static object Cast(Type type, IEnumerable value)
        {
            var key = GetMethodCacheKey(type);

            Func<object, object> f;
            if (!Cache.TryGetValue(key, out f))
            {
                f = StaticMethodSingleParameter<object>(CastMethod.MakeGenericMethod(type));
                Cache[key] = f;
            }

            return f(value);
        }

        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T}"/> that has the specified type argument.
        /// </summary>
        /// <param name="type">
        /// The <see cref="Type"/> to assign to the type parameter of the returned 
        /// generic <see cref="IEnumerable{T}"/>.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> representing the empty enumerable.
        /// </returns>
        public static object Empty(Type type)
        {
            var key = GetMethodCacheKey(type);

            Func<object, object> f;
            if (!Cache.TryGetValue(key, out f))
            {
                f = StaticMethod<object>(EmptyMethod.MakeGenericMethod(type));
                Cache[key] = f;
            }

            return f(type);
        }

        /// <summary>
        /// Returns the first element of a sequence, or a default value if no element is found.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to return.</param>
        /// <param name="value">
        /// The <see cref="IEnumerable"/> to return the item from.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> representing the item.
        /// </returns>
        public static object FirstOrDefault(Type type, IEnumerable value)
        {
            var key = GetMethodCacheKey(type);

            Func<object, object> f;
            if (!Cache.TryGetValue(key, out f))
            {
                f = StaticMethodSingleParameter<object>(FirstOrDefaultMethod.MakeGenericMethod(type));
                Cache[key] = f;
            }

            return f(Cast(type, value));
        }

        /// <summary>
        /// Provides a generic way of generating a static method taking a no parameters.
        /// </summary>
        /// <param name="method">
        /// The <see cref="MethodInfo"/> to generate.
        /// </param>
        /// <typeparam name="T">
        /// The <see cref="Type"/> of the generic argument.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Func{T,Object}"/>.
        /// </returns>
        private static Func<T, object> StaticMethod<T>(MethodInfo method)
        {
            var argument = Expression.Parameter(typeof(object), "argument");
            var methodCall = Expression.Call(
                null,
                method);

            return Expression.Lambda<Func<T, object>>(
                   Expression.Convert(methodCall, typeof(object)),
                   argument).Compile();
        }

        /// <summary>
        /// Provides a generic way of generating a static method that takes a single parameter.
        /// </summary>
        /// <param name="method">
        /// The <see cref="MethodInfo"/> to generate.
        /// </param>
        /// <typeparam name="T">
        /// The <see cref="Type"/> of the generic argument.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Func{T,Object}"/>.
        /// </returns>
        private static Func<T, object> StaticMethodSingleParameter<T>(MethodInfo method)
        {
            var parameter = method.GetParameters().Single();
            var argument = Expression.Parameter(typeof(object), "argument");

            var methodCall = Expression.Call(
                null,
                method,
                Expression.Convert(argument, parameter.ParameterType));

            return Expression.Lambda<Func<T, object>>(
                   Expression.Convert(methodCall, typeof(object)),
                   argument).Compile();
        }

        /// <summary>
        /// Returns a cache key for the given method and type.
        /// </summary>
        /// <param name="type">
        /// The <see cref="Type"/> the key reflects.
        /// </param>
        /// <param name="memberName">
        /// The method name. Generated at compile time.
        /// </param>
        /// <returns>
        /// The <see cref="MethodBaseCacheItem"/> for the given method and type.
        /// </returns>
        private static MethodBaseCacheItem GetMethodCacheKey(Type type, [CallerMemberName] string memberName = null)
        {
            return new MethodBaseCacheItem(memberName, type);
        }
    }
}
