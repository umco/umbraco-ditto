using BenchmarkDotNet.Attributes;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Our.Umbraco.Ditto.PerformanceTests
{
    [MemoryDiagnoser]
    public class PublishedContentMapping
    {
        public class BasicModel
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public string Item { get; set; }
        }

        private IPublishedContent content;

        public class NullContextAccessor : IDittoContextAccessor
        {
            public UmbracoContext UmbracoContext => default(UmbracoContext);
            public ApplicationContext ApplicationContext => default(ApplicationContext);
        }

        public class NullProcessorAttribute : DittoProcessorAttribute
        {
            public override object ProcessValue() { return null; }
        }

        [GlobalSetup]
        public void Setup()
        {
            this.content = new MockPublishedContent
            {
                Id = 1234,
                Name = "MyCustomName",
                Properties = new[] { new MockPublishedContentProperty("item", "myValue") }
            };

            // Try running at a complete minimum, strip back everything
            Ditto.RegisterContextAccessor<NullContextAccessor>();
            Ditto.RegisterDefaultProcessorType<NullProcessorAttribute>();
            Ditto.DeregisterPostProcessorType<HtmlStringAttribute>();
            Ditto.DeregisterPostProcessorType<EnumerableConverterAttribute>();
            Ditto.DeregisterPostProcessorType<RecursiveDittoAttribute>();
            Ditto.DeregisterPostProcessorType<TryConvertToAttribute>();

            // pre-load the type config
            DittoTypeConfigCache.Add<BasicModel>();
        }

        [Benchmark(Baseline = true)]
        public void MapManually()
        {
            new BasicModel
            {
                Id = default(int), // this.content.Id,
                Name = default(string), // this.content.Name,
                Item = default(string) // this.content.GetProperty("item").Value.TryConvertTo<string>().Result
            };
        }

        [Benchmark]
        public void MapWithDitto()
        {
            this.content.As<BasicModel>();
        }
    }
}