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
    internal class MI_Operation : MI_NativeObjectWithFT<MI_Operation.MI_OperationFT>
    {
        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        private struct MI_OperationMembers
        {
            internal UInt64 reserved1;
            internal IntPtr reserved2;
            internal IntPtr ft;
        }

        static MI_Operation()
        {
            CheckMembersTableMatchesNormalLayout<MI_OperationMembers>("ft");
        }
        
        private MI_Operation(bool isDirect) : base(isDirect)
        {
        }
        private MI_Operation(IntPtr existingPtr) : base(existingPtr)
        {
        }

        internal static MI_Operation NewDirectPtr()
        {
            return new MI_Operation(true);
        }

        internal static MI_Operation NewIndirectPtr()
        {
            return new MI_Operation(false);
        }

        internal static MI_Operation NewFromDirectPtr(IntPtr ptr)
        {
            return new MI_Operation(ptr);
        }

        internal void AssertValidInternalState()
        {
            throw new NotImplementedException();
        }
        
        internal static MI_Operation Null { get { return null; } }

        internal MI_Result Close()
        {
            return this.ft.Close(this);
        }

        internal MI_Result Cancel(
            MI_CancellationReason reason
            )
        {
            MI_Result resultLocal = this.ft.Cancel(this,
                reason);
            return resultLocal;
        }

        internal MI_Result GetSession(
            MI_Session session
            )
        {
            MI_Result resultLocal = this.ft.GetSession(this,
                session);
            return resultLocal;
        }

        internal MI_Result GetInstance(
            out MI_Instance instance,
            out bool moreResults,
            out MI_Result result,
            out string errorMessage,
            out MI_Instance completionDetails
            )
        {
            MI_Instance instanceLocal = MI_Instance.NewIndirectPtr();
            MI_String errorMessageLocal = MI_String.NewIndirectPtr();
            MI_Instance completionDetailsLocal = MI_Instance.NewIndirectPtr();

            MI_Result resultLocal = this.ft.GetInstance(this,
                instanceLocal,
                out moreResults,
                out result,
                errorMessageLocal,
                completionDetailsLocal);

            instance = instanceLocal;
            errorMessage = errorMessageLocal.Value;
            completionDetails = completionDetailsLocal;
            return resultLocal;
        }

        internal MI_Result GetIndication(
            out MI_Instance instance,
            out string bookmark,
            out string machineID,
            out bool moreResults,
            out MI_Result result,
            out string errorMessage,
            out MI_Instance completionDetails
            )
        {
            MI_Instance instanceLocal = MI_Instance.NewIndirectPtr();
            MI_String bookmarkLocal = MI_String.NewIndirectPtr();
            MI_String machineIDLocal = MI_String.NewIndirectPtr();
            MI_String errorMessageLocal = MI_String.NewIndirectPtr();
            MI_Instance completionDetailsLocal = MI_Instance.NewIndirectPtr();

            MI_Result resultLocal = this.ft.GetIndication(this,
                instanceLocal,
                bookmarkLocal,
                machineIDLocal,
                out moreResults,
                out result,
                errorMessageLocal,
                completionDetailsLocal);

            instance = instanceLocal;
            bookmark = bookmarkLocal.Value;
            machineID = machineIDLocal.Value;
            errorMessage = errorMessageLocal.Value;
            completionDetails = completionDetailsLocal;
            return resultLocal;
        }

        internal MI_Result GetClass(
            out MI_Class classResult,
            out bool moreResults,
            out MI_Result result,
            out string errorMessage,
            out MI_Instance completionDetails
            )
        {
            MI_Class classResultLocal = MI_Class.NewIndirectPtr();
            MI_String errorMessageLocal = MI_String.NewIndirectPtr();
            MI_Instance completionDetailsLocal = MI_Instance.NewIndirectPtr();

            MI_Result resultLocal = this.ft.GetClass(this,
                classResultLocal,
                out moreResults,
                out result,
                errorMessageLocal,
                completionDetailsLocal);

            classResult = classResultLocal;
            errorMessage = errorMessageLocal.Value;
            completionDetails = completionDetailsLocal;
            return resultLocal;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal class MI_OperationFT
        {
            internal MI_Operation_Close Close;
            internal MI_Operation_Cancel Cancel;
            internal MI_Operation_GetSession GetSession;
            internal MI_Operation_GetInstance GetInstance;
            internal MI_Operation_GetIndication GetIndication;
            internal MI_Operation_GetClass GetClass;

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Operation_Close(
                DirectPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Operation_Cancel(
                DirectPtr operation,
                MI_CancellationReason reason
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Operation_GetSession(
                DirectPtr operation,
                [In, Out] MI_Session.DirectPtr session
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Operation_GetInstance(
                DirectPtr operation,
                [In, Out] MI_Instance.IndirectPtr instance,
                [MarshalAs(UnmanagedType.U1)] out bool moreResults,
                out MI_Result result,
                [In, Out] MI_String errorMessage,
                [In, Out] MI_Instance.IndirectPtr completionDetails
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Operation_GetIndication(
                DirectPtr operation,
                [In, Out] MI_Instance.IndirectPtr instance,
                [In, Out] MI_String bookmark,
                [In, Out] MI_String machineID,
                [MarshalAs(UnmanagedType.U1)] out bool moreResults,
                out MI_Result result,
                [In, Out] MI_String errorMessage,
                [In, Out] MI_Instance.IndirectPtr completionDetails
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Operation_GetClass(
                DirectPtr operation,
                [In, Out] MI_Class.IndirectPtr classResult,
                [MarshalAs(UnmanagedType.U1)] out bool moreResults,
                out MI_Result result,
                [In, Out] MI_String errorMessage,
                [In, Out] MI_Instance.IndirectPtr completionDetails
                );
        }
    }
}
