namespace Our.Umbraco.Ditto
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    /// <summary>
    /// Provides appropriate Stind.X opcode 
    /// for the type of primitive value to be stored indirectly.
    /// </summary>
    internal sealed class StindOpCodesDictionary : Dictionary<Type, OpCode>
    {
        /// <summary>
        /// The lazily invoked instance.
        /// </summary>
        private static readonly Lazy<StindOpCodesDictionary> Lazy =
                    new Lazy<StindOpCodesDictionary>(() => new StindOpCodesDictionary());

        /// <summary>
        /// Prevents a default instance of the <see cref="StindOpCodesDictionary"/> class from being created. 
        /// </summary>
        private StindOpCodesDictionary()
        {
            this.Add(typeof(bool), OpCodes.Stind_I1);
            this.Add(typeof(sbyte), OpCodes.Stind_I1);
            this.Add(typeof(byte), OpCodes.Stind_I1);

            this.Add(typeof(short), OpCodes.Stind_I2);
            this.Add(typeof(ushort), OpCodes.Stind_I2);

            this.Add(typeof(int), OpCodes.Stind_I4);
            this.Add(typeof(uint), OpCodes.Stind_I4);

            this.Add(typeof(IntPtr), OpCodes.Stind_I4);
            this.Add(typeof(long), OpCodes.Stind_I8);
            this.Add(typeof(ulong), OpCodes.Stind_I8);
            this.Add(typeof(float), OpCodes.Stind_R4);
            this.Add(typeof(double), OpCodes.Stind_R8);
        }

        /// <summary>
        /// Gets the current <see cref="StindOpCodesDictionary"/> instance.
        /// </summary>
        public static StindOpCodesDictionary Instance
        {
            get
            {
                return Lazy.Value;
            }
        }
    }
}
