using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;
using Umbraco.Core;

namespace Our.Umbraco.Ditto.Tests
{
    [SetUpFixture]
    public class GlobalSetupFixture
    {
        [SetUp]
        public void Setup()
        {
            // Let Ditto know that we're running in a unit-test scenario
            Ditto.IsRunningInUnitTest = true;

            // Configure the accessor to use the mock contexts
            Ditto.RegisterContextAccessor<MockDittoContextAccessor>();

            // In order for the unit-test to work, we need to populate the attributed
            //-types resolver with the object types that have resolvable attributes.

            var assemblies = new[]
            {
                this.GetType().Assembly,
                typeof(Ditto).Assembly
            };

            // Set the `DittoCache` attributed types
            if (AttributedTypeResolver<DittoCacheAttribute>.HasCurrent == false)
            {
                AttributedTypeResolver<DittoCacheAttribute>.Current = AttributedTypeResolver<DittoCacheAttribute>.Create(
                    TypeFinder.FindClassesWithAttribute<DittoCacheAttribute>(assemblies, true));
            }

            // Set the `DittoDefaultProcessor` attributed types
            if (AttributedTypeResolver<DittoDefaultProcessorAttribute>.HasCurrent == false)
            {
                AttributedTypeResolver<DittoDefaultProcessorAttribute>.Current = AttributedTypeResolver<DittoDefaultProcessorAttribute>.Create(
                    TypeFinder.FindClassesWithAttribute<DittoDefaultProcessorAttribute>(assemblies, true));
            }

            // Set the `DittoProcessorMetaDataAttribute` attributed types
            if (AttributedTypeResolver<DittoProcessorMetaDataAttribute>.HasCurrent == false)
            {
                AttributedTypeResolver<DittoProcessorMetaDataAttribute>.Current = AttributedTypeResolver<DittoProcessorMetaDataAttribute>.Create(
                    TypeFinder.FindClassesOfType<DittoProcessorAttribute>(assemblies, false));
            }

            // Set the `UmbracoProperties` attributed types
            if (AttributedTypeResolver<UmbracoPropertiesAttribute>.HasCurrent == false)
            {
                AttributedTypeResolver<UmbracoPropertiesAttribute>.Current = AttributedTypeResolver<UmbracoPropertiesAttribute>.Create(
                    TypeFinder.FindClassesWithAttribute<UmbracoPropertiesAttribute>(assemblies, true));
            }
        }
    }
}