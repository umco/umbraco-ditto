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
            // Let Ditto know that we're running in a unit-test scenario (in debug mode)
            Ditto.IsRunningInUnitTest = true;
            Ditto.IsDebuggingEnabled = Ditto.GetDebugFlag();

            // Configure the accessor to use the mock contexts
            Ditto.RegisterContextAccessor<MockDittoContextAccessor>();
        }
    }
}