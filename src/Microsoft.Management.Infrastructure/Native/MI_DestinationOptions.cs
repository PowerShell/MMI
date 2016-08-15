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
    internal class MI_DestinationOptions : MI_NativeObjectWithFT<MI_DestinationOptions.MI_DestinationOptionsFT>
    {
        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        private struct MI_DestinationOptionsMembers
        {
            internal UInt64 reserved1;
            internal IntPtr reserved2;
            internal IntPtr ft;
        }

        static MI_DestinationOptions()
        {
            CheckMembersTableMatchesNormalLayout<MI_DestinationOptionsMembers>("ft");
        }

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

        private MI_DestinationOptions(bool isDirect) : base(isDirect)
        {
        }

        private MI_DestinationOptions(IntPtr existingPtr) : base(existingPtr)
        {
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
            return new MI_DestinationOptions(ptr);
        }
        
        internal static MI_DestinationOptions Null { get { return null; } }

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
                DirectPtr options
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_SetString(
                DirectPtr options,
                string optionName,
                string value,
                MI_DestinationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_SetNumber(
                DirectPtr options,
                string optionName,
                UInt32 value,
                MI_DestinationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_AddCredentials(
                DirectPtr options,
                string optionName,
                MI_UserCredentials credentials,
                MI_DestinationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_GetString(
                DirectPtr options,
                string optionName,
                [In, Out] MI_String value,
                out UInt32 index,
                out MI_DestinationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_GetNumber(
                DirectPtr options,
                string optionName,
                out UInt32 value,
                out UInt32 index,
                out MI_DestinationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_GetOptionCount(
                DirectPtr options,
                out UInt32 count
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_GetOptionAt(
                DirectPtr options,
                UInt32 index,
                [In, Out] MI_String optionName,
                [In, Out] MI_Value.DirectPtr value,
                out MI_Type type,
                out MI_DestinationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_GetOption(
                DirectPtr options,
                string optionName,
                [In, Out] MI_Value.DirectPtr value,
                out MI_Type type,
                out UInt32 index,
                out MI_DestinationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_GetCredentialsCount(
                DirectPtr options,
                out UInt32 count
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_GetCredentialsAt(
                DirectPtr options,
                UInt32 index,
                [In, Out] MI_String optionName,
                ref MI_UserCredentials credentials,
                out MI_DestinationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_GetCredentialsPasswordAt(
                DirectPtr options,
                UInt32 index,
                [In, Out] MI_String optionName,
                string password,
                UInt32 bufferLength,
                out UInt32 passwordLength,
                out MI_DestinationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_Clone(
                DirectPtr self,
                [In, Out] DirectPtr newDestinationOptions
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_SetInterval(
                DirectPtr options,
                string optionName,
                ref MI_Interval value,
                MI_DestinationOptionsFlags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_DestinationOptions_GetInterval(
                DirectPtr options,
                string optionName,
                ref MI_Interval value,
                out UInt32 index,
                out MI_DestinationOptionsFlags flags
                );
        }
    }
}
