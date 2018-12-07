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
            // Configure the accessor to use the mock contexts
            Ditto.RegisterContextAccessor<MockDittoContextAccessor>();
        }
    }
}