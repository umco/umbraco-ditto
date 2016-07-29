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
    public class BasicMappingTests
    {
        public class BasicModel
        {
            public string Name { get; set; }
        }

        public class BasicModelWithId
        {
            public int Id { get; set; }

            [UmbracoProperty("Id")]
            public int MyProperty { get; set; }
        }

        public class BasicModelWithStringProperty
        {
            public string MyProperty { get; set; }
        }

        public class BasicModelWithPublishedContentProperty
        {
            public IPublishedContent MyProperty { get; set; }
        }

        public class BasicModelWithItemProperty
        {
            public string Item { get; set; }
        }

        public class BasicModelWithChildrenProperty
        {
            public string Children { get; set; }
        }

        [Test]
        public void Basic_Name_IsMapped()
        {
            var name = "MyCustomName";

            var content = new MockPublishedContent
            {
                Name = name
            };

            var model = content.As<BasicModel>();

            Assert.That(model.Name, Is.EqualTo(name));
        }

        [Test]
        public void Basic_Id_And_Property_IsMapped()
        {
            var id = 1234;

            var content = new MockPublishedContent
            {
                Id = id
            };

            var model = content.As<BasicModelWithId>();

            Assert.That(model.Id, Is.EqualTo(id));
            Assert.That(model.MyProperty, Is.EqualTo(id));
        }

        [Test]
        public void Basic_String_Property_IsMapped()
        {
            var value = "myValue";

            var content = new MockPublishedContent
            {
                Properties = new[] { new MockPublishedContentProperty("myProperty", value) }
            };

            var model = content.As<BasicModelWithStringProperty>();

            Assert.That(model.MyProperty, Is.EqualTo(value));
        }

        [Test]
        public void Basic_PublishedContent_Property_IsMapped()
        {
            var value = new MockPublishedContent();

            var content = new MockPublishedContent
            {
                Properties = new[] { new MockPublishedContentProperty("myProperty", value) }
            };

            var model = content.As<BasicModelWithPublishedContentProperty>();

            Assert.That(model.MyProperty, Is.EqualTo(value));
        }

        [Test]
        public void Basic_Content_Item_Property_IsMapped()
        {
            var value = "myValue";

            var content = new MockPublishedContent
            {
                Properties = new[] { new MockPublishedContentProperty("item", value) }
            };

            var model = content.As<BasicModelWithItemProperty>();

            Assert.That(model.Item, Is.EqualTo(value));
        }

        [Test]
        public void Basic_Content_Reserved_Property_Warns()
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
            var value = "myValue";
            var content = new MockPublishedContent
            {
                Properties = new[] { new MockPublishedContentProperty("children", value) }
            };

            // Perform the conversion
            var model = content.As<BasicModelWithChildrenProperty>();

            // Ensure a warning message was logged
            var logMessages = mockLogger.GetLogMessages().Where(x => x.MessageType == LogMessageType.Warn && x.CallingType == typeof(UmbracoPropertyAttribute));

            // Turn debugging back off (can effect other tests if left enabled)
            ConfigurationManager.AppSettings["Ditto:DebugEnabled"] = "false";

            Assert.NotNull(logMessages);
            Assert.That(logMessages.Any(x => x.Message.Contains("hides")));
        }

        [Test]
        public void Basic_Property_To_String_Exception()
        {
            // The source is an `IPublishedContent`, the target is a `string`, type mismatch exception
            var value = new MockPublishedContent();

            var content = new MockPublishedContent
            {
                Properties = new[] { new MockPublishedContentProperty("myProperty", value) }
            };

            TestDelegate code = () =>
            {
                // We are passing an `IPublishedContent` object to a property (of type `string`),
                // so we know that internally Ditto will try calling `content.As<string>()`,
                // which will throw an `InvalidOperationException` exception.
                var model = content.As<BasicModelWithStringProperty>();
            };

            Assert.Throws<InvalidOperationException>(code);
        }

        [Test]
        public void Basic_Content_To_String_Exception()
        {
            var content = new MockPublishedContent();

            TestDelegate code = () =>
            {
                // Since a `string` does not have a parameterless constructor,
                // Ditto can not map the `IPublishedContent` object,
                // which will throw an `InvalidOperationException` exception.
                var model = content.As<string>();
            };

            Assert.Throws<InvalidOperationException>(code);
        }
    }
}