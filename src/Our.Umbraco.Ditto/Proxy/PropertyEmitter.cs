using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Provides methods for emitting intercepted property bodies in proxy classes.
    /// </summary>
    internal static class PropertyEmitter
    {
        /// <summary>
        /// The get interceptor method.
        /// </summary>
        // ReSharper disable once PossibleNullReferenceException
        private static readonly MethodInfo GetInterceptor = typeof(IProxy).GetProperty(nameof(IProxy.Interceptor)).GetGetMethod();

        /// <summary>
        /// The intercept handler method.
        /// </summary>
        private static readonly MethodInfo InterceptorMethod = typeof(IInterceptor).GetMethod(
            "Intercept",
            new[] { typeof(MethodBase), typeof(IProxy) });

        /// <summary>
        /// The get method from handle method.
        /// </summary>
        private static readonly MethodInfo GetMethodFromHandle = typeof(MethodBase).GetMethod("GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle) });

        /// <summary>
        /// The <see cref="ArgumentNullException"/> constructor.
        /// </summary>
        private static readonly ConstructorInfo ArgumentNullExceptionConstructor =
            typeof(ArgumentNullException).GetConstructor(new Type[0]);

        /// <summary>
        /// Uses reflection to emit the given <see cref="MethodInfo"/> body for interception.
        /// </summary>
        /// <param name="typeBuilder">
        /// The <see cref="TypeBuilder"/> for the current type.
        /// </param>
        /// <param name="method">
        /// The <see cref="MethodInfo"/> to intercept.
        /// </param>
        /// <param name="interceptorField">
        /// The <see cref="IInterceptor"/> field.
        /// </param>
        public static void Emit(TypeBuilder typeBuilder, MethodInfo method, FieldInfo interceptorField)
        {
            // Get the method parameters for any setters.
            ParameterInfo[] parameters = method.GetParameters();
            ParameterInfo parameter = parameters.FirstOrDefault();

            // Define attributes.
            const MethodAttributes methodAttributes = MethodAttributes.Public |
                                                      MethodAttributes.HideBySig |
                                                      MethodAttributes.Virtual;

            // Define the method.
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                method.Name,
                methodAttributes,
                CallingConventions.HasThis,
                method.ReturnType,
                parameters.Select(param => param.ParameterType).ToArray());

            ILGenerator il = methodBuilder.GetILGenerator();

            // Set the correct flags to signal the property is managed and implemented in intermediate language.
            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            methodBuilder.SetImplementationFlags(MethodImplAttributes.Managed | MethodImplAttributes.IL);

            // This is the equivalent to:
            // IInterceptor interceptor = ((IProxy)this).Interceptor;
            // if (interceptor == null)
            // {
            //    throw new ArgumentNullException();
            // }
            il.Emit(OpCodes.Ldarg_0); // this
            il.Emit(OpCodes.Callvirt, GetInterceptor); // .Interceptor
            Label skipThrow = il.DefineLabel();
            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Bne_Un, skipThrow);
            il.Emit(OpCodes.Newobj, ArgumentNullExceptionConstructor);
            il.Emit(OpCodes.Throw);
            il.MarkLabel(skipThrow);

            // This is equivalent to:
            // return return (PropertyType) interceptor.Intercept(methodof(BaseType.get_Property), null);
            il.Emit(OpCodes.Ldtoken, method);
            il.Emit(OpCodes.Call, GetMethodFromHandle);

            if (parameter == null)
            {
                // Getter
                // return interceptor.Intercept(MethodBase.GetMethodFromHandle(typeof(BaseType).GetMethod("get_PropertyName").MethodHandle), null);
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Callvirt, InterceptorMethod);

                // Unbox the object back to the correct type.
                il.Emit(OpCodes.Unbox_Any, method.ReturnType);
            }
            else
            {
                // Setter
                // interceptor.Intercept(MethodBase.GetMethodFromHandle(typeof(BaseType).GetMethod("set_PropertyName", new Type[] { typeof(PropertyType) }).MethodHandle), value);
                il.Emit(OpCodes.Ldarg_1);

                if (parameter.ParameterType.IsValueType)
                {
                    il.Emit(OpCodes.Box, parameter.ParameterType);
                }

                il.Emit(OpCodes.Callvirt, InterceptorMethod);
                il.Emit(OpCodes.Pop); // Clear the stack
            }

            il.Emit(OpCodes.Ret);
        }
    }
}