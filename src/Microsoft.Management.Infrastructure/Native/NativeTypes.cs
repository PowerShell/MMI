using System;

namespace Microsoft.Management.Infrastructure.Native
{
    using System.Runtime.InteropServices;
    
    internal class MI_SessionCallbacks
    {
        internal NativeMethods.MI_SessionCallbacks_WriteMessage writeMessage;
        internal NativeMethods.MI_SessionCallbacks_WriteError writeError;

        internal static MI_SessionCallbacks Null
        {
            get
            {
                return null;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal class MI_SessionCallbacksNative
    {
        private IntPtr callbackContext;
        internal NativeMethods.MI_SessionCallbacks_WriteMessageNative writeMessage;
        internal NativeMethods.MI_SessionCallbacks_WriteErrorNative writeError;
    }

    internal class MI_OperationCallbacks
    {
        internal NativeMethods.MI_OperationCallback_PromptUser promptUser;
        internal NativeMethods.MI_OperationCallback_WriteError writeError;
        internal NativeMethods.MI_OperationCallback_WriteMessage writeMessage;
        internal NativeMethods.MI_OperationCallback_WriteProgress writeProgress;

        internal NativeMethods.MI_OperationCallback_Instance instanceResult;
        internal NativeMethods.MI_OperationCallback_Indication indicationResult;
        internal NativeMethods.MI_OperationCallback_Class classResult;

        internal NativeMethods.MI_OperationCallback_StreamedParameter streamedParameterResult;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal class MI_OperationCallbacksNative
    {
        private IntPtr callbackContext;

        internal NativeMethods.MI_OperationCallback_PromptUserNative promptUser;
        internal NativeMethods.MI_OperationCallback_WriteErrorNative writeError;
        internal NativeMethods.MI_OperationCallback_WriteMessageNative writeMessage;
        internal NativeMethods.MI_OperationCallback_WriteProgressNative writeProgress;

        internal NativeMethods.MI_OperationCallback_InstanceNative instanceResult;
        internal NativeMethods.MI_OperationCallback_IndicationNative indicationResult;
        internal NativeMethods.MI_OperationCallback_ClassNative classResult;

        internal NativeMethods.MI_OperationCallback_StreamedParameterNative streamedParameterResult;
    }
}