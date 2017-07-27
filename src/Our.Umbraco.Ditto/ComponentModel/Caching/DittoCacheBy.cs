using System;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The built in properties to cache by
    /// </summary>
    [Flags]
    public enum DittoCacheBy
    {
        /// <summary>
        /// The content identifier
        /// </summary>
        ContentId = 1,

        /// <summary>
        /// The content version
        /// </summary>
        ContentVersion = 2,

        /// <summary>
        /// The property name
        /// </summary>
        PropertyName = 4,

        /// <summary>
        /// The target type
        /// </summary>
        TargetType = 8,

        /// <summary>
        /// The culture
        /// </summary>
        Culture = 16,

        /// <summary>
        /// The attribute type
        /// </summary>
        AttributeType = 32,

        /// <summary>
        /// Use the customisation method on the attribute
        /// </summary>
        Custom = 64
    }
}