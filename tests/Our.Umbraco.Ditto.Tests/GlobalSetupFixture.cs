using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;

namespace Our.Umbraco.Ditto.Tests
{
    [SetUpFixture]
    public class GlobalSetupFixture
    {
        [SetUp]
        public void Setup()
        {
            Ditto.RegisterContextAccessor<MockDittoContextAccessor>();

            // In order for the unit-test to work, we need to populate the attributed
            //-types resolver with the object types that have resolvable attributes.

            // Manually set the `DittoCache` attributed types
            if (AttributedTypeResolver<DittoCacheAttribute>.HasCurrent == false)
            {
                AttributedTypeResolver<DittoCacheAttribute>.Current = AttributedTypeResolver<DittoCacheAttribute>.Create(new[]
                {
                    typeof(CacheTests.MyValueResolverModel2)
                });
            }

            // Manually set the `DittoDefaultProcessor` attributed types
            if (AttributedTypeResolver<DittoDefaultProcessorAttribute>.HasCurrent == false)
            {
                AttributedTypeResolver<DittoDefaultProcessorAttribute>.Current = AttributedTypeResolver<DittoDefaultProcessorAttribute>.Create(new[]
                {
                    typeof(DefaultProcessorTests.MyCustomModel)
                });
            }

            // Manually set the `UmbracoProperties` attributed types
            if (AttributedTypeResolver<UmbracoPropertiesAttribute>.HasCurrent == false)
            {
                AttributedTypeResolver<UmbracoPropertiesAttribute>.Current = AttributedTypeResolver<UmbracoPropertiesAttribute>.Create(new[]
                {
                    typeof(PropertySourceMappingTests.BasicModelProperty2),
                    typeof(PrefixedPropertyTests.MyModel),
                    typeof(InheritedClassWithPrefixedPropertyTests.InheritedModel)
                });
            }
        }
    }
}