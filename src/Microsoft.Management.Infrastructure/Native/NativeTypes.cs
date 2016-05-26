using System;

namespace Microsoft.Management.Infrastructure.Native
{
    using System.Runtime.InteropServices;
    
    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal struct MI_Interval
    {
        internal UInt32 days;
        internal UInt32 hours;
        internal UInt32 minutes;
        internal UInt32 seconds;
        internal UInt32 microseconds;
        internal UInt32 __padding1;
        internal UInt32 __padding2;
        internal UInt32 __padding3;

        public static implicit operator TimeSpan(MI_Interval interval)
        {
            throw new NotImplementedException();
        }

        public static implicit operator MI_Interval(TimeSpan timespan)
        {
            throw new NotImplementedException();
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal struct MI_Timestamp
    {
        internal UInt32 year;
        internal UInt32 month;
        internal UInt32 day;
        internal UInt32 hour;
        internal UInt32 minute;
        internal UInt32 second;
        internal UInt32 microseconds;
        internal Int32 utc;
    }

    [StructLayout(LayoutKind.Explicit, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal struct MI_Datetime
    {
        [FieldOffset(0)]
        internal bool isTimestamp;

        [FieldOffset(4)]
        internal MI_Timestamp timestamp;

        [FieldOffset(4)]
        internal MI_Interval interval;

        internal MI_Datetime(TimeSpan interval)
        {
            throw new NotImplementedException();
        }

        internal MI_Datetime(DateTime datetime)
        {
            throw new NotImplementedException();
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal struct MI_UsernamePasswordCreds
    {
        internal string domain;
        internal string username;
        internal string password;
    }

    [StructLayout(LayoutKind.Explicit, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal struct MI_UserCredentials
    {
        [FieldOffset(0)]
        internal IntPtr authenticationType;

        [FieldOffset(4)]
        internal MI_UsernamePasswordCreds usernamePassword;

        [FieldOffset(4)]
        internal IntPtr certificateThumbprint;

        internal string authenticationTypeString
        {
            get
            {
                return MI_PlatformSpecific.PtrToString(this.authenticationType);
            }
        }
    }

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