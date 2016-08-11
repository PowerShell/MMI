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
    public class NativeTestsBase
    {
        internal MI_Application Application { get { return StaticFixtures.Application; } }

        internal MI_Session Session { get { return StaticFixtures.Session; } }
        
        protected NativeTestsBase()
        {
        }
    }
}
