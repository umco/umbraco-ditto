using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.Ditto.Tests
{
    /// <summary>
    /// The type inference tests.
    /// </summary>
    [TestFixture]
    [Category("Type Casting")]
    public class TypeInferenceTests
    {
        /// <summary>
        /// Tests the <see cref="TypeInferenceExtensions.IsCollectionType"/> method.
        /// </summary>
        /// <param name="input">
        /// The input <see cref="Type"/>.
        /// </param>
        /// <param name="expected">
        /// The expected result.
        /// </param>
        [Test]
        [TestCase(typeof(string), false)]
        [TestCase(typeof(bool), false)]
        [TestCase(typeof(int), false)]
        [TestCase(typeof(long), false)]
        [TestCase(typeof(List<string>), true)]
        [TestCase(typeof(Collection<string>), true)]
        [TestCase(typeof(HashSet<string>), true)]
        [TestCase(typeof(Enumerable), false)]
        [TestCase(typeof(Dictionary<string, string>), true)]
        [TestCase(typeof(EnumerableConverterTests.InheritedEnumerableModel), false)]
        public void TestIsCollectionType(Type input, bool expected)
        {
            Assert.AreEqual(input.IsCollectionType(), expected);
        }

        /// <summary>
        /// Tests the <see cref="TypeInferenceExtensions.IsEnumerableType"/> method.
        /// </summary>
        /// <param name="input">The input <see cref="Type"/>.</param>
        /// <param name="expected">The expected result.
        /// </param>
        [Test]
        [TestCase(typeof(string), true)]
        [TestCase(typeof(bool), false)]
        [TestCase(typeof(int), false)]
        [TestCase(typeof(long), false)]
        [TestCase(typeof(List<string>), true)]
        [TestCase(typeof(Collection<string>), true)]
        [TestCase(typeof(HashSet<string>), true)]
        [TestCase(typeof(Enumerable), false)]
        [TestCase(typeof(Dictionary<string, string>), true)]
        [TestCase(typeof(EnumerableConverterTests.InheritedEnumerableModel), true)]
        public void TestIsEnumerableType(Type input, bool expected)
        {
            Assert.AreEqual(input.IsEnumerableType(), expected);
        }

        /// <summary>
        /// Tests the <see cref="TypeInferenceExtensions.IsEnumerableType"/> method.
        /// </summary>
        /// <param name="input">The input <see cref="Type"/>.</param>
        /// <param name="expected">The expected result.
        /// </param>
        [Test]
        [TestCase(typeof(string), false)]
        [TestCase(typeof(bool), false)]
        [TestCase(typeof(int), false)]
        [TestCase(typeof(long), false)]
        [TestCase(typeof(List<string>), true)]
        [TestCase(typeof(Collection<string>), true)]
        [TestCase(typeof(HashSet<string>), true)]
        [TestCase(typeof(Enumerable), false)]
        [TestCase(typeof(Dictionary<string, string>), false)]
        [TestCase(typeof(EnumerableConverterTests.InheritedEnumerableModel), false)]
        public void TestIsCastableEnumerableType(Type input, bool expected)
        {
            Assert.AreEqual(input.IsCastableEnumerableType(), expected);
        }

        /// <summary>
        /// Tests the <see cref="TypeInferenceExtensions.IsEnumerableType"/> method.
        /// </summary>
        /// <param name="input">The input <see cref="Type"/>.</param>
        /// <param name="expected">The expected result.
        /// </param>
        [Test]
        [TestCase(typeof(string), false)]
        [TestCase(typeof(bool), false)]
        [TestCase(typeof(int), false)]
        [TestCase(typeof(long), false)]
        [TestCase(typeof(List<string>), false)]
        [TestCase(typeof(Collection<string>), false)]
        [TestCase(typeof(HashSet<string>), false)]
        [TestCase(typeof(Enumerable), false)]
        [TestCase(typeof(Dictionary<string, string>), true)]
        [TestCase(typeof(List<KeyValuePair<string, string>>), true)]
        [TestCase(typeof(Collection<KeyValuePair<string, string>>), true)]
        [TestCase(typeof(HashSet<KeyValuePair<string, string>>), true)]
        [TestCase(typeof(EnumerableConverterTests.InheritedEnumerableModel), false)]
        public void TestIsEnumerableOfKeyValueType(Type input, bool expected)
        {
            Assert.AreEqual(input.IsEnumerableOfKeyValueType(), expected);
        }

        /// <summary>
        /// Tests the <see cref="TypeInferenceExtensions.IsEnumerableOfType"/> method.
        /// <remarks>
        /// Resharper doesn't work properly if the first parameter is an array.
        /// <see href="http://stackoverflow.com/a/17949732/427899"/>
        /// </remarks>
        /// </summary><param name="input">The input <see cref="Type"/>.
        /// </param>
        /// <param name="argumentType">The argument Type.</param>
        /// <param name="expected">The expected result.</param>
        [Test]
        [TestCase(typeof(IEnumerable<string>), typeof(string), true)]
        [TestCase(typeof(string[]), typeof(string), true, TestName = "TestIsEnumerableOfType: arrayTest Hack")]
        [TestCase(typeof(IEnumerable<int>), typeof(int), true)]
        [TestCase(typeof(int[]), typeof(int), true, TestName = "TestIsEnumerableOfType: arrayTest Hack 2")]
        [TestCase(typeof(string), typeof(string), false)]
        [TestCase(typeof(string), typeof(char), true)]
        [TestCase(typeof(Dictionary<string, string>), typeof(KeyValuePair<string, string>), true)]
        [TestCase(typeof(IEnumerable<Mocks.MockPublishedContent>), typeof(IPublishedContent), true, TestName = "TestIsEnumerableOfType: Derived IPublishedContent")]
        [TestCase(typeof(EnumerableConverterTests.InheritedEnumerableModel), typeof(EnumerableConverterTests.MyModel), true)]
        public void TestIsEnumerableOfType(Type input, Type argumentType, bool expected)
        {
            Assert.AreEqual(input.IsEnumerableOfType(argumentType), expected);
        }
    }
}