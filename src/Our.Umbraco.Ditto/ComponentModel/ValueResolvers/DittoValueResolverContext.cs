namespace Our.Umbraco.Ditto
{
    using System;

    /// <summary>
    /// The Ditto resolver context.
    /// Provides a way of providing contextual information to value resolvers.
    /// </summary>
    public class DittoValueResolverContext : PublishedContentContext
    {
        public Type ConversionType { get; set; }
    }
}