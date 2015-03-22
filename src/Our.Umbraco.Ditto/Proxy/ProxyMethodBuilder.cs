namespace Our.Umbraco.Ditto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// The proxy method builder.
    /// </summary>
    internal class ProxyMethodBuilder
    {
        /// <summary>
        /// The method emitter.
        /// </summary>
        private MethodEmitter emitter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyMethodBuilder"/> class.
        /// </summary>
        public ProxyMethodBuilder()
        {
            this.emitter = new MethodEmitter();
        }

        /// <summary>
        /// Gets or sets the method body emitter.
        /// </summary>
        public MethodEmitter MethodBodyEmitter
        {
            get { return this.emitter; }
            set { this.emitter = value; }
        }

        /// <summary>
        /// Created the proxy method.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <param name="method">
        /// The method.
        /// </param>
        /// <param name="typeBuilder">
        /// The type builder.
        /// </param>
        public void CreateProxiedMethod(FieldInfo field, MethodInfo method, TypeBuilder typeBuilder)
        {
            ParameterInfo[] parameters = method.GetParameters();

            const MethodAttributes MethodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig |
                                                      MethodAttributes.Virtual;

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                method.Name,
                MethodAttributes,
                CallingConventions.HasThis,
                method.ReturnType,
                parameters.Select(param => param.ParameterType).ToArray());

            Type[] typeArgs = method.GetGenericArguments();

            if (typeArgs.Length > 0)
            {
                List<string> typeNames = new List<string>();

                for (int index = 0; index < typeArgs.Length; index++)
                {
                    typeNames.Add(string.Format("T{0}", index));
                }

                methodBuilder.DefineGenericParameters(typeNames.ToArray());
            }

            ILGenerator il = methodBuilder.GetILGenerator();

            this.emitter.EmitMethodBody(il, method, field);
        }
    }
}
