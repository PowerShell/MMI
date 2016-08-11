/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/

using Microsoft.Management.Infrastructure;
using Xunit;

namespace MMI.Tests.UnitTests
{
    


    public class CimSessionTest
    {
        #region Test create
        [TDDFact]
        public void Create_ComputerName_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                Assert.Null(cimSession.ComputerName, "cimSession.ComputerName should not be the same as the value passed to Create method");
            }
        }

        [TDDFact]
        public void Create_ComputerName_Localhost()
        {
            using (CimSession cimSession = CimSession.Create("localhost"))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                Assert.Equal("localhost", cimSession.ComputerName, "cimSession.ComputerName should not be the same as the value passed to Create method");
                Assert.True(cimSession.ToString().Contains("localhost"), "cimSession.ToString should contain computer name");
            }
        }
        // Todo: will add more test cases
        #endregion Test create
    }
}
