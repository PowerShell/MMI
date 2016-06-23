/*============================================================================
* Copyright (C) Microsoft Corporation, All rights reserved.
*=============================================================================
*/

namespace Microsoft.Management.Infrastructure.UnitTests
{
    using Microsoft.Management.Infrastructure;
    using MMI.Tests;

    public class CimSessionTest
    {
        #region Test create
        [TDDFact]
        public void Create_ComputerName_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                MMI.Tests.Assert.NotNull(cimSession, "cimSession should not be null");
                MMI.Tests.Assert.Null(cimSession.ComputerName, "cimSession.ComputerName should not be the same as the value passed to Create method");
            }
        }

        [TDDFact]
        public void Create_ComputerName_Localhost()
        {
            using (CimSession cimSession = CimSession.Create("localhost"))
            {
                MMI.Tests.Assert.NotNull(cimSession, "cimSession should not be null");
                MMI.Tests.Assert.Equal("localhost", cimSession.ComputerName, "cimSession.ComputerName should not be the same as the value passed to Create method");
                MMI.Tests.Assert.True(cimSession.ToString().Contains("localhost"), "cimSession.ToString should contain computer name");
            }
        }
        // Todo: will add more test cases
        #endregion Test create
    }
}