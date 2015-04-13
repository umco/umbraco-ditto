namespace Our.Umbraco.Ditto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents the proxy cache entry to be stored in the cache.
    /// </summary>
    internal struct ProxyCacheEntry
    {
        /// <summary>
        /// The base type.
        /// </summary>
        public readonly Type BaseType;

        /// <summary>
        /// The excluded properties.
        /// </summary>
        public readonly IEnumerable<PropertyInfo> ExcludedProperties;

        /// <summary>
        /// The hash code for comparing <see cref="ProxyCacheEntry"/> instances.
        /// </summary>
        private readonly int hashCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyCacheEntry"/> struct.
        /// </summary>
        /// <param name="baseType">
        /// The base type to proxy.
        /// </param>
        /// <param name="excludedProperties">
        /// The excluded properties.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the given <see cref="Type"/> is null.
        /// </exception>
        public ProxyCacheEntry(Type baseType, IEnumerable<PropertyInfo> excludedProperties)
        {
            if (baseType == null)
            {
                throw new ArgumentNullException("baseType");
            }

            this.BaseType = baseType;
            this.ExcludedProperties = excludedProperties;

            unchecked
            {
                int h = this.BaseType.GetHashCode();

                if (this.ExcludedProperties != null)
                {
                    // Prevent property duplication.
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (var property in this.ExcludedProperties.Distinct())
                    {
                        h = (h * 397) ^ property.GetHashCode();
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
