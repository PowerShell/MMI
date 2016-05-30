using Microsoft.Management.Infrastructure.Internal;

namespace MMI.Tests.Internal
{
    /// <summary>
    /// Example test that the string is correct.
    /// </summary>
    public class CimApplicationTests
    {
        [Fact]
        public void ApplicationID()
        {
            Assert.Equal("CoreCLRSingletonAppDomain", CimApplication.ApplicationID, "Expect test framework to be sane");
        }
    }
}
