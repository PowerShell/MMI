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
    public class SessionFixture : IDisposable
    {
        internal MI_Session Session { get; private set; }

        public SessionFixture()
        {
            var application = StaticFixtures.Application;

            MI_Session newSession;
            MI_Instance extendedError = null;
            MI_Result res = application.NewSession(null,
                    null,
                    MI_DestinationOptions.Null,
                    MI_SessionCreationCallbacks.Null,
                    out extendedError,
                    out newSession);
            MIAssert.Succeeded(res, "Expect simple NewSession to succeed");
            this.Session = newSession;
        }

        public virtual void Dispose()
        {
            if (this.Session != null)
            {
                this.Session.Close(IntPtr.Zero, null);
            }
        }
    }
}
