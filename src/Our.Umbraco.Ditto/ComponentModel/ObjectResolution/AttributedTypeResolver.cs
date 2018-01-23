using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// A resolver class to provide a lookup for a specific Ditto attribue for a given type.
    /// </summary>
    /// <typeparam name="TAttribute">A specific Ditto attribute type.</typeparam>
    internal sealed class AttributedTypeResolver<TAttribute> where TAttribute : Attribute
    {
        private readonly ConcurrentDictionary<Type, TAttribute> _attributedTypeLookup;

        private AttributedTypeResolver()
        {
            _attributedTypeLookup = new ConcurrentDictionary<Type, TAttribute>();
        }

        private void Initialize(IEnumerable<Type> types, bool inherit = false)
        {
            if (types != null)
            {
                foreach (var type in types)
                {
                    TryAddAttributedType(type, out TAttribute attribute, inherit);
                }
            }
        }

        private static AttributedTypeResolver<TAttribute> _resolver;

        /// <summary>
        /// A static instance of the attributed type resolver.
        /// </summary>
        public static AttributedTypeResolver<TAttribute> Current
        {
            get { return _resolver; }
            set { _resolver = value; }
        }

        /// <summary>
        /// Returns true if the resolver instance is currently available.
        /// </summary>
        public static bool HasCurrent
        {
            get { return _resolver != null; }
        }

        /// <summary>
        /// Creates a new instance of the attributed type resolver.
        /// </summary>
        /// <param name="pluginManager">An instance of Umbraco's PluginManager object.</param>
        /// <param name="inherit">A boolean flag to search the type's inheritance chain to find the attribute.</param>
        /// <returns>Returns a new instance of the attributed type resolver.</returns>
        public static AttributedTypeResolver<TAttribute> Create(PluginManager pluginManager, bool inherit = false)
        {
            return Create(pluginManager.ResolveAttributedTypes<TAttribute>(), inherit);
        }

        /// <summary>
        /// Creates a new instance of the attributed type resolver.
        /// </summary>
        /// <param name="types">An enumerable of attributed types.</param>
        /// <param name="inherit">A boolean flag to search the type's inheritance chain to find the attribute.</param>
        /// <returns>Returns a new instance of the attributed type resolver.</returns>
        public static AttributedTypeResolver<TAttribute> Create(IEnumerable<Type> types, bool inherit = false)
        {
            var resolver = new AttributedTypeResolver<TAttribute>();

            resolver.Initialize(types, inherit);

            return resolver;
        }

        /// <summary>
        /// Tries to add the associated attribute for a given type.
        /// </summary>
        /// <param name="type">The object type.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="inherit">A boolean flag to search the type's inheritance chain to find the attribute.</param>
        /// <returns>Returns the result status of adding the attribute to the internal dictionary.</returns>
        private bool TryAddAttributedType(Type type, out TAttribute attribute, bool inherit)
        {
            attribute = type.GetCustomAttribute<TAttribute>(inherit);
            if (attribute != null && _attributedTypeLookup.ContainsKey(type) == false)
            {
                return _attributedTypeLookup.TryAdd(type, attribute);
            }
            else
            {
                LogHelper.Warn<AttributedTypeResolver<TAttribute>>($"Duplicate '{typeof(TAttribute)}' attribute found in type: '{type}'");
            }

            return false;
        }

        /// <summary>
        /// Tries to get the associated attribute for a given object-type.
        /// </summary>
        /// <param name="type">The object-type.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="inherit">A boolean flag to search the type's inheritance chain to find the attribute.</param>
        /// <returns>Returns the associated attribute for the given object-type.</returns>
        public bool TryGetTypeAttribute(Type type, out TAttribute attribute, bool inherit = false)
        {
            bool result = _attributedTypeLookup.TryGetValue(type, out attribute);

            if (result == false)
            {
                result = TryAddAttributedType(type, out attribute, inherit);
            }

            return result;
        }
    }
}