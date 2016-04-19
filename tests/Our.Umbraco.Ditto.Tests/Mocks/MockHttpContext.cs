using System.Web;

namespace Our.Umbraco.Ditto.Tests.Mocks
{
    public class MockHttpContext : HttpContextBase
    {
        public override HttpRequestBase Request { get { return null; } }
    }
}