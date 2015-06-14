namespace Our.Umbraco.Ditto.Tests
{
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;

    [TestFixture]
    public class ExistingObjectInstanceTests
    {
        public class MyModel
        {
            public string MyProperty1 { get; set; }

            [DittoIgnore]
            public string MyProperty2 { get; set; }
        }

        [Test]
        public void Existing_Object_Mapped()
        {
            var content = new PublishedContentMock();

            var value = "Hello world";
            var model = new MyModel()
            {
                MyProperty1 = value,
                MyProperty2 = value
            };

            content.As<MyModel>(instance: model);

            Assert.That(model.MyProperty1, Is.Not.EqualTo(value));
            Assert.That(model.MyProperty2, Is.EqualTo(value));
        }
    }
}