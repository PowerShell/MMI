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
    internal class MI_OperationCallbacks
    {
        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal class MI_OperationCallbacksNative
        {
            private IntPtr callbackContext;

            internal MI_OperationCallback_PromptUserNative promptUser;
            internal MI_OperationCallback_WriteErrorNative writeError;
            internal MI_OperationCallback_WriteMessageNative writeMessage;
            internal MI_OperationCallback_WriteProgressNative writeProgress;

            internal MI_OperationCallback_InstanceNative instanceResult;
            internal MI_OperationCallback_IndicationNative indicationResult;
            internal MI_OperationCallback_ClassNative classResult;

            internal MI_OperationCallback_StreamedParameterNative streamedParameterResult;
        }

        internal MI_OperationCallback_PromptUser promptUser;
        internal MI_OperationCallback_WriteError writeError;
        internal MI_OperationCallback_WriteMessage writeMessage;
        internal MI_OperationCallback_WriteProgress writeProgress;

        internal MI_OperationCallback_Instance instanceResult;
        internal MI_OperationCallback_Indication indicationResult;
        internal MI_OperationCallback_Class classResult;

        internal MI_OperationCallback_StreamedParameter streamedParameterResult;

        internal delegate void MI_OperationCallback_PromptUserResult(MI_Operation operation, MI_OperationCallback_ResponseType responseType);

        internal delegate void MI_OperationCallback_ResultAcknowledgement(MI_Operation operation);

        internal delegate void MI_OperationCallback_PromptUser(MI_Operation operation, object callbackContext, string message, MI_PromptType promptType, MI_OperationCallback_PromptUserResult promptUserResult);

        internal delegate void MI_OperationCallback_WriteError(MI_Operation operation, object callbackContext, MI_Instance instance, MI_OperationCallback_PromptUserResult promptUserResult);

        internal delegate void MI_OperationCallback_WriteMessage(MI_Operation operation, object callbackContext, MI_WriteMessageChannel channel, string message);

        internal delegate void MI_OperationCallback_WriteProgress(MI_Operation operation, object callbackContext, string activity, string currentOperation, string statusDescription, UInt32 percentageComplete, UInt32 secondsRemaining);

        internal delegate void MI_OperationCallback_Instance(MI_Operation operation, object callbackContext, MI_Instance instance, bool moreResults, MI_Result resultCode, string errorString, MI_Instance errorDetails, MI_OperationCallback_ResultAcknowledgement resultAcknowledgement);

        internal delegate void MI_OperationCallback_Indication(MI_Operation operation, object callbackContext, MI_Instance instance, string bookmark, string machineID, bool moreResults, MI_Result resultcode, string errorString, MI_Instance errorDetails, MI_OperationCallback_ResultAcknowledgement resultAcknowledgement);

        internal delegate void MI_OperationCallback_Class(MI_Operation operation, object callbackContext, MI_Class classResult, bool moreResults, MI_Result resultCode, string errorString, MI_Instance errorDetails, MI_OperationCallback_ResultAcknowledgement resultAcknowledgement);

        internal delegate void MI_OperationCallback_StreamedParameter(MI_Operation operation, object callbackContext, string parameterName, MI_Type resultType, MI_Value result, MI_OperationCallback_ResultAcknowledgement resultAcknowledgement);

        [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal delegate void MI_OperationCallback_PromptUserNative(MI_Operation operation, object callbackContext, string message, MI_PromptType promptType, MI_OperationCallback_PromptUserResult promptUserResult);

        [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal delegate void MI_OperationCallback_WriteErrorNative(MI_Operation operation, object callbackContext, MI_Instance instance, MI_OperationCallback_PromptUserResult promptUserResult);

        [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal delegate void MI_OperationCallback_WriteMessageNative(MI_Operation operation, object callbackContext, MI_WriteMessageChannel channel, string message);

        [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal delegate void MI_OperationCallback_WriteProgressNative(MI_Operation operation, object callbackContext, string activity, string currentOperation, string statusDescription, UInt32 percentageComplete, UInt32 secondsRemaining);

        [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal delegate void MI_OperationCallback_InstanceNative(MI_Operation operation, object callbackContext, MI_Instance instance, bool moreResults, MI_Result resultCode, string errorString, MI_Instance errorDetails, MI_OperationCallback_ResultAcknowledgement resultAcknowledgement);

        [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal delegate void MI_OperationCallback_IndicationNative(MI_Operation operation, object callbackContext, MI_Instance instance, string bookmark, string machineID, bool moreResults, MI_Result resultcode, string errorString, MI_Instance errorDetails, MI_OperationCallback_ResultAcknowledgement resultAcknowledgement);

        [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal delegate void MI_OperationCallback_ClassNative(MI_Operation operation, object callbackContext, MI_Class classResult, bool moreResults, MI_Result resultCode, string errorString, MI_Instance errorDetails, MI_OperationCallback_ResultAcknowledgement resultAcknowledgement);

        [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal delegate void MI_OperationCallback_StreamedParameterNative(MI_Operation operation, object callbackContext, string parameterName, MI_Type resultType, MI_Value result, MI_OperationCallback_ResultAcknowledgement resultAcknowledgement);
    }
}
