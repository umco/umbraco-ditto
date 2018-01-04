using BenchmarkDotNet.Attributes;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Core;
using Umbraco.Core.Models;

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

        [GlobalSetup]
        public void Setup()
        {
            this.content = new MockPublishedContent
            {
                Id = 1234,
                Name = "MyCustomName",
                Properties = new[] { new MockPublishedContentProperty("item", "myValue") }
            };
        }

        [Benchmark(Baseline = true)]
        public void MapManually()
        {
            new BasicModel
            {
                Id = this.content.Id,
                Name = this.content.Name,
                Item = this.content.GetProperty("item").Value.TryConvertTo<string>().Result
            };
        }

        [Benchmark]
        public void MapWithDitto()
        {
            this.content.As<BasicModel>();
        }
    }
}