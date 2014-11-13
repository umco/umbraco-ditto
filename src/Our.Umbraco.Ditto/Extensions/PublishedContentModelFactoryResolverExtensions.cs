// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublishedContentModelFactoryResolverExtensions.cs" company="Umbrella Inc, Our Umbraco and other contributors">
//   Copyright Umbrella Inc, Our Umbraco and other contributors
// </copyright>
// <summary>
//   Encapsulates extension methods for <see cref="PublishedContentModelFactoryResolver" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Our.Umbraco.Ditto.Extensions
{
    using System;
    using System.Collections.Generic;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Models.PublishedContent;

    using Our.Umbraco.Ditto.ModelFactory;

    /// <summary>
    /// Encapsulates extension methods for <see cref="PublishedContentModelFactoryResolver"/>.
    /// </summary>
    public static class PublishedContentModelFactoryResolverExtensions
    {
        /// <summary>
        /// Sets the factory resolver to resolve the given types using the <see cref="DittoPublishedContentModelFactory"/>.
        /// </summary>
        /// <param name="resolver">
        /// The <see cref="PublishedContentModelFactoryResolver"/> this method extends.
        /// </param>
        /// <typeparam name="T">
        /// The base <see cref="Type"/> to retrieve classes that inherit from.
        /// </typeparam>
        public static void SetFactory<T>(this PublishedContentModelFactoryResolver resolver)
            where T : IPublishedContent
        {
            IEnumerable<Type> types = PluginManager.Current.ResolveTypes<T>();
            DittoPublishedContentModelFactory factory = new DittoPublishedContentModelFactory(types);

            resolver.SetFactory(factory);
        }
    }
}