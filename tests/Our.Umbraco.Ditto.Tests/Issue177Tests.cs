using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;

namespace Our.Umbraco.Ditto.Tests
{
    /// <remarks>
    /// Unit-test to verify an issue - as outlined in #177
    /// https://github.com/umco/umbraco-ditto/issues/177
    /// When a property is marked as `virtual`, it becomes lazy-loaded,
    /// and not accessible by other properties on the model.
    /// </remarks>
    [TestFixture]
    public class Issue177Tests
    {
        interface IInterface1
        {
            int IdInt { get; set; }
        }

        interface IInterface2
        {
            string IdString { get; }
        }

        public abstract class Base : IInterface1, IInterface2
        {
            public virtual string IdString
            {
                get
                {
                    return IdInt.ToString();
                }
            }

            public virtual int IdInt
            {
                get;
                set;
            }
        }

        public class NewClass : Base
        {
            public override string IdString
            {
                get
                {
                    return IdInt.ToString();
                }
            }

            [UmbracoProperty("ItemLink")]
            public override int IdInt
            {
                get;
                set;
            }
        }

        [Test]
        public void Issue177_Test()
        {
            var nodeId = 1613;

            var content = new MockPublishedContent
            {
                Properties = new[]
                {
                    new MockPublishedContentProperty("ItemLink", nodeId)
                }
            };

            var model = content.As<NewClass>();

            Assert.That(model, Is.Not.Null);
            Assert.That(model.IdInt, Is.EqualTo(nodeId));

            Assert.That(model.IdString, Is.EqualTo(nodeId.ToString()));
        }
    }
}