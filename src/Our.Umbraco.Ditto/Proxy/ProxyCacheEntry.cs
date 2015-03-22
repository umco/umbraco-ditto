namespace Our.Umbraco.Ditto
{
    using System;
    using System.Linq;

    /// <summary>
    /// A single proxy cache entry.
    /// </summary>
    internal struct ProxyCacheEntry
    {
        /// <summary>
        /// The base type.
        /// </summary>
        public readonly Type BaseType;

        /// <summary>
        /// The interfaces.
        /// </summary>
        public readonly Type[] Interfaces;

        /// <summary>
        /// The hash code.
        /// </summary>
        private readonly int hashCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyCacheEntry"/> struct.
        /// </summary>
        /// <param name="baseType">
        /// The base type.
        /// </param>
        /// <param name="interfaces">
        /// The interfaces.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the base type is null.
        /// </exception>
        public ProxyCacheEntry(Type baseType, Type[] interfaces)
        {
            if (baseType == null)
            {
                throw new ArgumentNullException("baseType");
            }

            this.BaseType = baseType;
            this.Interfaces = interfaces;

            unchecked
            {
                int h = this.BaseType.GetHashCode();

                if (this.Interfaces != null)
                {
                    // Prevent type duplication.
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (var type in this.Interfaces.Distinct())
                    {
                        h = (h * 397) ^ type.GetHashCode();
                    }
                }

                this.hashCode = h;
            }
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; 
        /// otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to.</param>
        public override bool Equals(object obj)
        {
            if (!(obj is ProxyCacheEntry))
            {
                return false;
            }

            ProxyCacheEntry other = (ProxyCacheEntry)obj;
            return this.hashCode == other.hashCode;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return this.hashCode;
        }
    }
}
