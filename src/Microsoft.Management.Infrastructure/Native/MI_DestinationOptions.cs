﻿using System;
using System.Runtime.InteropServices;

namespace Microsoft.Management.Infrastructure.Native
{
    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal struct MI_DestinationOptionsPtr
    {
        internal IntPtr ptr;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal struct MI_DestinationOptionsOutPtr
    {
        internal IntPtr ptr;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal class MI_DestinationOptions
    {
        internal MI_Result SetInterval(
            string optionName,
            MI_Interval value,
            MI_DestinationOptionsFlags flags
            )
        {
            MI_Result resultLocal = this.ft.SetInterval(this,
                optionName,
                ref value,
                flags);
            return resultLocal;
        }

        internal MI_Result GetInterval(
            string optionName,
            out MI_Interval value,
            out UInt32 index,
            out MI_DestinationOptionsFlags flags
            )
        {
            MI_Interval valueLocal = new MI_Interval();
            MI_Result resultLocal = this.ft.GetInterval(this,
                optionName,
                ref valueLocal,
                out index,
                out flags);

            value = valueLocal;
            return resultLocal;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        private struct MI_DestinationOptionsMembers
        {
            internal UInt64 reserved1;
            internal IntPtr reserved2;
            internal IntPtr ft;
        }

        // Marshal implements these with Reflection - pay this hit only once
        private static int MI_DestinationOptionsMembersFTOffset = (int)Marshal.OffsetOf<MI_DestinationOptionsMembers>("ft");

        private static int MI_DestinationOptionsMembersSize = Marshal.SizeOf<MI_DestinationOptionsMembers>();

        private MI_DestinationOptionsPtr ptr;
        private bool isDirect;
        private Lazy<MI_DestinationOptionsFT> mft;

        ~MI_DestinationOptions()
        {
            Marshal.FreeHGlobal(this.ptr.ptr);
        }

        private MI_DestinationOptions(bool isDirect)
        {
            this.isDirect = isDirect;
            this.mft = new Lazy<MI_DestinationOptionsFT>(this.MarshalFT);

            var necessarySize = this.isDirect ? MI_DestinationOptionsMembersSize : NativeMethods.IntPtrSize;
            this.ptr.ptr = Marshal.AllocHGlobal(necessarySize);

            unsafe
            {
                NativeMethods.memset((byte*)this.ptr.ptr, 0, (uint)necessarySize);
            }
        }

        internal static MI_DestinationOptions NewDirectPtr()
        {
            return new MI_DestinationOptions(true);
        }

        internal static MI_DestinationOptions NewIndirectPtr()
        {
            return new MI_DestinationOptions(false);
        }

        internal static MI_DestinationOptions NewFromDirectPtr(IntPtr ptr)
        {
            var res = new MI_DestinationOptions(false);
            Marshal.WriteIntPtr(res.ptr.ptr, ptr);
            return res;
        }

        public static implicit operator MI_DestinationOptionsPtr(MI_DestinationOptions instance)
        {
            // If the indirect pointer is zero then the object has not
            // been initialized and it is not valid to refer to its data
            if (instance != null && instance.Ptr == IntPtr.Zero)
            {
                throw new InvalidCastException();
            }

            return new MI_DestinationOptionsPtr() { ptr = instance == null ? IntPtr.Zero : instance.Ptr };
        }

        public static implicit operator MI_DestinationOptionsOutPtr(MI_DestinationOptions instance)
        {
            // We are not currently supporting the ability to get the address
            // of our direct pointer, though it is technically feasible
            if (instance != null && instance.isDirect)
            {
                throw new InvalidCastException();
            }

            return new MI_DestinationOptionsOutPtr() { ptr = instance == null ? IntPtr.Zero : instance.ptr.ptr };
        }

        internal static MI_DestinationOptions Null { get { return null; } }
        internal bool IsNull { get { return this.Ptr == IntPtr.Zero; } }

        internal IntPtr Ptr
        {
            get
            {
                IntPtr structurePtr = this.ptr.ptr;
                if (!this.isDirect)
                {
                    if (structurePtr == IntPtr.Zero)
                    {
                        throw new InvalidOperationException();
                    }

                    // This can be easily implemented with Marshal.ReadIntPtr
                    // but that has function call overhead
                    unsafe
                    {
                        structurePtr = *(IntPtr*)structurePtr;
                    }
                }

                return structurePtr;
            }
        }

        internal void Delete()
        {
            this.ft.Delete(this);
        }

        internal MI_Result SetString(
            string optionName,
            string value,
            MI_DestinationOptionsFlags flags
            )
        {
            MI_Result resultLocal = this.ft.SetString(this,
                optionName,
                value,
                flags);
            return resultLocal;
        }

        internal MI_Result SetNumber(
            string optionName,
            UInt32 value,
            MI_DestinationOptionsFlags flags
            )
        {
            MI_Result resultLocal = this.ft.SetNumber(this,
                optionName,
                value,
                flags);
            return resultLocal;
        }

        internal MI_Result AddCredentials(
            string optionName,
            MI_UserCredentials credentials,
            MI_DestinationOptionsFlags flags
            )
        {
            MI_Result resultLocal = this.ft.AddCredentials(this,
                optionName,
                credentials,
                flags);
            return resultLocal;
        }

        internal MI_Result GetString(
            string optionName,
            out string value,
            out UInt32 index,
            out MI_DestinationOptionsFlags flags
            )
        {
            MI_String valueLocal = MI_String.NewIndirectPtr();

            MI_Result resultLocal = this.ft.GetString(this,
                optionName,
                valueLocal,
                out index,
                out flags);

            value = valueLocal.Value;
            return resultLocal;
        }

        internal MI_Result GetNumber(
            string optionName,
            out UInt32 value,
            out UInt32 index,
            out MI_DestinationOptionsFlags flags
            )
        {
            MI_Result resultLocal = this.ft.GetNumber(this,
                optionName,
                out value,
                out index,
                out flags);
            return resultLocal;
        }

        internal MI_Result GetOptionCount(
            out UInt32 count
            )
        {
            MI_Result resultLocal = this.ft.GetOptionCount(this,
                out count);
            return resultLocal;
        }

        internal MI_Result GetOptionAt(
            UInt32 index,
            out string optionName,
            MI_Value value,
            out MI_Type type,
            out MI_DestinationOptionsFlags flags
            )
        {
            MI_String optionNameLocal = MI_String.NewIndirectPtr();

            MI_Result resultLocal = this.ft.GetOptionAt(this,
                index,
                optionNameLocal,
                value,
                out type,
                out flags);

            optionName = optionNameLocal.Value;
            return resultLocal;
        }

        internal MI_Result GetOption(
            string optionName,
            MI_Value value,
            out MI_Type type,
            out UInt32 index,
            out MI_DestinationOptionsFlags flags
            )
        {
            MI_Result resultLocal = this.ft.GetOption(this,
                optionName,
                value,
                out type,
                out index,
                out flags);
            return resultLocal;
        }

        internal MI_Result GetCredentialsCount(
            out UInt32 count
            )
        {
            MI_Result resultLocal = this.ft.GetCredentialsCount(this,
                out count);
            return resultLocal;
        }

        internal MI_Result GetCredentialsAt(
            UInt32 index,
            out string optionName,
            out MI_UserCredentials credentials,
            out MI_DestinationOptionsFlags flags
            )
        {
            MI_String optionNameLocal = MI_String.NewIndirectPtr();
            MI_UserCredentials credentialsLocal = new MI_UserCredentials();

            MI_Result resultLocal = this.ft.GetCredentialsAt(this,
                index,
                optionNameLocal,
                ref credentialsLocal,
                out flags);

            optionName = optionNameLocal.Value;
            credentials = credentialsLocal;
            return resultLocal;
        }

        internal MI_Result GetCredentialsPasswordAt(
            UInt32 index,
            out string optionName,
            string password,
            UInt32 bufferLength,
            out UInt32 passwordLength,
            out MI_DestinationOptionsFlags flags
            )
        {
            MI_String optionNameLocal = MI_String.NewIndirectPtr();

            MI_Result resultLocal = this.ft.GetCredentialsPasswordAt(this,
                index,
                optionNameLocal,
                password,
                bufferLength,
                out passwordLength,
                out flags);

            optionName = optionNameLocal.Value;
            return resultLocal;
        }

        internal MI_Result Clone(
            out MI_DestinationOptions newDestinationOptions
            )
        {
            MI_DestinationOptions newDestinationOptionsLocal =
                MI_DestinationOptions.NewIndirectPtr();

            MI_Result resultLocal = this.ft.Clone(this,
                newDestinationOptionsLocal);

            newDestinationOptions = newDestinationOptionsLocal;
            return resultLocal;
        }

        private MI_DestinationOptionsFT ft { get { return this.mft.Value; } }

        private MI_DestinationOptionsFT MarshalFT()
        {
            return NativeMethods.GetFTAsOffsetFromPtr<MI_DestinationOptionsFT>(this.Ptr, MI_DestinationOptions.MI_DestinationOptionsMembersFTOffset);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal class MI_DestinationOptionsFT
        {
            internal MI_DestinationOptions_Delete Delete;
            internal MI_DestinationOptions_SetString SetString;
            internal MI_DestinationOptions_SetNumber SetNumber;
            internal MI_DestinationOptions_AddCredentials AddCredentials;
            internal MI_DestinationOptions_GetString GetString;
            internal MI_DestinationOptions_GetNumber GetNumber;
            internal MI_DestinationOptions_GetOptionCount GetOptionCount;
            internal MI_DestinationOptions_GetOptionAt GetOptionAt;
            internal MI_DestinationOptions_GetOption GetOption;
            internal MI_DestinationOptions_GetCredentialsCount GetCredentialsCount;
            internal MI_DestinationOptions_GetCredentialsAt GetCredentialsAt;
            internal MI_DestinationOptions_GetCredentialsPasswordAt GetCredentialsPasswordAt;
            internal MI_DestinationOptions_Clone Clone;
            internal MI_DestinationOptions_SetInterval SetInterval;
            internal MI_DestinationOptions_GetInterval GetInterval;

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_DestinationOptions_Delete(
                MI_DestinationOptionsPtr options
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_SetString(
                MI_DestinationOptionsPtr options,
                string optionName,
                string value,
                MI_DestinationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_SetNumber(
                MI_DestinationOptionsPtr options,
                string optionName,
                UInt32 value,
                MI_DestinationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_AddCredentials(
                MI_DestinationOptionsPtr options,
                string optionName,
                MI_UserCredentials credentials,
                MI_DestinationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_GetString(
                MI_DestinationOptionsPtr options,
                string optionName,
                [In, Out] MI_String value,
                out UInt32 index,
                out MI_DestinationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_GetNumber(
                MI_DestinationOptionsPtr options,
                string optionName,
                out UInt32 value,
                out UInt32 index,
                out MI_DestinationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_GetOptionCount(
                MI_DestinationOptionsPtr options,
                out UInt32 count
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_GetOptionAt(
                MI_DestinationOptionsPtr options,
                UInt32 index,
                [In, Out] MI_String optionName,
                [In, Out] MI_Value.MIValueBlock value,
                out MI_Type type,
                out MI_DestinationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_GetOption(
                MI_DestinationOptionsPtr options,
                string optionName,
                [In, Out] MI_Value.MIValueBlock value,
                out MI_Type type,
                out UInt32 index,
                out MI_DestinationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_GetCredentialsCount(
                MI_DestinationOptionsPtr options,
                out UInt32 count
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_GetCredentialsAt(
                MI_DestinationOptionsPtr options,
                UInt32 index,
                [In, Out] MI_String optionName,
                ref MI_UserCredentials credentials,
                out MI_DestinationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_GetCredentialsPasswordAt(
                MI_DestinationOptionsPtr options,
                UInt32 index,
                [In, Out] MI_String optionName,
                string password,
                UInt32 bufferLength,
                out UInt32 passwordLength,
                out MI_DestinationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_Clone(
                MI_DestinationOptionsPtr self,
                [In, Out] MI_DestinationOptionsPtr newDestinationOptions
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_SetInterval(
                MI_DestinationOptionsPtr options,
                string optionName,
                ref MI_Interval value,
                MI_DestinationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_GetInterval(
                MI_DestinationOptionsPtr options,
                string optionName,
                ref MI_Interval value,
                out UInt32 index,
                out MI_DestinationOptionsFlags flags
                );
        }
    }
}