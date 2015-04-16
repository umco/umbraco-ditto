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
            return (other != null) && other.ResolverType.AssemblyQualifiedName == _resolverType.AssemblyQualifiedName;
        }

        public override int GetHashCode()
        {
            return _resolverType.AssemblyQualifiedName.GetHashCode();
        }

        ///// <summary>
        ///// Returns the value for the given type and property.
        ///// </summary>
        ///// <param name="content">The <see cref="IPublishedContent"/> to convert.</param>
        ///// <param name="type">The <see cref="Type"/> of items to return.</param>
        ///// <param name="culture">The <see cref="CultureInfo"/></param>
        ///// <param name="propertyInfo">The <see cref="PropertyInfo"/> property info associated with the type.</param>
        ///// <returns>The <see cref="object"/> representing the value.</returns>
        //public abstract object GetValue(
        //    IPublishedContent content,
        //    Type type,
        //    CultureInfo culture,
        //    PropertyInfo propertyInfo);
    }
}