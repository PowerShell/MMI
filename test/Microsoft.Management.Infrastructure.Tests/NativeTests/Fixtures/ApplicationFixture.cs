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
    public class ApplicationFixture
    {
        private readonly string ApplicationName = "MMINativeTests";

        internal MI_Application Application { get; private set; }

        public ApplicationFixture()
        {
            MI_Instance extendedError = null;
            MI_Application newApplication;
            MI_Result res = MI_Application.Initialize(ApplicationName, out extendedError, out newApplication);
            MIAssert.Succeeded(res, "Expect basic application initialization to succeed");
            this.Application = newApplication;
        }

        public void Dispose()
        {
            if (this.Application != null)
            {
                var shutdownTask = Task.Factory.StartNew(() => this.Application.Close());
                bool completed = shutdownTask.Wait(TimeSpan.FromSeconds(5));
                Assert.True(completed, "Expect shutdown to complete successfully (did you leave an object open?)");
            }
        }
    }
}
