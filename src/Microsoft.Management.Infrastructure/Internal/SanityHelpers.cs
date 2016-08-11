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
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.Management.Infrastructure.Internal
{
    internal static class SanityHelpers
    {
        public static void AssertUnusedArg(object shouldBeNull)
        {
            if (shouldBeNull != null)
            {
                throw new NotSupportedException();
            }
        }
    }
}
