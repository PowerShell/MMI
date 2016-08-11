using System;

namespace Microsoft.Management.Infrastructure.Native
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal struct MI_UserCredentials
    {
        [FieldOffset(0)]
        private IntPtr authenticationType;

        [FieldOffset(8)]
        private IntPtr certificateThumbprint;

        [FieldOffset(8)]
        private IntPtr domain;

        [FieldOffset(16)]
        private IntPtr username;

        [FieldOffset(24)]
        private IntPtr password;


        internal string authenticationTypeString
        {
            get
            {
                return MI_PlatformSpecific.PtrToString(this.authenticationType);
            }
            set
            {
                if (this.authenticationType != IntPtr.Zero)
                {
                    unsafe
                    {
                        Marshal.FreeHGlobal(this.authenticationType);
                    }
                    this.authenticationType = IntPtr.Zero;
                }

                this.authenticationType = MI_PlatformSpecific.StringToPtr(value);
            }
        }

        internal string certificateThumbprintString
        {
            get
            {
                return MI_PlatformSpecific.PtrToString(this.certificateThumbprint);
            }
            set
            {
                if (this.certificateThumbprint != IntPtr.Zero)
                {
                    unsafe
                    {
                        Marshal.FreeHGlobal(this.certificateThumbprint);
                    }
                    this.certificateThumbprint = IntPtr.Zero;
                }

                this.certificateThumbprint = MI_PlatformSpecific.StringToPtr(value);
            }
        }

        internal string domainString
        {
            get
            {
                return MI_PlatformSpecific.PtrToString(this.domain);
            }
            set
            {
                if (this.domain != IntPtr.Zero)
                {
                    unsafe
                    {
                        Marshal.FreeHGlobal(this.domain);
                    }
                    this.domain = IntPtr.Zero;
                }

                this.domain = MI_PlatformSpecific.StringToPtr(value);
            }
        }

        internal string usernameString
        {
            get
            {
                return MI_PlatformSpecific.PtrToString(this.username);
            }
            set
            {
                if (this.username != IntPtr.Zero)
                {
                    unsafe
                    {
                        Marshal.FreeHGlobal(this.username);
                    }
                    this.username = IntPtr.Zero;
                }

                this.username = MI_PlatformSpecific.StringToPtr(value);
            }
        }

        internal string passwordString
        {
            get
            {
                return MI_PlatformSpecific.PtrToString(this.password);
            }
            set
            {
                if (this.password != IntPtr.Zero)
                {
                    unsafe
                    {
                        Marshal.FreeHGlobal(this.password);
                    }
                    this.password = IntPtr.Zero;
                }

                this.password = MI_PlatformSpecific.StringToPtr(value);
            }
        }


    }
}