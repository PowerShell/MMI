/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


using System;
using System.Diagnostics;
using Microsoft.Management.Infrastructure.Native;

namespace Microsoft.Management.Infrastructure.Internal
{
    internal static class MiFlagsExtensionMethods
    {
        public static CimFlags ToCimFlags(this MI_Flags miFlags)
        {
            return (CimFlags)miFlags;
        }
        public static MI_Flags FromCimFlags(this CimFlags cimFlags)
        {
            return (MI_Flags)cimFlags;
        }
    }
}
