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
        }
    }
}