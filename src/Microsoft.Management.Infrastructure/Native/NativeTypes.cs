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
}