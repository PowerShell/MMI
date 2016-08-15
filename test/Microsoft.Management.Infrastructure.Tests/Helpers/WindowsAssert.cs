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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Management.Infrastructure.Native;

namespace MMI.Tests
{

    internal static class WindowsAssert
    {
        internal static void Equal<T>(T expected, T actual)
        {
#if !_LINUX
            Assert.Equal(expected, actual);
#endif
        }
        internal static void Equal<T>(T expected, T actual, string message)
        {
#if !_LINUX
            Assert.Equal(expected, actual, message);
#endif
        }
    }
}
