using System.Linq;
using Lucene.Net.Store;
using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Ditto.Tests
{
	[TestFixture]
	public class PublishedContentTests
	{
		[Test]
		public void Name_IsMapped()
		{
			var name = "MyCustomName";
			var content = ContentBuilder.Default()
				.WithName(name)
				.Build();

			//Do your Ditto magic here, and assert it maps as it should
			Assert.That(content.Name, Is.EqualTo(name));
		}

		[Test]
		public void Children_Counted()
		{
			var child = ContentBuilder.Default().Build();

			var content = ContentBuilder.Default()
				.AddChild(child)
				.Build();

			//Do your Ditto magic here, and assert it maps as it should
			Assert.That(content.Children.Count(), Is.EqualTo(1));
		}

		[Test]
		public void Property_Returned()
		{
			IPublishedContentProperty prop = PropertyBuilder.Default("myprop", "myval").Build();
			var content = ContentBuilder.Default()
				.AddProperty(prop)
				.Build();

			//Do your Ditto magic here, and assert it maps as it should
			Assert.That(content.GetPropertyValue<string>("myprop"), Is.EqualTo("myval"));
		}

		[Test]
		public void Property_Converted()
		{
			// With this kind of mocking, we dont need property value converters, because they would already 
			// have run at this point, so we just mock the result of the conversion.

			var picked = ContentBuilder.Default().Build();
			IPublishedContentProperty prop = PropertyBuilder.Default("myprop", picked).Build();
			var content = ContentBuilder.Default()
				.AddProperty(prop)
				.Build();

			//Do your Ditto magic here, and assert it maps as it should
			Assert.That(content.GetPropertyValue<IPublishedContent>("myprop"), Is.EqualTo(picked));
		}
	}
}
