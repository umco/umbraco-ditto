namespace Our.Umbraco.Ditto
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// The proxy implementer.
    /// </summary>
    internal class ProxyImplementer
    {
        /// <summary>
        /// The field builder.
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

            this.fieldBuilder = typeBuilder.DefineField("__interceptor", typeof(IInterceptor), FieldAttributes.Private);

            // Implement the getter
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

            getterMethod.SetImplementationFlags(MethodImplAttributes.Managed | MethodImplAttributes.IL);

            ILGenerator il = getterMethod.GetILGenerator();

            // This is equivalent to:
            // get { return __interceptor;
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

            setterMethod.SetImplementationFlags(MethodImplAttributes.Managed | MethodImplAttributes.IL);
            il = setterMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, this.fieldBuilder);
            il.Emit(OpCodes.Ret);

            MethodInfo originalSetter = typeof(IProxy).GetMethod("set_Interceptor");
            MethodInfo originalGetter = typeof(IProxy).GetMethod("get_Interceptor");

            typeBuilder.DefineMethodOverride(setterMethod, originalSetter);
            typeBuilder.DefineMethodOverride(getterMethod, originalGetter);
        }
    }
}
