namespace Our.Umbraco.Ditto
{
    using System;

    /// <summary>
    /// A single method base cache item for identifying methods.
    /// </summary>
    internal struct MethodBaseCacheItem
    {
        /// <summary>
        /// Gets or sets the method base.
        /// </summary>
        public readonly string MethodBase;

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public readonly Type Type;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodBaseCacheItem"/> struct.
        /// </summary>
        /// <param name="methodBase">The method base.</param>
        /// <param name="type">The type.</param>
        public MethodBaseCacheItem(string methodBase, Type type)
        {
            this.MethodBase = methodBase;
            this.Type = type;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the 
        /// same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param>
        public override bool Equals(object obj)
        {
            if (obj is MethodBaseCacheItem)
            {
                MethodBaseCacheItem other = (MethodBaseCacheItem)obj;

                return this.MethodBase == other.MethodBase && this.Type == other.Type;
            }

            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.MethodBase.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Type.GetHashCode();
                return hashCode;
            }
        }
    }
}
