using System;
using System.Configuration;
using System.Linq;
using System.Web;
using Moq;
using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
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

        [Test]
        public void PropertySource_Properties_Map()
        {
            var instanceName = "Instance Name";
            var instanceUrl = "/instance/url";
            var custonName = "Custom Name";
            var custonProp = "Custom Prop";

            // Create a hidden mapping
            var content = new MockPublishedContent
            {
                Name = instanceName,
                Url = instanceUrl,
                Properties = new[]
                {
                    new MockPublishedContentProperty("name", custonName),
                    new MockPublishedContentProperty("custom", custonProp)
                }
            };

            Ditto.DefaultPropertySource = PropertySource.InstanceThenUmbracoProperties;

            var model = content.As<BasicModelProperty>();
            Assert.AreEqual(instanceName, model.Name);
            Assert.AreEqual(instanceUrl, model.Url);
            Assert.AreEqual(custonProp, model.Custom);

            Ditto.DefaultPropertySource = PropertySource.UmbracoThenInstanceProperties;

            model = content.As<BasicModelProperty>();
            Assert.AreEqual(custonName, model.Name);
            Assert.AreEqual(instanceUrl, model.Url);
            Assert.AreEqual(custonProp, model.Custom);

            Ditto.DefaultPropertySource = PropertySource.InstanceProperties;

            model = content.As<BasicModelProperty>();
            Assert.AreEqual(instanceName, model.Name);
            Assert.AreEqual(instanceUrl, model.Url);
            Assert.IsNull(model.Custom);

            Ditto.DefaultPropertySource = PropertySource.UmbracoProperties;

            model = content.As<BasicModelProperty>();
            Assert.AreEqual(custonName, model.Name);
            Assert.IsNull(model.Url);
            Assert.AreEqual(custonProp, model.Custom);

            // Reset
            Ditto.DefaultPropertySource = PropertySource.InstanceThenUmbracoProperties;
        }

        [Test]
        public void PropertySource_Hidden_Properties_Warn()
        {
            ConfigurationManager.AppSettings["Ditto:DebugEnabled"] = "true";

            // Create a mock logger
            var mockLogger = new MockLogger();
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
            var custonName = "Custom Name";
            var custonProp = "Custom Prop";

            var content = new MockPublishedContent
            {
                Name = instanceName,
                Url = instanceUrl,
                Properties = new[]
                {
                    new MockPublishedContentProperty("name", custonName),
                    new MockPublishedContentProperty("custom", custonProp)
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
            ConfigurationManager.AppSettings["Ditto:DebugEnabled"] = "false";
        }
    }
}