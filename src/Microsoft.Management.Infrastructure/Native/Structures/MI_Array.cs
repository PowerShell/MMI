using System;
using System.Runtime.InteropServices;

namespace Microsoft.Management.Infrastructure.Native
{
    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal struct MI_Array
    {
        internal IntPtr data;
        internal UInt32 size;
    }
}