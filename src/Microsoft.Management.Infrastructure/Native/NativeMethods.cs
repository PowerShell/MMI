/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/
namespace Microsoft.Management.Infrastructure.Native
{
    using System;
    using System.Runtime.InteropServices;

    internal static class NativeMethods
    {
        [DllImport(MI_PlatformSpecific.MI, CallingConvention = MI_PlatformSpecific.MiMainCallConvention)]
        internal static extern MI_Result MI_Application_InitializeV1(
            UInt32 flags,
            [MarshalAs(MI_PlatformSpecific.AppropriateStringType)] string applicationID,
            MI_Instance.IndirectPtr extendedError,
            [In, Out] MI_Application.DirectPtr application
            );

        [DllImport(MI_PlatformSpecific.MOFCodecHost, CallingConvention = MI_PlatformSpecific.MiMainCallConvention)]
        internal static extern MI_Result MI_Application_NewSerializer_Mof(
            MI_Application.DirectPtr application,
            MI_SerializerFlags flags,
            [MarshalAs(UnmanagedType.LPWStr)]string format,
            MI_Serializer.DirectPtr serializer
            );

        [DllImport(MI_PlatformSpecific.MOFCodecHost, CallingConvention = MI_PlatformSpecific.MiMainCallConvention)]
        internal static extern MI_Result MI_Application_NewDeserializer_Mof(
            MI_Application.DirectPtr application,
            MI_SerializerFlags flags,
            [MarshalAs(UnmanagedType.LPWStr)] string format,
            MI_Deserializer.DirectPtr serializer
            );

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPWStr)]string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string procName);

        internal static readonly int IntPtrSize = Marshal.SizeOf<IntPtr>();

        [UnmanagedFunctionPointer(MI_PlatformSpecific.MiMainCallConvention)]
        internal delegate IntPtr MI_MainFunction(IntPtr callbackContext);

        internal static unsafe void memcpy(byte* dst, byte* src, int size, uint count)
        {
            long byteCount = size * count;
            for (long i = 0; i < byteCount; i++)
            {
                *dst++ = *src++;
            }
        }

        internal static unsafe void memset(IntPtr dst, byte val, int byteCount)
        {
            unsafe
            {
                memset((byte*)dst, val, (uint)byteCount);
            }
        }

        private static unsafe void memset(byte* dst, byte val, uint byteCount)
        {
            for (long i = 0; i < byteCount; i++)
            {
                *dst++ = val;
            }
        }
    }
}
