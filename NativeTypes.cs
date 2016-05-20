using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NativeObject
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    public struct MI_Interval
    {
        public UInt32 days;
        public UInt32 hours;
        public UInt32 minutes;
        public UInt32 seconds;
        public UInt32 microseconds;
        public UInt32 __padding1;
        public UInt32 __padding2;
        public UInt32 __padding3;

	public static implicit operator MI_Interval(TimeSpan ts)
	{
	    // TODO: implement this
	    MI_Interval interval;
	    interval.days = interval.hours = interval.minutes = interval.seconds = interval.microseconds = interval.__padding1 = interval.__padding2 = interval.__padding3 = 0;
	    return interval;
	}

	public static implicit operator TimeSpan(MI_Interval interval)
	{
	    // TODO: implement this
	    TimeSpan ts = new TimeSpan(0);
	    return ts;
	}
    }

    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    public struct MI_Timestamp
    {
        public UInt32 year;
        public UInt32 month;
        public UInt32 day;
        public UInt32 hour;
        public UInt32 minute;
        public UInt32 second;
        public UInt32 microseconds;
        public Int32 utc;
    }

    [StructLayout(LayoutKind.Explicit, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    public struct MI_Datetime
    {
        [FieldOffset(0)]
        public bool isTimestamp;

        [FieldOffset(4)]
        public MI_Timestamp timestamp;

        [FieldOffset(4)]
        public MI_Interval interval;

	public MI_Datetime(System.DateTime sdt)
	{
	    // TODO: Finish this function
	    this.isTimestamp = true;
	    this.interval.days = this.interval.hours = this.interval.minutes = this.interval.seconds = this.interval.microseconds = this.interval.__padding1 = this.interval.__padding2 = this.interval.__padding3 = 0;
	    this.timestamp.year = this.timestamp.month = this.timestamp.day = this.timestamp.hour = this.timestamp.minute = this.timestamp.second = this.timestamp.microseconds = 0;
	    this.timestamp.utc = 0;
	}

	public MI_Datetime(System.TimeSpan sts)
	{
	    // TODO: Finish this function
	    this.isTimestamp = true;
	    this.interval.days = this.interval.hours = this.interval.minutes = this.interval.seconds = this.interval.microseconds = this.interval.__padding1 = this.interval.__padding2 = this.interval.__padding3 = 0;
	    this.timestamp.year = this.timestamp.month = this.timestamp.day = this.timestamp.hour = this.timestamp.minute = this.timestamp.second = this.timestamp.microseconds = 0;
	    this.timestamp.utc = 0;
	}
    }

    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    public struct MI_UsernamePasswordCreds
    {
        public string domain;
        public string username;
        public string password;
    }

    [StructLayout(LayoutKind.Explicit, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    public struct MI_UserCredentials
    {
        [FieldOffset(0)]
        public IntPtr authenticationType;

        [FieldOffset(4)]
        public MI_UsernamePasswordCreds usernamePassword;

        [FieldOffset(4)]
        public IntPtr certificateThumbprint;

        public string authenticationTypeString
        {
            get
            {
                return MI_PlatformSpecific.PtrToString(this.authenticationType);
            }
        }
    }

    public class MI_SessionCallbacks
    {
        public NativeMethods.MI_SessionCallbacks_WriteMessage writeMessage;
        public NativeMethods.MI_SessionCallbacks_WriteError writeError;

        public static MI_SessionCallbacks Null
        {
            get
            {
                return null;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    public class MI_SessionCallbacksNative
    {
        IntPtr callbackContext;
        public NativeMethods.MI_SessionCallbacks_WriteMessageNative writeMessage;
        public NativeMethods.MI_SessionCallbacks_WriteErrorNative writeError;
    }

    public class MI_OperationCallbacks
    {
        public NativeMethods.MI_OperationCallback_PromptUser promptUser;
        public NativeMethods.MI_OperationCallback_WriteError writeError;
        public NativeMethods.MI_OperationCallback_WriteMessage writeMessage;
        public NativeMethods.MI_OperationCallback_WriteProgress writeProgress;

        public NativeMethods.MI_OperationCallback_Instance instanceResult;
        public NativeMethods.MI_OperationCallback_Indication indicationResult;
        public NativeMethods.MI_OperationCallback_Class classResult;

        public NativeMethods.MI_OperationCallback_StreamedParameter streamedParameterResult;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    public class MI_OperationCallbacksNative
    {
        IntPtr callbackContext;

        public NativeMethods.MI_OperationCallback_PromptUserNative promptUser;
        public NativeMethods.MI_OperationCallback_WriteErrorNative writeError;
        public NativeMethods.MI_OperationCallback_WriteMessageNative writeMessage;
        public NativeMethods.MI_OperationCallback_WriteProgressNative writeProgress;

        public NativeMethods.MI_OperationCallback_InstanceNative instanceResult;
        public NativeMethods.MI_OperationCallback_IndicationNative indicationResult;
        public NativeMethods.MI_OperationCallback_ClassNative classResult;

        public NativeMethods.MI_OperationCallback_StreamedParameterNative streamedParameterResult;
    }
}
