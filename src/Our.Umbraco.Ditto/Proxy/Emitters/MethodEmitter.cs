namespace Our.Umbraco.Ditto
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// Emits instruction for producing proxy method calls.
    /// </summary>
    internal class MethodEmitter
    {
        /// <summary>
        /// The get interceptor method.
        /// </summary>
        private static readonly MethodInfo GetInterceptor;

        /// <summary>
        /// The get generic method from handle method.
        /// </summary>
        private static readonly MethodInfo GetGenericMethodFromHandle =
            typeof(MethodBase).GetMethod(
                "GetMethodFromHandle",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) },
                null);

        /// <summary>
        /// The get method from handle method.
        /// </summary>
        private static readonly MethodInfo GetMethodFromHandle = typeof(MethodBase).GetMethod("GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle) });

        /// <summary>
        /// The get type from handle method.
        /// </summary>
        private static readonly MethodInfo GetTypeFromHandle = typeof(Type).GetMethod("GetTypeFromHandle");

        /// <summary>
        /// The intercept handler method.
        /// </summary>
        private static readonly MethodInfo HandlerMethod = typeof(IInterceptor).GetMethod("Intercept");

        /// <summary>
        /// The info constructor.
        /// </summary>
        private static readonly ConstructorInfo InfoConstructor;

        /// <summary>
        /// The interceptor property.
        /// </summary>
        private static readonly PropertyInfo InterceptorProperty = typeof(IProxy).GetProperty("Interceptor");

        /// <summary>
        /// The not implemented constructor.
        /// </summary>
        private static readonly ConstructorInfo NotImplementedConstructor =
            typeof(NotImplementedException).GetConstructor(new Type[0]);

        /// <summary>
        /// The argument handler.
        /// </summary>
        private readonly ArgumentHandler argumentHandler;

        /// <summary>
        /// Initializes static members of the <see cref="MethodEmitter"/> class.
        /// </summary>
        static MethodEmitter()
        {
            GetInterceptor = InterceptorProperty.GetGetMethod();
            Type[] constructorTypes =
                {
                    typeof(object), typeof(MethodInfo), typeof(StackTrace), typeof(Type[]),
                    typeof(object[])
                };

            InfoConstructor = typeof(InvocationInfo).GetConstructor(constructorTypes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodEmitter"/> class.
        /// </summary>
        public MethodEmitter()
        {
            this.argumentHandler = new ArgumentHandler();
        }

        /// <summary>
        /// Outputs the current method.
        /// </summary>
        /// <param name="il">
        /// The <see cref="ILGenerator"/>.
        /// </param>
        /// <param name="method">
        /// The method to emit.
        /// </param>
        /// <param name="field">
        /// The field.
        /// </param>
        public void EmitMethodBody(ILGenerator il, MethodInfo method, FieldInfo field)
        {
            const bool IsStatic = false;

            ParameterInfo[] parameters = method.GetParameters();
            il.DeclareLocal(typeof(object[]));
            il.DeclareLocal(typeof(InvocationInfo));
            il.DeclareLocal(typeof(Type[]));

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, GetInterceptor);

            Label skipThrow = il.DefineLabel();

            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Bne_Un, skipThrow);

            il.Emit(OpCodes.Newobj, NotImplementedConstructor);
            il.Emit(OpCodes.Throw);

            il.MarkLabel(skipThrow);

            // Push the 'this' pointer onto the stack
            il.Emit(OpCodes.Ldarg_0);

            // Push the MethodInfo onto the stack
            Type declaringType = method.DeclaringType;

            il.Emit(OpCodes.Ldtoken, method);
            if (declaringType != null && declaringType.IsGenericType)
            {
                il.Emit(OpCodes.Ldtoken, declaringType);
                il.Emit(OpCodes.Call, GetGenericMethodFromHandle);
            }
            else
            {
                il.Emit(OpCodes.Call, GetMethodFromHandle);
            }

            il.Emit(OpCodes.Castclass, typeof(MethodInfo));

            this.PushStackTrace(il);
            this.PushGenericArguments(method, il);
            this.argumentHandler.PushArguments(parameters, il, IsStatic);

            // InvocationInfo info = new InvocationInfo(...);
            il.Emit(OpCodes.Newobj, InfoConstructor);
            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Callvirt, HandlerMethod);

            SaveRefArguments(il, parameters);
            this.PackageReturnType(method, il);

            il.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Saves any reference arguments.
        /// </summary>
        /// <param name="il">
        /// The <see cref="ILGenerator"/>.
        /// </param>
        /// <param name="parameters">
        /// The array of parameters.
        /// </param>
        private static void SaveRefArguments(ILGenerator il, ParameterInfo[] parameters)
        {
            // Save the arguments returned from the handler method
            MethodInfo getArguments = typeof(InvocationInfo).GetMethod("get_Arguments");
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Call, getArguments);
            il.Emit(OpCodes.Stloc_0);

            foreach (ParameterInfo param in parameters)
            {
                string typeName = param.ParameterType.Name;

                bool isRef = param.ParameterType.IsByRef && typeName.EndsWith("&");
                if (!isRef)
                {
                    continue;
                }

                // Load the destination address
                il.Emit(OpCodes.Ldarg, param.Position + 1);

                // Load the argument value
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ldc_I4, param.Position);

                var referenceInstruction = OpCodes.Ldelem_Ref;
                il.Emit(referenceInstruction);

                Type unboxedType = param.ParameterType.IsByRef 
                    ? param.ParameterType.GetElementType() 
                    : param.ParameterType;

                il.Emit(OpCodes.Unbox_Any, unboxedType);

                OpCode stind = GetStindInstruction(param.ParameterType);
                il.Emit(stind);
            }
        }

        /// <summary>
        /// Returns the correct instruction for the parameter type.
        /// </summary>
        /// <param name="parameterType">
        /// The parameter type.
        /// </param>
        /// <returns>
        /// The <see cref="OpCode"/>.
        /// </returns>
        private static OpCode GetStindInstruction(Type parameterType)
        {
            string typeName = parameterType.Name;
            if (parameterType.IsClass && !typeName.EndsWith("&"))
            {
                return OpCodes.Stind_Ref;
            }

            if (!StindOpCodesDictionary.Instance.ContainsKey(parameterType) && parameterType.IsByRef)
            {
                return OpCodes.Stind_Ref;
            }

            OpCode result = StindOpCodesDictionary.Instance[parameterType];

            return result;
        }

        /// <summary>
        /// Adds the stack trace.
        /// <remarks>
        /// This is null for performance reasons.</remarks>
        /// </summary>
        /// <param name="il">
        /// The <see cref="ILGenerator"/>.
        /// </param>
        private void PushStackTrace(ILGenerator il)
        {
            // NOTE: The stack trace has been disabled for performance reasons
            il.Emit(OpCodes.Ldnull);
        }

        /// <summary>
        /// Adds any generic arguments to the output.
        /// </summary>
        /// <param name="method">
        /// The method.
        /// </param>
        /// <param name="il">
        /// The <see cref="ILGenerator"/>.
        /// </param>
        private void PushGenericArguments(MethodInfo method, ILGenerator il)
        {
            Type[] typeParameters = method.GetGenericArguments();

            // If this is a generic method, we need to store
            // the generic method arguments
            int genericTypeCount = typeParameters.Length;

            // Type[] genericTypeArgs = new Type[genericTypeCount];
            il.Emit(OpCodes.Ldc_I4, genericTypeCount);
            il.Emit(OpCodes.Newarr, typeof(Type));

            if (genericTypeCount == 0)
            {
                return;
            }

            for (int index = 0; index < genericTypeCount; index++)
            {
                Type currentType = typeParameters[index];

                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldc_I4, index);
                il.Emit(OpCodes.Ldtoken, currentType);
                il.Emit(OpCodes.Call, GetTypeFromHandle);
                il.Emit(OpCodes.Stelem_Ref);
            }
        }

        /// <summary>
        /// Adds the return type to the output.
        /// </summary>
        /// <param name="method">
        /// The method.
        /// </param>
        /// <param name="il">
        /// The <see cref="ILGenerator"/>.
        /// </param>
        private void PackageReturnType(MethodInfo method, ILGenerator il)
        {
            Type returnType = method.ReturnType;

            // Unbox the return value if necessary
            if (returnType == typeof(void))
            {
                il.Emit(OpCodes.Pop);
                return;
            }

            il.Emit(OpCodes.Unbox_Any, returnType);
        }
    }
}
