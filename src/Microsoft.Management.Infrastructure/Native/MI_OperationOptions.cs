using System;
using System.Runtime.InteropServices;

namespace Microsoft.Management.Infrastructure.Native
{
    internal class MI_OperationOptions
    {
        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal struct MI_OperationOptionsPtr
        {
            internal IntPtr ptr;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal struct MI_OperationOptionsOutPtr
        {
            internal IntPtr ptr;
        }

        internal MI_Result SetInterval(
            string optionName,
            MI_Interval value,
            MI_OperationOptionsFlags flags
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
            out MI_OperationOptionsFlags flags
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
        private struct MI_OperationOptionsMembers
        {
            internal UInt64 reserved1;
            internal IntPtr reserved2;
            internal IntPtr ft;
        }

        // Marshal implements these with Reflection - pay this hit only once
        private static int MI_OperationOptionsMembersFTOffset = (int)Marshal.OffsetOf<MI_OperationOptionsMembers>("ft");

        private static int MI_OperationOptionsMembersSize = Marshal.SizeOf<MI_OperationOptionsMembers>();

        private MI_OperationOptionsPtr ptr;
        private bool isDirect;
        private Lazy<MI_OperationOptionsFT> mft;

        ~MI_OperationOptions()
        {
            Marshal.FreeHGlobal(this.ptr.ptr);
        }

        private MI_OperationOptions(bool isDirect)
        {
            this.isDirect = isDirect;
            this.mft = new Lazy<MI_OperationOptionsFT>(this.MarshalFT);

            var necessarySize = this.isDirect ? MI_OperationOptionsMembersSize : NativeMethods.IntPtrSize;
            this.ptr.ptr = Marshal.AllocHGlobal(necessarySize);

            unsafe
            {
                NativeMethods.memset((byte*)this.ptr.ptr, 0, (uint)necessarySize);
            }
        }

        internal static MI_OperationOptions NewDirectPtr()
        {
            return new MI_OperationOptions(true);
        }

        internal static MI_OperationOptions NewIndirectPtr()
        {
            return new MI_OperationOptions(false);
        }

        internal static MI_OperationOptions NewFromDirectPtr(IntPtr ptr)
        {
            var res = new MI_OperationOptions(false);
            Marshal.WriteIntPtr(res.ptr.ptr, ptr);
            return res;
        }

        public static implicit operator MI_OperationOptionsPtr(MI_OperationOptions instance)
        {
            // If the indirect pointer is zero then the object has not
            // been initialized and it is not valid to refer to its data
            if (instance != null && instance.Ptr == IntPtr.Zero)
            {
                throw new InvalidCastException();
            }

            return new MI_OperationOptionsPtr() { ptr = instance == null ? IntPtr.Zero : instance.Ptr };
        }

        public static implicit operator MI_OperationOptionsOutPtr(MI_OperationOptions instance)
        {
            // We are not currently supporting the ability to get the address
            // of our direct pointer, though it is technically feasible
            if (instance != null && instance.isDirect)
            {
                throw new InvalidCastException();
            }

            return new MI_OperationOptionsOutPtr() { ptr = instance == null ? IntPtr.Zero : instance.ptr.ptr };
        }

        internal static MI_OperationOptions Null { get { return null; } }
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
            MI_OperationOptionsFlags flags
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
            MI_OperationOptionsFlags flags
            )
        {
            MI_Result resultLocal = this.ft.SetNumber(this,
                optionName,
                value,
                flags);
            return resultLocal;
        }

        internal MI_Result SetCustomOption(
            string optionName,
            MI_Type valueType,
            MI_Value value,
            bool mustComply,
            MI_OperationOptionsFlags flags
            )
        {
            MI_Result resultLocal = this.ft.SetCustomOption(this,
                optionName,
                valueType,
                value,
                mustComply,
                flags);
            return resultLocal;
        }

        internal MI_Result GetString(
            string optionName,
            out string value,
            out UInt32 index,
            out MI_OperationOptionsFlags flags
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
            out MI_OperationOptionsFlags flags
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
            out MI_OperationOptionsFlags flags
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
            out MI_OperationOptionsFlags flags
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

        internal MI_Result GetEnabledChannels(
            string optionName,
            out UInt32 channels,
            UInt32 bufferLength,
            out UInt32 channelCount,
            out MI_OperationOptionsFlags flags
            )
        {
            MI_Result resultLocal = this.ft.GetEnabledChannels(this,
                optionName,
                out channels,
                bufferLength,
                out channelCount,
                out flags);
            return resultLocal;
        }

        internal MI_Result Clone(
            out MI_OperationOptions newOperationOptions
            )
        {
            MI_OperationOptions newOperationOptionsLocal =
                MI_OperationOptions.NewIndirectPtr();

            MI_Result resultLocal = this.ft.Clone(this,
                newOperationOptionsLocal);

            newOperationOptions = newOperationOptionsLocal;
            return resultLocal;
        }

        private MI_OperationOptionsFT ft { get { return this.mft.Value; } }

        private MI_OperationOptionsFT MarshalFT()
        {
            return NativeMethods.GetFTAsOffsetFromPtr<MI_OperationOptionsFT>(this.Ptr, MI_OperationOptions.MI_OperationOptionsMembersFTOffset);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal class MI_OperationOptionsFT
        {
            internal MI_OperationOptions_Delete Delete;
            internal MI_OperationOptions_SetString SetString;
            internal MI_OperationOptions_SetNumber SetNumber;
            internal MI_OperationOptions_SetCustomOption SetCustomOption;
            internal MI_OperationOptions_GetString GetString;
            internal MI_OperationOptions_GetNumber GetNumber;
            internal MI_OperationOptions_GetOptionCount GetOptionCount;
            internal MI_OperationOptions_GetOptionAt GetOptionAt;
            internal MI_OperationOptions_GetOption GetOption;
            internal MI_OperationOptions_GetEnabledChannels GetEnabledChannels;
            internal MI_OperationOptions_Clone Clone;
            internal MI_OperationOptions_SetInterval SetInterval;
            internal MI_OperationOptions_GetInterval GetInterval;

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_OperationOptions_Delete(
                MI_OperationOptionsPtr options
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_OperationOptions_SetString(
                MI_OperationOptionsPtr options,
                string optionName,
                string value,
                MI_OperationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_OperationOptions_SetNumber(
                MI_OperationOptionsPtr options,
                string optionName,
                UInt32 value,
                MI_OperationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_OperationOptions_SetCustomOption(
                MI_OperationOptionsPtr options,
                string optionName,
                MI_Type valueType,
                [In, Out] MI_Value.MIValueBlock value,
                [MarshalAs(UnmanagedType.U1)] bool mustComply,
                MI_OperationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_OperationOptions_GetString(
                MI_OperationOptionsPtr options,
                string optionName,
                [In, Out] MI_String value,
                out UInt32 index,
                out MI_OperationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_OperationOptions_GetNumber(
                MI_OperationOptionsPtr options,
                string optionName,
                out UInt32 value,
                out UInt32 index,
                out MI_OperationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_OperationOptions_GetOptionCount(
                MI_OperationOptionsPtr options,
                out UInt32 count
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_OperationOptions_GetOptionAt(
                MI_OperationOptionsPtr options,
                UInt32 index,
                [In, Out] MI_String optionName,
                [In, Out] MI_Value.MIValueBlock value,
                out MI_Type type,
                out MI_OperationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_OperationOptions_GetOption(
                MI_OperationOptionsPtr options,
                string optionName,
                [In, Out] MI_Value.MIValueBlock value,
                out MI_Type type,
                out UInt32 index,
                out MI_OperationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_OperationOptions_GetEnabledChannels(
                MI_OperationOptionsPtr options,
                string optionName,
                out UInt32 channels,
                UInt32 bufferLength,
                out UInt32 channelCount,
                out MI_OperationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_OperationOptions_Clone(
                MI_OperationOptionsPtr self,
                [In, Out] MI_OperationOptionsPtr newOperationOptions
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_OperationOptions_SetInterval(
                MI_OperationOptionsPtr options,
                string optionName,
                ref MI_Interval value,
                MI_OperationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_OperationOptions_GetInterval(
                MI_OperationOptionsPtr options,
                string optionName,
                ref MI_Interval value,
                out UInt32 index,
                out MI_OperationOptionsFlags flags
                );
        }
    }
}