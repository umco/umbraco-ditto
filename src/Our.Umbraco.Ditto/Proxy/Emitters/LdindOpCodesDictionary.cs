namespace Our.Umbraco.Ditto
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    /// <summary>
    /// Provides appropriate Ldind.X opcode 
    /// for the type of primitive value to be stored indirectly.
    /// </summary>
    internal sealed class LdindOpCodesDictionary : Dictionary<Type, OpCode>
    {
        /// <summary>
        /// The lazily invoked instance.
        /// </summary>
        private static readonly Lazy<LdindOpCodesDictionary> Lazy =
                    new Lazy<LdindOpCodesDictionary>(() => new LdindOpCodesDictionary());

        /// <summary>
        /// Prevents a default instance of the <see cref="LdindOpCodesDictionary"/> class from being created. 
        /// </summary>
        private LdindOpCodesDictionary()
        {
            this.Add(typeof(bool), OpCodes.Ldind_I1);
            this.Add(typeof(sbyte), OpCodes.Ldind_I1);
            this.Add(typeof(byte), OpCodes.Ldind_I1);

            this.Add(typeof(short), OpCodes.Ldind_I2);
            this.Add(typeof(ushort), OpCodes.Ldind_I2);

            this.Add(typeof(int), OpCodes.Ldind_I4);
            this.Add(typeof(uint), OpCodes.Ldind_I4);

            this.Add(typeof(IntPtr), OpCodes.Ldind_I4);
            this.Add(typeof(long), OpCodes.Ldind_I8);
            this.Add(typeof(ulong), OpCodes.Ldind_I8);
            this.Add(typeof(float), OpCodes.Ldind_R4);
            this.Add(typeof(double), OpCodes.Ldind_R8);
        }

        /// <summary>
        /// Gets the current <see cref="LdindOpCodesDictionary"/> instance.
        /// </summary>
        public static LdindOpCodesDictionary Instance
        {
            get
            {
                return Lazy.Value;
            }
        }
    }
}
