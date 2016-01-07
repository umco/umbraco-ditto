using System;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum DittoProcessorCacheBy
    {
        /// <summary>
        /// The content identifier
        /// </summary>
        /// 
        ContentId = 1,

        /// <summary>
        /// The property name
        /// </summary>
        PropertyName = 2,

        /// <summary>
        /// The target type
        /// </summary>
        TargetType = 4,

        /// <summary>
        /// The culture
        /// </summary>
        Culture = 8
    }
}