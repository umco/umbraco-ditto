namespace Our.Umbraco.Ditto.Tests
{
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;

    [TestFixture]
    public class AppSettingsTests
    {
        public class MyAppSettingsModel
        {
            [AppSetting("MyAppSettingKey")]
            public string MyProperty { get; set; }
        }

        [Test]
        public void AppSetting_Property_Returned()
        {
            var value = "MyAppSettingValue";

            var content = ContentBuilder.Default().Build();

            var model = content.As<MyAppSettingsModel>();

            Assert.That(model.MyProperty, Is.EqualTo(value));
        }
    }
}