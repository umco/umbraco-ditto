namespace Our.Umbraco.Ditto
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// Provides methods for emitting constructors in proxy classes.
    /// </summary>
    internal static class ConstructorEmitter
    {
        /// <summary>
        /// Uses reflection to emit the given <see cref="ConstructorInfo"/> body for the given type.
        /// </summary>
        /// <param name="typeBuilder">
        /// The <see cref="TypeBuilder"/>.
        /// </param>
        /// <param name="constructorInfo">
        /// The <see cref="ConstructorInfo"/>.
        /// </param>
        public static void Emit(TypeBuilder typeBuilder, ConstructorInfo constructorInfo)
        {
            // Define the default constructor attributes.
            const MethodAttributes ConstructorAttributes = MethodAttributes.Public |
                                                           MethodAttributes.HideBySig |
                                                           MethodAttributes.SpecialName |
                                                           MethodAttributes.RTSpecialName;

            // Get the parameters.
            Type[] parameterTypes = constructorInfo.GetParameters().Select(p => p.ParameterType).ToArray();

            // Define the constructor.
            ConstructorBuilder constructor = typeBuilder.DefineConstructor(
                                                            ConstructorAttributes,
                                                            CallingConventions.Standard,
                                                            parameterTypes);

            ILGenerator il = constructor.GetILGenerator();

            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            constructor.SetImplementationFlags(MethodImplAttributes.IL | MethodImplAttributes.Managed);

            // Load all constructor arguments. Note argument 0 is 'this' pointer, so you must emit one more.
            for (int i = 0; i <= parameterTypes.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_S, i);
            }

            // Call the base constructor and return.
            il.Emit(OpCodes.Call, constructorInfo);
            il.Emit(OpCodes.Ret);
        }
    }
}
