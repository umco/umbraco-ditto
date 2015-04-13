namespace Our.Umbraco.Ditto
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// Implements the <see cref="IProxy"/> <see cref="IInterceptor"/> property on the dynamic proxy class.
    /// </summary>
    internal class ProxyImplementer
    {
        /// <summary>
        /// The field builder for defining the interceptor field.
        /// </summary>
        private FieldBuilder fieldBuilder;

        /// <summary>
        /// Gets the interceptor field.
        /// </summary>
        public FieldBuilder InterceptorField
        {
            get { return this.fieldBuilder; }
        }

        /// <summary>
        /// Implements the proxy interceptor fields.
        /// </summary>
        /// <param name="typeBuilder">
        /// The type builder.
        /// </param>
        public void ImplementProxy(TypeBuilder typeBuilder)
        {
            // Implement the IProxy interface
            typeBuilder.AddInterfaceImplementation(typeof(IProxy));

            // Define the private "interceptor" filed.
            this.fieldBuilder = typeBuilder.DefineField("interceptor", typeof(IInterceptor), FieldAttributes.Private);

            // Define the correct attributes fot the property. This makes it public virtual.
            const MethodAttributes Attributes = MethodAttributes.Public | MethodAttributes.HideBySig |
                                                MethodAttributes.SpecialName | MethodAttributes.NewSlot |
                                                MethodAttributes.Virtual;

            // Implement the getter
            MethodBuilder getterMethod = typeBuilder.DefineMethod(
                "get_Interceptor",
                Attributes,
                CallingConventions.HasThis,
                typeof(IInterceptor),
                new Type[0]);

            // Set the correct flags to signal the property is managed and implemented in intermediate language.
            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            getterMethod.SetImplementationFlags(MethodImplAttributes.Managed | MethodImplAttributes.IL);

            ILGenerator il = getterMethod.GetILGenerator();

            // This is equivalent to:
            // get { return this.interceptor; }
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, this.fieldBuilder);
            il.Emit(OpCodes.Ret);

            // Implement the setter
            MethodBuilder setterMethod = typeBuilder.DefineMethod(
                "set_Interceptor",
                Attributes,
                CallingConventions.HasThis,
                typeof(void),
                new[] { typeof(IInterceptor) });

            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            setterMethod.SetImplementationFlags(MethodImplAttributes.Managed | MethodImplAttributes.IL);
            il = setterMethod.GetILGenerator();

            // This is equivalent to:
            // set { this.interceptor = value; }
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, this.fieldBuilder);
            il.Emit(OpCodes.Ret);

            // Implement the properties on the IProxy interface
            MethodInfo originalSetter = typeof(IProxy).GetMethod("set_Interceptor");
            MethodInfo originalGetter = typeof(IProxy).GetMethod("get_Interceptor");

            typeBuilder.DefineMethodOverride(setterMethod, originalSetter);
            typeBuilder.DefineMethodOverride(getterMethod, originalGetter);
        }
    }
}
