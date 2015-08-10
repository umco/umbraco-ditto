namespace Our.Umbraco.Ditto
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The Ditto value attribute, defines how a property can get its value.
    /// All other Ditto value attributes should inherit from this class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DittoValueResolverAttribute : Attribute
    {
        /// <summary>
        /// The _resolver type
        /// </summary>
        private readonly Type resolverType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DittoValueResolverAttribute"/> class.
        /// </summary>
        /// <param name="resolverType">Type of the resolver.</param>
        /// <exception cref="System.ArgumentException">Resolver type must inherit from DittoValueResolver;resolverType</exception>
        public DittoValueResolverAttribute(Type resolverType)
        {
            if (!typeof(DittoValueResolver).IsAssignableFrom(resolverType))
            {
                throw new ArgumentException("Resolver type must inherit from DittoValueResolver", "resolverType");
            }

            this.resolverType = resolverType;
        }

        /// <summary>
        /// Gets the type of the resolver.
        /// </summary>
        /// <value>
        /// The type of the resolver.
        /// </value>
        public Type ResolverType
        {
            get
            {
                return this.resolverType;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as DittoValueResolverAttribute;
            return (other != null)
                && other.ResolverType.AssemblyQualifiedName == this.resolverType.AssemblyQualifiedName;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.resolverType.AssemblyQualifiedName.GetHashCode();
        }
    }
}