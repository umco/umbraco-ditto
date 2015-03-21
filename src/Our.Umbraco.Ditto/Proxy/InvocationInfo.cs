namespace Our.Umbraco.Ditto
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Contains information about the current method invocation.
    /// </summary>
    internal sealed class InvocationInfo
    {
        /// <summary>
        /// The arguments.
        /// </summary>
        private readonly object[] args;

        /// <summary>
        /// The current proxy.
        /// </summary>
        private readonly object proxy;

        /// <summary>
        /// The target method.
        /// </summary>
        private readonly MethodInfo targetMethod;

        /// <summary>
        /// The stack trace.
        /// </summary>
        private readonly StackTrace trace;

        /// <summary>
        /// The type arguments.
        /// </summary>
        private readonly Type[] typeArgs;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvocationInfo"/> class.
        /// </summary>
        /// <param name="proxy">The proxy object.
        /// </param>
        /// <param name="targetMethod">The target method.</param>
        /// <param name="trace">The <see cref="StackTrace"/> containing the stack frames.</param>
        /// <param name="genericTypeArgs">
        /// The generic type arguments.</param>
        /// <param name="args">The arguments passed to the target.</param>
        public InvocationInfo(
            object proxy,
            MethodInfo targetMethod,
            StackTrace trace,
            Type[] genericTypeArgs,
            object[] args)
        {
            this.proxy = proxy;
            this.targetMethod = targetMethod;
            this.typeArgs = genericTypeArgs;
            this.args = args;
            this.trace = trace;
        }

        /// <summary>
        /// Gets the target.
        /// </summary>
        public object Target
        {
            get { return this.proxy; }
        }

        /// <summary>
        /// Gets the target method.
        /// </summary>
        public MethodInfo TargetMethod
        {
            get { return this.targetMethod; }
        }

        /// <summary>
        /// Gets the stack trace.
        /// </summary>
        public StackTrace StackTrace
        {
            get { return this.trace; }
        }

        // Stack trace is disabled for performance reasons.
        /// <summary>
        /// Gets the calling method.
        /// </summary>
        // public MethodInfo CallingMethod
        // {
        //    get { return (MethodInfo)this.trace.GetFrame(0).GetMethod(); }
        // }

        /// <summary>
        /// Gets the generic type arguments.
        /// </summary>
        public Type[] TypeArguments
        {
            get { return this.typeArgs; }
        }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        public object[] Arguments
        {
            get { return this.args; }
        }

        /// <summary>
        /// Sets a given method argument.
        /// </summary>
        /// <param name="position">
        /// The position of the argument.
        /// </param>
        /// <param name="value">
        /// The value to set.
        /// </param>
        public void SetArgument(int position, object value)
        {
            this.args[position] = value;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            // Stacktrace is disabled for performance reasons.
            // builder.AppendFormat("Calling Method: {0}\n", this.GetMethodName(this.CallingMethod));
            builder.AppendFormat("Target Method:{0}\n", this.GetMethodName(this.targetMethod));
            builder.AppendLine("Arguments:");

            foreach (ParameterInfo info in this.targetMethod.GetParameters())
            {
                object currentArgument = this.args[info.Position] ?? "(null)";
                builder.AppendFormat("\t{0}: {1}\n", info.Name, currentArgument);
            }

            builder.AppendLine();

            return builder.ToString();
        }

        /// <summary>
        /// Gets the specific method name from the <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private string GetMethodName(MethodInfo method)
        {
            StringBuilder builder = new StringBuilder();
            if (method.DeclaringType != null)
            {
                builder.AppendFormat("{0}.{1}", method.DeclaringType.Name, method.Name);
            }

            builder.Append("(");

            ParameterInfo[] parameters = method.GetParameters();
            int parameterCount = parameters.Length;

            int index = 0;
            foreach (ParameterInfo param in parameters)
            {
                index++;
                builder.AppendFormat("{0} {1}", param.ParameterType.Name, param.Name);

                if (index < parameterCount)
                {
                    builder.Append(", ");
                }
            }
            builder.Append(")");

            return builder.ToString();
        }
    }
}
