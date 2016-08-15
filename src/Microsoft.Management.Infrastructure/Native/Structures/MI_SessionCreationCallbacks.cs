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
    internal class MI_SessionCreationCallbacks
    {
        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal class MI_SessionCreationCallbacksNative
        {
            private IntPtr callbackContext;
            internal MI_SessionCallbacks_WriteMessageNative writeMessage;
            internal MI_SessionCallbacks_WriteErrorNative writeError;

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_SessionCallbacks_WriteErrorNative(IntPtr application, IntPtr callbackContext, IntPtr instance);

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_SessionCallbacks_WriteMessageNative(IntPtr application, IntPtr callbackContext, MI_WriteMessageChannel channel, IntPtr message);
        }

        internal MI_SessionCallbacks_WriteMessage writeMessage;
        internal MI_SessionCallbacks_WriteError writeError;

        internal static MI_SessionCreationCallbacks Null
        {
            get
            {
                return null;
            }
        }

        internal delegate void MI_SessionCallbacks_WriteError(MI_Application application, object callbackContext, MI_Instance instance);

        internal delegate void MI_SessionCallbacks_WriteMessage(MI_Application application, object callbackContext, MI_WriteMessageChannel channel, string message);
    }
}
