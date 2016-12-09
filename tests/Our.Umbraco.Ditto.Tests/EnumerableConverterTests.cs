using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using NUnit.Framework;

namespace Our.Umbraco.Ditto.Tests
{
    using System.Linq;

    [TestFixture]
    public class EnumerableConverterTests
    {
        /*
         * An issue has been flagged for when attempting to map a property type that inherits from a generic enumerable.
         * Background on the issue can be found here:
         * <https://our.umbraco.org/projects/developer-tools/ditto/ditto-feedback/79244-error-on-using-relatedlink-with-ditto-and-jeavon-property-converter#comment-257744>
         * 
         * The culprit is here: EnumerableConverterAttribute.cs, line 76
         * As the ojbect-type is detected as an enumerable type (line 40), but when the `GenericTypeArguments` are accessed (line 76),
         * the top-level type is accessed, not the base type.
         * 
         * These unit-tests attempt to test the processor directly, (as opposed to going via the `.As<T>` method.
         * The first test is successful, as it is working with a generic enumerable type directly.
         * The second test fails, as it is working with a class inherited from a generic enumerable type.
         */

        public class MyModel
        { }

        public class InheritedEnumerableModel : IEnumerable<MyModel>
        {
            IEnumerable<MyModel> _items;

            public InheritedEnumerableModel(IEnumerable<MyModel> items)
            {
                _items = items;
            }

            public IEnumerator<MyModel> GetEnumerator()
            {
                return _items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        public class WrappedModel
        {
            public IEnumerable<MyModel> MyProperty1 { get; set; }

            public InheritedEnumerableModel MyProperty2 { get; set; }

            public List<MyModel> MyProperty3 { get; set; }
            public Dictionary<MyModel, string> MyProperty4 { get; set; }
        }

        [Test]
        public void EnumerableConverter_EnumerableModel_Test()
        {
            var context = new DittoProcessorContext
            {
                PropertyDescriptor = TypeDescriptor.GetProperties(new WrappedModel())["MyProperty1"]
            };

            var processor = new EnumerableConverterAttribute();
            var result = processor.ProcessValue(null, context);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void EnumerableConverter_InheritedEnumerableModel_Test()
        {
            var context = new DittoProcessorContext
            {
                PropertyDescriptor = TypeDescriptor.GetProperties(new WrappedModel())["MyProperty2"]
            };

            var processor = new EnumerableConverterAttribute();
            var result = processor.ProcessValue(null, context);

            // The value should be null since we don't have an empty constructor. 
            Assert.That(result, Is.Null);
        }

        [Test]
        public void EnumerableConverter_EnumerableModelEmptyList_Test()
        {
            var context = new DittoProcessorContext
            {
                PropertyDescriptor = TypeDescriptor.GetProperties(new WrappedModel())["MyProperty3"]
            };

            var processor = new EnumerableConverterAttribute();
            var result = processor.ProcessValue(null, context);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void EnumerableConverter_EnumerableModelEmptyDictionary_Test()
        {
            var context = new DittoProcessorContext
            {
                PropertyDescriptor = TypeDescriptor.GetProperties(new WrappedModel())["MyProperty4"]
            };

            var processor = new EnumerableConverterAttribute();
            var result = processor.ProcessValue(null, context);

            Assert.That(result, Is.Not.Null);
        }
    }
}