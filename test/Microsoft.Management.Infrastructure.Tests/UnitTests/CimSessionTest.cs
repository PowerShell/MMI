/*============================================================================
* Copyright (C) Microsoft Corporation, All rights reserved.
*=============================================================================
*/
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Options;
using Xunit;

namespace MMI.Tests.UnitTests
{
    public class CimSessionTest
    {
        #region Test create
        [Fact]
        public void Create_ComputerName_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                Assert.Null(cimSession.ComputerName, "cimSession.ComputerName should not be the same as the value passed to Create method");
            }
        }

        [Fact]
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

        [Fact]
        public void BaseOptions_Empty()
        {
            var sessionOptions = new CimSessionOptions();
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void WSManOptions_SetDestinationPort()
        {
            // TODO/FIXME - add unit test for corner cases (0, > 65535)

            var sessionOptions = new WSManSessionOptions();
            sessionOptions.DestinationPort = (uint)8080;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void WSManOptions_SetProxyCredential()
        {
            var sessionOptions = new WSManSessionOptions();
            //sessionOptions.DestinationPort = 8080;
            CimCredential cred = new CimCredential(ImpersonatedAuthenticationMechanism.None); //wsman accepts only username/password
            sessionOptions.AddProxyCredentials(cred);
            //Exception is thrown after creating the session as WSMAN doesn't allow proxy without username/password.
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

    }
}