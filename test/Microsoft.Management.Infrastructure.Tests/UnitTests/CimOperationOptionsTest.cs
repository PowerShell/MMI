/*============================================================================
* Copyright (C) Microsoft Corporation, All rights reserved.
*=============================================================================
*/

using System;
using System.Globalization;
using System.Security;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Options;
using Xunit;

namespace MMI.Tests.UnitTests
{

    public class CimOperationOptionsTest
    {
        [Fact]
        public void BaseOptions_Empty()
        {
            using (var operationOptions = new CimOperationOptions(mustUnderstand: true))
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IEnumerable<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process", operationOptions);
                Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");
                Assert.True(enumeratedInstances.Count() > 0, "Got some results back from CimSession.EnumerateInstances");
            }
        }
    }
}
