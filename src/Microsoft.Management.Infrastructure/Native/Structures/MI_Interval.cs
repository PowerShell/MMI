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
    internal struct MI_Interval
    {
        internal UInt32 days;
        internal UInt32 hours;
        internal UInt32 minutes;
        internal UInt32 seconds;
        internal UInt32 microseconds;
        internal UInt32 __padding1;
        internal UInt32 __padding2;
        internal UInt32 __padding3;

        public static implicit operator TimeSpan(MI_Interval interval)
        {
            throw new NotImplementedException();
        }

        public static implicit operator MI_Interval(TimeSpan timespan)
        {
            throw new NotImplementedException();
        }
    }
}
