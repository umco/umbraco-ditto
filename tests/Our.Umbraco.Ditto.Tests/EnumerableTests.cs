namespace Our.Umbraco.Ditto.Tests
{
    using NUnit.Framework;

    /// <summary>
    /// The enumerable tests.
    /// </summary>
    [TestFixture]
    [Category("Collections")]
    public class EnumerableTests
    {
        /// <summary>
        /// Tests the <see cref="EnumerableExtensions.YieldSingleItem{T}"/> method.
        /// </summary>
        /// <param name="input">
        /// The input <see cref="Type"/>.
        /// </param>
        /// <param name="expected">
        /// The expected result.
        /// </param>
        [Test]
        [TestCase("test", true)]
        [TestCase(true, true)]
        [TestCase(1, true)]
        public void TestEnumerableYieldSingleItem(object input, bool expected)
        {
            Assert.AreEqual(input.YieldSingleItem().GetType().IsEnumerableType(), expected);
        }
    }
}