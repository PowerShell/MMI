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

    internal class MI_PlatformSpecific
    {
#if !_LINUX
        internal const UnmanagedType AppropriateStringType = UnmanagedType.LPWStr;
        internal const CharSet AppropriateCharSet = CharSet.Unicode;
        internal const CallingConvention MiMainCallConvention = CallingConvention.Cdecl;
        internal const CallingConvention MiCallConvention = CallingConvention.StdCall;
        internal const string MI = "mi.dll";
        internal const string MOFCodecHost = "mimofcodec.dll";

        internal static string PtrToString(IntPtr ptr)
        {
            return Marshal.PtrToStringUni(ptr);
        }

        internal static IntPtr StringToPtr(string str)
        {
            return Marshal.StringToHGlobalUni(str);
        }

#else
        internal const UnmanagedType AppropriateStringType = UnmanagedType.LPStr;
        internal const CharSet AppropriateCharSet = CharSet.Ansi;
        internal const CallingConvention MiMainCallConvention = CallingConvention.Cdecl;
        internal const CallingConvention MiCallConvention = CallingConvention.Cdecl;
        internal const string MI = "libmi";
        internal const string MOFCodecHost = "libmi";

        internal static string PtrToString(IntPtr ptr)
        {
            return Marshal.PtrToStringAnsi(ptr);
        }

        internal static IntPtr StringToPtr(string str)
        {
            return Marshal.StringToHGlobalAnsi(str);
        }
#endif
    }
}
