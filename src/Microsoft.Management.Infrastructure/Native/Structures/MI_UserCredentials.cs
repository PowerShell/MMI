/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/
using System;

namespace Microsoft.Management.Infrastructure.Native
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal struct MI_UserCredentials
    {
        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal struct MI_UsernamePasswordCreds
        {
            internal string domain;
            internal string username;
            internal string password;
        }

        [FieldOffset(0)]
        internal IntPtr authenticationType;

        [FieldOffset(16)]
        internal MI_UsernamePasswordCreds usernamePassword;

        [FieldOffset(8)]
        internal IntPtr certificateThumbprint;

        internal string authenticationTypeString
        {
            get
            {
                return MI_PlatformSpecific.PtrToString(this.authenticationType);
            }
        }
    }
}
