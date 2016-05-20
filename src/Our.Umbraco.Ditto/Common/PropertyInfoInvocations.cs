namespace Our.Umbraco.Ditto
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Provides a way to set a properties value using delegates. 
    /// This is much faster than <see cref="M:PropertyInfo.SetValue"/> as it avois the normal overheads of reflection.
    /// Once a method is invoked for a given type then it is cached so that subsequent calls do not require
    /// any overhead compilation costs.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class PropertyInfoInvocations : CachedInvocations
    {
        /// <summary>
        /// Gets the value of the property on the given instance. 
        /// </summary>
        /// <param name="property">The property to set.</param>
        /// <param name="instance">The current instance to return the property from.</param>
        /// <returns>The <see cref="object"/> value.</returns>
        public static object GetValue(PropertyInfo property, object instance)
        {
            var key = GetMethodCacheKey(property);

            Func<object, object> a;
            if (!FunctionCache.TryGetValue(key, out a))
            {
                a = BuildGetAccessor(property.GetGetMethod());
                FunctionCache[key] = a;
            }

            return a(instance);
        }

        /// <summary>
        /// Set the value of the property on the given instance. 
        /// </summary>
        /// <param name="property">The property to set.</param>
        /// <param name="instance">The current instance to assign the property to.</param>
        /// <param name="value">The value to set.</param>
        public static void SetValue(PropertyInfo property, object instance, object value)
        {
            var key = GetMethodCacheKey(property);

            Action<object, object> a;
            if (!ActionCache.TryGetValue(key, out a))
            {
                a = BuildSetAccessor(property.GetSetMethod());
                ActionCache[key] = a;
            }

            if (a != null)
            {
                a(instance, value);
            }
        }

        /// <summary>
        /// Builds the get accessor for the given type.
        /// </summary>
        /// <param name="method">The method to compile.</param>
        /// <returns>The <see cref="Action{Object, Object}"/></returns>
        private static Func<object, object> BuildGetAccessor(MethodInfo method)
        {
            var obj = Expression.Parameter(typeof(object), "o");

            if (method.DeclaringType != null)
            {
                Expression<Func<object, object>> expr =
                    Expression.Lambda<Func<object, object>>(
                        Expression.Convert(
                            Expression.Call(
                                Expression.Convert(obj, method.DeclaringType),
                                method),
                            typeof(object)),
                        obj);

                return expr.Compile();
            }

            return null;
        }

        /// <summary>
        /// Builds the set accessor for the given type.
        /// </summary>
        /// <param name="method">The method to compile.</param>
        /// <returns>The <see cref="Action{Object, Object}"/></returns>
        private static Action<object, object> BuildSetAccessor(MethodInfo method)
        {
            var obj = Expression.Parameter(typeof(object), "o");
            var value = Expression.Parameter(typeof(object));

            if (method != null && method.DeclaringType != null)
            {
                Expression<Action<object, object>> expr =
                    Expression.Lambda<Action<object, object>>(
                        Expression.Call(
                            Expression.Convert(obj, method.DeclaringType),
                            method,
                            Expression.Convert(value, method.GetParameters()[0].ParameterType)),
                        obj,
                        value);

                return expr.Compile();
            }

            return null;
        }
    }
}
