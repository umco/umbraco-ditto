using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Processors")]
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
            var content = new MockPublishedContent();

            var model = content.As<MyAppSettingsModel>();

            Assert.That(model.MyProperty, Is.EqualTo(value));
        }

        [Test(Description = "This unit-test is used as a benchmark against the Ditto value-resolver.")]
        public void AppSetting_Native_Returned()
        {
            var value = "MyAppSettingValue";
            var result = System.Web.Configuration.WebConfigurationManager.AppSettings["MyAppSettingKey"];

            Assert.That(result, Is.EqualTo(value));
        }
    }
}