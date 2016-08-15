/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/
using Microsoft.Management.Infrastructure.Internal;
using Xunit;

namespace MMI.Tests.Internal
{
    /// <summary>
    /// Example test that the string is correct.
    /// </summary>
    public class CimApplicationTests
    {
        [Fact]
        public void ApplicationIDSane()
        {
            Assert.Equal("CoreCLRSingletonAppDomain", CimApplication.ApplicationID, "Expect test framework to be sane");
        }

        [Fact]
        public void CimApplicationInitializes()
        {
            Assert.NotNull(CimApplication.Handle, "Expect the application handle to initialize properly");
            Assert.False(CimApplication.Handle.IsNull, "Expect the pointer to be logically non-null");
        }
    }
}
