using Xunit;

namespace Microsoft.Management.Infrastructure.Internal
{
    /// <summary>
    /// Example test that the string is correct.
    /// </summary>
    public class CimApplicationTests
    {
        [Fact]
        public void ApplicationID()
        {
            Assert.Equal("CoreCLRSingletonAppDomain", CimApplication.ApplicationID);
        }
    }
}
