using System;

namespace Microsoft.Management.Infrastructure.Native
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal struct MI_Datetime
    {
        [FieldOffset(0)]
        internal bool isTimestamp;

        [FieldOffset(4)]
        internal MI_Timestamp timestamp;

        [FieldOffset(4)]
        internal MI_Interval interval;

        internal MI_Datetime(TimeSpan interval)
        {
            throw new NotImplementedException();
        }

        internal MI_Datetime(DateTime datetime)
        {
            throw new NotImplementedException();
        }
    }
}