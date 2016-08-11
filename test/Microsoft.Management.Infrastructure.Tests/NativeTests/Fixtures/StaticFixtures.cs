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

namespace MMI.Tests.Native
{
    public static class StaticFixtures
    {
        private static Lazy<ApplicationFixture> appFixture;

        internal static MI_Application Application { get { return appFixture.Value.Application; } }

        private static Lazy<SessionFixture> sessionFixture;

        internal static MI_Session Session { get { return sessionFixture.Value.Session; } }

        static StaticFixtures()
        {
            // This stuff leaks
            appFixture = new Lazy<ApplicationFixture>(() => new ApplicationFixture());
            sessionFixture = new Lazy<SessionFixture>(() => new SessionFixture());
        }
    }
}
