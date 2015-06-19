namespace Our.Umbraco.Ditto
{
    using global::Umbraco.Web;

    /// <summary>
    /// The Umbraco dictionary value resolver.
    /// </summary>
    public class UmbracoDictionaryValueResolver : DittoValueResolver<UmbracoDictionaryAttribute>
    {
        /// <summary>
        /// Gets the raw value for the current property from Umbraco.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/> representing the raw value.
        /// </returns>
        public override object ResolveValue()
        {
            var dictionaryKey = Attribute.DictionaryKey ?? (Context.PropertyDescriptor != null ? Context.PropertyDescriptor.Name : string.Empty);

            if (string.IsNullOrWhiteSpace(dictionaryKey))
            {
                return null;
            }

            // HACK: [LK:2015-04-14] Resorting to using `UmbracoHelper`, as `CultureDictionaryFactoryResolver` isn't public in v6.2.x.
            return new UmbracoHelper(UmbracoContext.Current, Content).GetDictionaryValue(dictionaryKey);
        }
    }
}