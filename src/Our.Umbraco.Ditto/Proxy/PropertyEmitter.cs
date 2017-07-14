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
        private static readonly MethodInfo GetInterceptor = typeof(IProxy).GetProperty("Interceptor").GetGetMethod();

        /// <summary>
        /// The intercept handler method.
        /// </summary>
        private static readonly MethodInfo InterceptorMethod = typeof(IInterceptor).GetMethod(
            "Intercept",
            new[] { typeof(MethodBase), typeof(object) });

        /// <summary>
        /// The get method from handle method.
        /// </summary>
        private static readonly MethodInfo GetMethodFromHandle = typeof(MethodBase).GetMethod("GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle) });

        /// <summary>
        /// The <see cref="NotImplementedException"/> constructor.
        /// </summary>
        private static readonly ConstructorInfo NotImplementedConstructor =
            typeof(NotImplementedException).GetConstructor(new Type[0]);

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
            const MethodAttributes MethodAttributes = MethodAttributes.Public |
                                                      MethodAttributes.HideBySig |
                                                      MethodAttributes.Virtual;

            // Define the method.
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                method.Name,
                MethodAttributes,
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
            //    throw new NotImplementedException();
            // }
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, GetInterceptor);
            Label skipThrow = il.DefineLabel();
            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Bne_Un, skipThrow);
            il.Emit(OpCodes.Newobj, NotImplementedConstructor);
            il.Emit(OpCodes.Throw);
            il.MarkLabel(skipThrow);

            // This is equivalent to: 
            // For get
            // return return (PropertyType) interceptor.Intercept(methodof(BaseType.get_Property), null);
            // For set
            // interceptor.Intercept(methodof(BaseType.set_Property), value);
            il.Emit(OpCodes.Ldtoken, method);
            il.Emit(OpCodes.Call, GetMethodFromHandle);
            il.Emit(parameter == null ? OpCodes.Ldnull : OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, InterceptorMethod);

            // Setter only.
            if (method.ReturnType == typeof(void))
            {
                il.Emit(OpCodes.Pop);
            }
            else
            {
                // Unbox the object back to the corrct type.
                il.Emit(OpCodes.Unbox_Any, method.ReturnType);
            }

            il.Emit(OpCodes.Ret);
        }
    }
}