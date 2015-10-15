namespace Our.Umbraco.Ditto
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Specifies what type to use as a converter for the object this attribute is bound to. 
    /// This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public sealed class DittoTypeConverterAttribute : Attribute
    {
        /// <summary>
        /// Specifies the type to use as a converter for the object this attribute is bound to. 
        /// This <see langword="static"/>field is read-only.
        /// </summary>
        public static readonly DittoTypeConverterAttribute Default = new DittoTypeConverterAttribute();

        /// <summary>
        /// The name of the type to use as a converter for the object this attribute is bound to.
        /// </summary>
        private readonly string typeName;

        /// <summary>
        /// Initializes a new instance of the <see cref="DittoTypeConverterAttribute"/> class.
        /// </summary>
        public DittoTypeConverterAttribute()
        {
            this.typeName = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DittoTypeConverterAttribute"/> class.
        /// </summary>
        /// <param name="type">
        /// The <see cref="System.Type"/> to use as a converter for the object this attribute is bound to.
        /// </param>
        public DittoTypeConverterAttribute(Type type)
        {
            this.typeName = type.AssemblyQualifiedName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DittoTypeConverterAttribute"/> class.
        /// </summary>
        /// <param name="typeName">
        /// The name of the <see cref="System.Type"/> to use as a converter for the object this attribute is bound to.
        /// </param>
        public DittoTypeConverterAttribute(string typeName)
        {
            string temp = typeName.ToUpper(CultureInfo.InvariantCulture);
            Debug.Assert(temp.IndexOf(".DLL", StringComparison.Ordinal) == -1, "Came across: " + typeName + " . Please remove the .dll extension");
            this.typeName = typeName;
        }

        /// <summary>
        /// Gets the fully qualified type name of the <see cref="System.Type"/>
        /// to use as a converter for the object this attribute is bound to.
        /// </summary>
        public string ConverterTypeName
        {
            get
            {
                return this.typeName;
            }
        }

        /// <summary>
        /// Returns a value that indicates whether this instance is equal to a specified object.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> equals the type and value of this instance; otherwise, false.
        /// </returns>
        /// <param name="obj">An <see cref="T:System.Object"/> to compare with this instance or null. </param>
        public override bool Equals(object obj)
        {
            DittoTypeConverterAttribute other = obj as DittoTypeConverterAttribute;
            return (other != null) && other.ConverterTypeName == this.typeName;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode()
        {
            return this.typeName.GetHashCode();
        }
    }
}
