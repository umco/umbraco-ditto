namespace Our.Umbraco.Ditto
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    /// <summary>
    /// The map of storage codes.
    /// </summary>
    public class StindMap : Dictionary<string, OpCode>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StindMap"/> class.
        /// </summary>
        public StindMap()
        {
            this["Bool&"] = OpCodes.Stind_I1;
            this["Int8&"] = OpCodes.Stind_I1;
            this["Uint8&"] = OpCodes.Stind_I1;

            this["Int16&"] = OpCodes.Stind_I2;
            this["Uint16&"] = OpCodes.Stind_I2;

            this["Uint32&"] = OpCodes.Stind_I4;
            this["Int32&"] = OpCodes.Stind_I4;

            this["IntPtr"] = OpCodes.Stind_I4;
            this["Uint64&"] = OpCodes.Stind_I8;
            this["Int64&"] = OpCodes.Stind_I8;
            this["Float32&"] = OpCodes.Stind_R4;
            this["Float64&"] = OpCodes.Stind_R8;
        }
    }
}
