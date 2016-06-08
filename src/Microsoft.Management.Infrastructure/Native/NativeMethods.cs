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
            MI_Instance.MI_InstanceOutPtr extendedError,
            [In, Out] MI_Application.MI_ApplicationPtr application
            );

        [DllImport(MI_PlatformSpecific.MOFCodecHost, CallingConvention = MI_PlatformSpecific.MiMainCallConvention)]
        internal static extern MI_Result MI_Application_NewSerializer_Mof(
            MI_Application.MI_ApplicationPtr application,
            MI_SerializerFlags flags,
            [MarshalAs(UnmanagedType.LPWStr)]string format,
            MI_Serializer.MI_SerializerPtr serializer
            );

        [DllImport(MI_PlatformSpecific.MOFCodecHost, CallingConvention = MI_PlatformSpecific.MiMainCallConvention)]
        internal static extern MI_Result MI_Application_NewDeserializer_Mof(
            MI_Application.MI_ApplicationPtr application,
            MI_SerializerFlags flags,
            [MarshalAs(UnmanagedType.LPWStr)] string format,
            MI_DeserializerPtr serializer
            );

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPWStr)]string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string procName);

        internal static readonly int IntPtrSize = Marshal.SizeOf<IntPtr>();

        [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention)]
        internal delegate void MI_Session_Close_CompletionCallback(IntPtr callbackContext);

        [UnmanagedFunctionPointer(MI_PlatformSpecific.MiMainCallConvention)]
        internal delegate IntPtr MI_MainFunction(IntPtr callbackContext);

        internal delegate void MI_SessionCallbacks_WriteError(MI_Application application, object callbackContext, MI_Instance instance);

        internal delegate void MI_SessionCallbacks_WriteMessage(MI_Application application, object callbackContext, MI_WriteMessageChannel channel, string message);

        [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal delegate void MI_SessionCallbacks_WriteErrorNative(IntPtr application, IntPtr callbackContext, IntPtr instance);

        [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal delegate void MI_SessionCallbacks_WriteMessageNative(IntPtr application, IntPtr callbackContext, MI_WriteMessageChannel channel, IntPtr message);

        internal static unsafe void memcpy(byte* dst, byte* src, int size, uint count)
        {
            long byteCount = size * count;
            for (long i = 0; i < byteCount; i++)
            {
                *dst++ = *src++;
            }
        }

        internal static unsafe void memset(byte* dst, byte val, uint byteCount)
        {
            for (long i = 0; i < byteCount; i++)
            {
                *dst++ = val;
            }
        }

        internal static unsafe T GetFTAsOffsetFromPtr<T>(IntPtr ptr, int offset) where T : new()
        {
            T res = new T();
            IntPtr ftPtr = IntPtr.Zero;
            unsafe
            {
                // Just as easily could be implemented with Marshal
                // but that would copy more than the one pointer we need
                IntPtr structurePtr = ptr;
                if (structurePtr == IntPtr.Zero)
                {
                    throw new InvalidOperationException();
                }

                ftPtr = *((IntPtr*)((byte*)structurePtr + offset));
            }

            if (ftPtr == IntPtr.Zero)
            {
                throw new InvalidOperationException();
            }

            // No apparent way to implement this in an unsafe block
            Marshal.PtrToStructure(ftPtr, res);
            return res;
        }
    }
}