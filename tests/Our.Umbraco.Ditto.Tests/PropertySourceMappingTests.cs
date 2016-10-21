using System.Configuration;
using System.Linq;
using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Core.Logging;
using Umbraco.Core.ObjectResolution;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Mapping")]
    public class PropertySourceMappingTests
    {
        public class BasicModelProperty
        {
            public string Name { get; set; }

            public string Url { get; set; }

            public string Custom { get; set; }
        }

        [UmbracoProperties(PropertySource = PropertySource.UmbracoThenInstanceProperties)]
        public class BasicModelProperty2
        {
            public string Name { get; set; }

            [UmbracoProperty(PropertySource = PropertySource.InstanceThenUmbracoProperties)]
            public string Url { get; set; }

            public string Custom { get; set; }
        }

        [Test]
        public void PropertySource_Properties_Map()
        {
            var instanceName = "Instance Name";
            var instanceUrl = "/instance/url";
            var customName = "Custom Name";
            var customProp = "Custom Prop";

            // Create a hidden mapping
            var content = new MockPublishedContent
            {
                Name = instanceName,
                Url = instanceUrl,
                Properties = new[]
                {
                    new MockPublishedContentProperty("name", customName),
                    new MockPublishedContentProperty("custom", customProp)
                }
            };

            Ditto.DefaultPropertySource = PropertySource.InstanceThenUmbracoProperties;

            var model = content.As<BasicModelProperty>();
            Assert.AreEqual(instanceName, model.Name);
            Assert.AreEqual(instanceUrl, model.Url);
            Assert.AreEqual(customProp, model.Custom);

            Ditto.DefaultPropertySource = PropertySource.UmbracoThenInstanceProperties;

            model = content.As<BasicModelProperty>();
            Assert.AreEqual(customName, model.Name);
            Assert.AreEqual(instanceUrl, model.Url);
            Assert.AreEqual(customProp, model.Custom);

            Ditto.DefaultPropertySource = PropertySource.InstanceProperties;

            model = content.As<BasicModelProperty>();
            Assert.AreEqual(instanceName, model.Name);
            Assert.AreEqual(instanceUrl, model.Url);
            Assert.IsNull(model.Custom);

            Ditto.DefaultPropertySource = PropertySource.UmbracoProperties;

            model = content.As<BasicModelProperty>();
            Assert.AreEqual(customName, model.Name);
            Assert.IsNull(model.Url);
            Assert.AreEqual(customProp, model.Custom);

            // Reset
            Ditto.DefaultPropertySource = PropertySource.InstanceThenUmbracoProperties;
        }

        [Test]
        public void PropertySource_Attributed_Properties_Map()
        {
            var instanceName = "Instance Name";
            var instanceUrl = "/instance/url";
            var customName = "Custom Name";
            var customProp = "Custom Prop";

            // Create a hidden mapping
            var content = new MockPublishedContent
            {
                Name = instanceName,
                Url = instanceUrl,
                Properties = new[]
                {
                    new MockPublishedContentProperty("name", customName),
                    new MockPublishedContentProperty("custom", customProp)
                }
            };

            var model = content.As<BasicModelProperty2>();

            Assert.AreEqual(customName, model.Name);
            Assert.AreEqual(instanceUrl, model.Url);
            Assert.AreEqual(customProp, model.Custom);
        }

        [Test]
        public void PropertySource_Hidden_Properties_Warn()
        {
            var t = Ditto.IsDebuggingEnabled;

            Ditto.SetDebuggingState(true);

            var s = Ditto.IsDebuggingEnabled;


            // Create a mock logger
            var mockLogger = new MockLogger();
            //
            // TODO: [LK:2016-08-12] Could we initalize this in an `SetUpFixture` method?
            // Then it could apply across all unit-tests.
            //
            if (ResolverBase<LoggerResolver>.HasCurrent)
            {
                ResolverBase<LoggerResolver>.Current.SetLogger(mockLogger);
            }
            else
            {
                ResolverBase<LoggerResolver>.Current = new LoggerResolver(mockLogger) { CanResolveBeforeFrozen = true };
            }

            // Create a hidden mapping
            var instanceName = "Instance Name";
            var instanceUrl = "/instance/url";
            var customName = "Custom Name";
            var customProp = "Custom Prop";

            var content = new MockPublishedContent
            {
                Name = instanceName,
                Url = instanceUrl,
                Properties = new[]
                {
                    new MockPublishedContentProperty("name", customName),
                    new MockPublishedContentProperty("custom", customProp)
                }
            };

            // Check for hidden umbraco properties
            Ditto.DefaultPropertySource = PropertySource.InstanceThenUmbracoProperties;
            
            var model = content.As<BasicModelProperty>();
            
            var logMessages = mockLogger.GetLogMessages().Where(x => x.MessageType == LogMessageType.Warn && x.CallingType == typeof(UmbracoPropertyAttribute));

            Assert.NotNull(logMessages);
            Assert.That(logMessages.Any(x => x.Message.Contains("hides a property in the umbraco properties collection")));

            // Check for hidden instance properties
            mockLogger.ClearLogMessages();

            Ditto.DefaultPropertySource = PropertySource.UmbracoThenInstanceProperties;

            model = content.As<BasicModelProperty>();

            logMessages = mockLogger.GetLogMessages().Where(x => x.MessageType == LogMessageType.Warn && x.CallingType == typeof(UmbracoPropertyAttribute));

            Assert.NotNull(logMessages);
            Assert.That(logMessages.Any(x => x.Message.Contains("hides an instance property")));

            // Reset
            Ditto.DefaultPropertySource = PropertySource.InstanceThenUmbracoProperties;
            Ditto.SetDebuggingState(false);
        }
    }
}