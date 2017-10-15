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

        private void Initialize(IEnumerable<Type> types)
        {
            if (types != null)
            {
                foreach (var type in types)
                {
                    var attribute = type.GetCustomAttribute<TAttribute>(false);
                    if (attribute != null && _attributedTypeLookup.ContainsKey(type) == false)
                    {
                        _attributedTypeLookup.TryAdd(type, attribute);
                    }
                    else
                    {
                        LogHelper.Warn<AttributedTypeResolver<TAttribute>>($"Duplicate '{nameof(TAttribute)}' attribute found in type: '{type}'");
                    }
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
        /// <returns>Returns a new instance of the attributed type resolver.</returns>
        public static AttributedTypeResolver<TAttribute> Create(PluginManager pluginManager)
        {
            return Create(pluginManager.ResolveAttributedTypes<TAttribute>());
        }

        /// <summary>
        /// Creates a new instance of the attributed type resolver.
        /// </summary>
        /// <param name="types">An enumerable of attributed types.</param>
        /// <returns>Returns a new instance of the attributed type resolver.</returns>
        public static AttributedTypeResolver<TAttribute> Create(IEnumerable<Type> types)
        {
            var resolver = new AttributedTypeResolver<TAttribute>();

            resolver.Initialize(types);

            return resolver;
        }

        /// <summary>
        /// Tries to get the associated attribute for a given type.
        /// </summary>
        /// <param name="type">The object type.</param>
        /// <param name="attribute">The attribute.</param>
        /// <returns>Returns the associated attribute for the given type.</returns>
        public bool TryGetAttribute(Type type, out TAttribute attribute)
        {
            return _attributedTypeLookup.TryGetValue(type, out attribute);
        }

        /// <summary>
        /// Gets the associated attribute for a given type.
        /// </summary>
        /// <param name="type">The object type.</param>
        /// <returns>Returns the associated attribute for the given type.</returns>
        public TAttribute GetAttribute(Type type)
        {
            return _attributedTypeLookup.TryGetValue(type, out TAttribute attribute)
                ? attribute
                : null;
        }
    }
}