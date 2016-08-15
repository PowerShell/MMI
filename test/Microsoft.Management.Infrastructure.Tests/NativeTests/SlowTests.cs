/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/
using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Management.Infrastructure.Native;
using MMI.Tests;
using Xunit;

namespace MMI.Tests.Native
{
    public class SlowTests
    {
        public class SessionTestConnection
        {
            [WindowsFact]
            public void BadHost()
            {
                MI_Session badSession;
                MI_Instance extendedError = null;
                MI_Result res = StaticFixtures.Application.NewSession(null,
                        "badhost",
                        MI_DestinationOptions.Null,
                        MI_SessionCreationCallbacks.Null,
                        out extendedError,
                        out badSession);
                MIAssert.Succeeded(res, "Expect simple NewSession to succeed");

                MI_Operation operation = null;
                badSession.TestConnection(0, null, out operation);

                bool moreResults;
                MI_Result result;
                string errorMessage = null;
                MI_Instance instance = null;
                MI_Instance errorDetails = null;
                res = operation.GetInstance(out instance, out moreResults, out result, out errorMessage, out errorDetails);
                MIAssert.Succeeded(res, "Expect the GetInstance operation to succeed");
                MIAssert.Failed(result, "Expect the actual retrieval to fail");
                Assert.True(!String.IsNullOrEmpty(errorMessage), "Expect error message to be available");

                res = operation.Close();
                MIAssert.Succeeded(res, "Expect to be able to close operation now");

                res = badSession.Close(IntPtr.Zero, null);
                MIAssert.Succeeded(res, "Expect to be able to close the bad session");
            }
        }
    }
}
