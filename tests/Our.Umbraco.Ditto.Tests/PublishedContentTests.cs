namespace Our.Umbraco.Ditto.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::Umbraco.Core.Models;
    using NUnit.Framework;
    using Our.Umbraco.Ditto.Tests.Mocks;
    using Our.Umbraco.Ditto.Tests.Models;

    [TestFixture]
    public class PublishedContentTests
    {
        [Test]
        public void Complex_Property_Convertered()
        {
            var value = new PublishedContentMock();

            var property = new PublishedContentPropertyMock
            {
                Alias = "myprop",
                Value = value
            };

            var content = new PublishedContentMock
            {
                Id = 1234,
                Properties = new[] { property }
            };

            var model = content.As<ComplexModel>();

            Assert.That(model.Id, Is.EqualTo(1234));

            Assert.That(model.MyProperty, Is.EqualTo(value));

            Assert.That(model.MyPublishedContent, Is.InstanceOf<IPublishedContent>());
            Assert.That(model.MyPublishedContent.Id, Is.EqualTo(1234));
            Assert.That(model.MyPublishedContent.Name, Is.EqualTo("Mock Published Content"));
        }

        [Test]
        public void Custom_Value_Resolver_Resolves()
        {
            var content = new PublishedContentMock();
            var model = content.As<ComplexModel>();
            Assert.That(model.Name, Is.EqualTo("Name Test"));
        }

        [Test]
        public void Can_Resolve_Prefixed_Properties()
        {
            var prop1 = new PublishedContentPropertyMock
            {
                Alias = "siteName",
                Value = "Name"
            };
            var prop2 = new PublishedContentPropertyMock
            {
                Alias = "siteDescription",
                Value = "Description"
            };
            var prop3 = new PublishedContentPropertyMock
            {
                Alias = "fallback",
                Value = "Fallback"
            };

            var content = new PublishedContentMock
            {
                Properties = new[] { prop1, prop2, prop3 }
            };

            var converted = content.As<PrefixedModel>();

            Assert.That(converted.Name, Is.EqualTo("Name"));
            Assert.That(converted.Description, Is.EqualTo("Description"));
            Assert.That(converted.Fallback, Is.EqualTo("Fallback"));
        }

        [Test]
        public void Umbraco_Property_Attribute_Overrides_Prefix()
        {
            var prop1 = new PublishedContentPropertyMock
            {
                Alias = "siteUnprefixedProp",
                Value = "Site Unprefixed"
            };
            var prop2 = new PublishedContentPropertyMock
            {
                Alias = "unprefixedProp",
                Value = "Unprefixed"
            };

            var content = new PublishedContentMock
            {
                Properties = new[] { prop1, prop2 }
            };

            var converted = content.As<PrefixedModel>();

            Assert.That(converted.UnprefixedProp, Is.EqualTo("Unprefixed"));
        }

        [Test]
        public void Can_Resolve_Recursive_Properties_Via_Umbraco_Properties_Attribute()
        {
            var childContent = new PublishedContentMock();
            var parentContent = new PublishedContentMock
            {
                Properties = new[]
                {
                    new PublishedContentPropertyMock
                    {
                        Alias = "description",
                        Value = "Description"
                    }
                },
                Children = new[]
                {
                    childContent
                }
            };

            childContent.Parent = parentContent;

            var converted = childContent.As<PrefixedModel>();

            Assert.That(converted.Description, Is.EqualTo("Description"));
        }

        [Test]
        public void IPublishedContent_Property_Value_Triggers_Recursive_As()
        {
            var content1 = new PublishedContentMock
            {
                Properties = new[]
                {
                    new PublishedContentPropertyMock
                    {
                        Alias = "innerProp",
                        Value = "Inner Prop"
                    }
                }
            };

            var content2 = new PublishedContentMock
            {
                Properties = new[]
                {
                    new PublishedContentPropertyMock
                    {
                        Alias = "contentProp",
                        Value = content1
                    }
                }
            };

            var model = content2.As<InnerContentModel>();

            Assert.That(model.ContentProp.InnerProp, Is.EqualTo("Inner Prop"));
        }

        [Test]
        public void IEnumerable_IPublishedContent_Property_Value_Triggers_Recursive_As()
        {
            var contentList = new List<IPublishedContent>();

            for (var i = 0; i < 5; i++)
            {
                contentList.Add(new PublishedContentMock
                {
                    Id = i,
                    Name = "Node " + i,
                    Properties = new[]
                    {
                        new PublishedContentPropertyMock
                        {
                            Alias = "innerProp",
                            Value = "Inner Prop"
                        }
                    }
                });
            }

            var content2 = new PublishedContentMock
            {
                Properties = new[]
                {
                    new PublishedContentPropertyMock
                    {
                        Alias = "contentListProp",
                        Value = contentList
                    }
                }
            };

            var model = content2.As<InnerContentModel>();

            Assert.That(model.ContentListProp.Count(), Is.EqualTo(5));
            Assert.That(model.ContentListProp.All(x => x.InnerProp == "Inner Prop"), Is.EqualTo(true));
        }

        [Test]
        public void Can_As_To_Castable_Interface()
        {
            var content = new PublishedContentMock { Id = 123, Name = "Test" };
            var content2 = content.As<IPublishedContent>();

            Assert.That(content2.Id, Is.EqualTo(content.Id));
            Assert.That(content2.Name, Is.EqualTo(content.Name));
        }
    }
}