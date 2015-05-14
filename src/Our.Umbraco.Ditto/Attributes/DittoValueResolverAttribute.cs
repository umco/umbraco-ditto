namespace Our.Umbraco.Ditto
{
    using System;

    /// <summary>
    /// The Ditto value attribute, defines how a property can get its value.
    /// All other Ditto value attributes should inherit from this class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class DittoValueResolverAttribute : Attribute
    {
        private readonly Type _resolverType;

        public Type ResolverType
        {
            get
            {
                return _resolverType;
            }
        }

        protected DittoValueResolverAttribute(Type resolverType)
        {
            if (!typeof(DittoValueResolver).IsAssignableFrom(resolverType))
                throw new ArgumentException("Resolver type must inherit from DittoValueResolver", "resolverType");

            _resolverType = resolverType;
        }

        public override bool Equals(object obj)
        {
            var other = obj as DittoValueResolverAttribute;
            return (other != null)
                && other.ResolverType.AssemblyQualifiedName == _resolverType.AssemblyQualifiedName;
        }

        public override int GetHashCode()
        {
            return _resolverType.AssemblyQualifiedName.GetHashCode();
        }
    }
}