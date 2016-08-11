/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/
using System;

namespace Microsoft.Management.Infrastructure.Native
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal struct MI_Timestamp
    {
        internal UInt32 year;
        internal UInt32 month;
        internal UInt32 day;
        internal UInt32 hour;
        internal UInt32 minute;
        internal UInt32 second;
        internal UInt32 microseconds;
        internal Int32 utc;
    }
}
