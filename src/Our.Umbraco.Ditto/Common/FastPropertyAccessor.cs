using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Provides a way to set a properties value using a combination of dynamic methods and IL generation.
    /// This is much faster 4.3x than <see cref="M:PropertyInfo.SetValue"/> as it avoids the normal overheads of reflection.
    /// Once a method is invoked for a given type then it is cached so that subsequent calls do not require
    /// any overhead compilation costs.
    /// </summary>
    internal class FastPropertyAccessor : CachedInvocations
    {
        /// <summary>
        /// Gets the value of the property on the given instance.
        /// </summary>
        /// <param name="property">The property to set.</param>
        /// <param name="instance">The current instance to return the property from.</param>
        /// <returns>The <see cref="object"/> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetValue(PropertyInfo property, object instance)
        {
            var key = GetMethodCacheKey(property);

            Func<object, object> a;
            if (!FunctionCache.TryGetValue(key, out a))
            {
                a = MakeGetMethod(property.GetGetMethod(), property.PropertyType);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(PropertyInfo property, object instance, object value)
        {
            var key = GetMethodCacheKey(property);

            Action<object, object> a;
            if (!ActionCache.TryGetValue(key, out a))
            {
                a = MakeSetMethod(property.GetSetMethod(), property.PropertyType);
                ActionCache[key] = a;
            }

            a(instance, value);
        }

        /// <summary>
        /// Builds the get accessor for the given type.
        /// </summary>
        /// <param name="method">The method to compile.</param>
        /// <returns>The <see cref="Action{Object, Object}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<object, object> MakeGetMethod(MethodInfo method, Type propertyType)
        {
            Type type = method.DeclaringType;
            DynamicMethod dmethod = new DynamicMethod("Getter", typeof(object), new[] { typeof(object) }, type, true);
            ILGenerator il = dmethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0); // Load our value to the stack.
            il.Emit(OpCodes.Castclass, type);// Cast to the value type.

            // Call the set method.
            il.Emit(OpCodes.Callvirt, method);

            if (propertyType.IsValueType)
            {
                // Cast if a value type to set the correct type.
                il.Emit(OpCodes.Box, propertyType);
            }

            // Return thr result.
            il.Emit(OpCodes.Ret);

            return (Func<object, object>)dmethod.CreateDelegate(typeof(Func<object, object>));
        }

        /// <summary>
        /// Builds the set accessor for the given type.
        /// </summary>
        /// <param name="method">The method to compile.</param>
        /// <param name="propertyType">The property type.</param>
        /// <returns>The <see cref="Action{Object, Object}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Action<object, object> MakeSetMethod(MethodInfo method, Type propertyType)
        {
            Type type = method.DeclaringType;
            DynamicMethod dmethod = new DynamicMethod("Setter", typeof(void), new[] { typeof(object), typeof(object) }, type, true);
            ILGenerator il = dmethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0); // Load our instance to the stack.
            il.Emit(OpCodes.Castclass, type); // Cast to the instance type.
            il.Emit(OpCodes.Ldarg_1); // Load our value to the stack.

            if (propertyType.IsValueType)
            {
                // Cast if a value type to set the correct type.
                il.Emit(OpCodes.Unbox_Any, propertyType);
            }

            // Call the set method and return.
            il.Emit(OpCodes.Callvirt, method);
            il.Emit(OpCodes.Ret);

            return (Action<object, object>)dmethod.CreateDelegate(typeof(Action<object, object>));
        }
    }
}
