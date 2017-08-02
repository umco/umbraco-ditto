namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Enum to describe when properties should be lazy loaded during a ditto conversion
    /// </summary>
    public enum LazyLoad
    {
        /// <summary>
        /// Make all virtual properties lazy
        /// </summary>
        AllVirtuals,

        /// <summary>
        /// Only make attributed virtual properties lazy
        /// </summary>
        AttributedVirtuals
    }
}