namespace Our.Umbraco.Ditto
{
    using System.Collections.Generic;

    /// <summary>
    /// Extensions methods for <see cref="T:System.Collections.Generic.IEnumerable{T}"/>.
    /// </summary>
    internal static class EnumerableExtensions
    {
        /// <summary>
        /// Wraps this object instance into an <see cref="IEnumerable{T}"/>
        /// consisting of a single item.
        /// </summary>
        /// <typeparam name="T"> Type of the object. </typeparam>
        /// <param name="item"> The instance that will be wrapped. </param>
        /// <returns> An <see cref="IEnumerable{T}"/> consisting of a single item. </returns>
        public static IEnumerable<T> YieldSingleItem<T>(this T item)
        {
            yield return item;
        }

        /// <summary>
        /// Wraps this objects items into a new <see cref="IEnumerable{T}"/>
        /// thus preventing modification of the original collection.
        /// </summary>
        /// <typeparam name="T"> Type of the object. </typeparam>
        /// <param name="source"> The instance that will be wrapped. </param>
        /// <returns> An <see cref="IEnumerable{T}"/>. </returns>
        public static IEnumerable<T> YieldItems<T>(this IEnumerable<T> source)
        {
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
            }
        }
    }
}