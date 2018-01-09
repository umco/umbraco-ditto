using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Extensions methods for <see cref="T:System.Type"/> for creating instances of types faster than 
    /// using reflection. Modified from the original class at.
    /// <see href="http://geekswithblogs.net/mrsteve/archive/2012/02/19/a-fast-c-sharp-extension-method-using-expression-trees-create-instance-from-type-again.aspx"/>
    /// </summary>
    internal static class TypeInitializationExtensions
    {
        /// <summary>
        /// Returns a new instance of type <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An instance of the <paramref name="type"/>.</returns>
        public static object GetInstance(this Type type)
        {
            // This is about as quick as it gets.
            return Activator.CreateInstance(type);
        }

        /// <summary>
        ///  Returns a new instance of type <paramref name="type"/> and casts to <typeparamref name="TCast"/>.
        /// </summary>
        /// <typeparam name="TCast">The type to cast to.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>An instance of the <paramref name="type"/>, casted to <typeparamref name="TCast"/>.</returns>
        public static TCast GetInstance<TCast>(this Type type)
        {
            // Cast the new instance to the desired type.
            return (TCast)Activator.CreateInstance(type);
        }

        /// <summary>
        /// Returns an instance of the <paramref name="type"/> on which the method is invoked.
        /// </summary>
        /// <typeparam name="TArg">The type of the argument to pass to the constructor.</typeparam>
        /// <param name="type">The type on which the method was invoked.</param>
        /// <param name="argument">The argument to pass to the constructor.</param>
        /// <returns>An instance of the given <paramref name="type"/>.</returns>
        public static object GetInstance<TArg>(this Type type, TArg argument)
        {
            return InstanceCreationFactory<TArg>.CreateInstanceOf(type, argument);
        }

        /// <summary>
        /// The instance creation factory for creating instances.
        /// </summary>
        /// <typeparam name="TArg">The type of the first argument to pass to the constructor.</typeparam>
        private static class InstanceCreationFactory<TArg>
        {
            /// <summary>
            /// This dictionary will hold a cache of object-creation functions, keyed by the Type to create:
            /// </summary>
            private static readonly ConcurrentDictionary<Type, Func<TArg, object>> InstanceCreationMethods
                = new ConcurrentDictionary<Type, Func<TArg, object>>();

            /// <summary>
            /// The create instance of.
            /// </summary>
            /// <param name="type">
            /// The type.
            /// </param>
            /// <param name="arg1">The first argument to pass to the constructor.</param>
            /// <param name="arg2">The second argument to pass to the constructor.</param>
            /// <param name="arg3">The third argument to pass to the constructor.</param>
            /// <returns>
            /// The <see cref="object"/>.
            /// </returns>
            public static object CreateInstanceOf(Type type, TArg arg1)
            {
                CacheInstanceCreationMethodIfRequired(type);

                return InstanceCreationMethods[type].Invoke(arg1);
            }

            /// <summary>
            /// Caches the instance creation method.
            /// </summary>
            /// <param name="type">
            /// The <see cref="Type"/> who's constructor to cache.
            /// </param>
            private static void CacheInstanceCreationMethodIfRequired(Type type)
            {
                // Bail out if we've already cached the instance creation method:
                if (InstanceCreationMethods.TryGetValue(type, out Func<TArg, object> cached))
                {
                    return;
                }

                // Get a collection of the constructor argument Types we've been given;
                Type[] constructorArgumentTypes = { typeof(TArg) };

                // Get the Constructor which matches the given argument Types:
                ConstructorInfo constructor = type.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    CallingConventions.HasThis,
                    constructorArgumentTypes,
                    new ParameterModifier[0]);

                // Get a set of Expressions representing the parameters which will be passed to the Func:
                ParameterExpression[] lamdaParameterExpressions =
                {
                    Expression.Parameter(typeof(TArg), "param1"),
                };

                // Get a set of Expressions representing the parameters which will be passed to the constructor:
                ParameterExpression[] constructorParameterExpressions =
                    lamdaParameterExpressions.Take(constructorArgumentTypes.Length).ToArray();

                // Get an Expression representing the constructor call, passing in the constructor parameters:
                NewExpression constructorCallExpression = Expression.New(constructor, constructorParameterExpressions.Cast<Expression>());

                // Compile the Expression into a Func which takes one argument and returns the constructed object:
                Func<TArg, object> constructorCallingLambda =
                    Expression.Lambda<Func<TArg, object>>(
                        constructorCallExpression,
                        lamdaParameterExpressions).Compile();

                InstanceCreationMethods.TryAdd(type, constructorCallingLambda);
            }
        }
    }
}