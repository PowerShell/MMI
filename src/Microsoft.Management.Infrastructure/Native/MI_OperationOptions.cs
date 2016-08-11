/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Management.Infrastructure.Native
{
    internal class MI_OperationOptions : MI_NativeObjectWithFT<MI_OperationOptions.MI_OperationOptionsFT>
    {
        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        private struct MI_OperationOptionsMembers
        {
            internal UInt64 reserved1;
            internal IntPtr reserved2;
            internal IntPtr ft;
        }

        static MI_OperationOptions()
        {
            CheckMembersTableMatchesNormalLayout<MI_OperationOptionsMembers>("ft");
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

        private MI_OperationOptions(bool isDirect) : base(isDirect)
        {
        }

        private MI_OperationOptions(IntPtr existingPtr) : base(existingPtr)
        {
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
            return new MI_OperationOptions(ptr);
        }
        
        internal static MI_OperationOptions Null { get { return null; } }

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
                DirectPtr options
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_OperationOptions_SetString(
                DirectPtr options,
                string optionName,
                string value,
                MI_OperationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_OperationOptions_SetNumber(
                DirectPtr options,
                string optionName,
                UInt32 value,
                MI_OperationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_OperationOptions_SetCustomOption(
                DirectPtr options,
                string optionName,
                MI_Type valueType,
                [In, Out] MI_Value.DirectPtr value,
                [MarshalAs(UnmanagedType.U1)] bool mustComply,
                MI_OperationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_OperationOptions_GetString(
                DirectPtr options,
                string optionName,
                [In, Out] MI_String value,
                out UInt32 index,
                out MI_OperationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_OperationOptions_GetNumber(
                DirectPtr options,
                string optionName,
                out UInt32 value,
                out UInt32 index,
                out MI_OperationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_OperationOptions_GetOptionCount(
                DirectPtr options,
                out UInt32 count
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_OperationOptions_GetOptionAt(
                DirectPtr options,
                UInt32 index,
                [In, Out] MI_String optionName,
                [In, Out] MI_Value.DirectPtr value,
                out MI_Type type,
                out MI_OperationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_OperationOptions_GetOption(
                DirectPtr options,
                string optionName,
                [In, Out] MI_Value.DirectPtr value,
                out MI_Type type,
                out UInt32 index,
                out MI_OperationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_OperationOptions_GetEnabledChannels(
                DirectPtr options,
                string optionName,
                out UInt32 channels,
                UInt32 bufferLength,
                out UInt32 channelCount,
                out MI_OperationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_OperationOptions_Clone(
                DirectPtr self,
                [In, Out] DirectPtr newOperationOptions
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_OperationOptions_SetInterval(
                DirectPtr options,
                string optionName,
                ref MI_Interval value,
                MI_OperationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_OperationOptions_GetInterval(
                DirectPtr options,
                string optionName,
                ref MI_Interval value,
                out UInt32 index,
                out MI_OperationOptionsFlags flags
                );
        }
    }
}
