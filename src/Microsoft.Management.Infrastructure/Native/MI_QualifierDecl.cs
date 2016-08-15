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
    internal class MI_QualifierDecl
    {
        private struct MI_QualifierDeclMembers
        {
            public IntPtr name;
            public MI_Type type;
            public UInt32 scope;
            public UInt32 flavor;
            public UInt32 subscript;
            public IntPtr value;
        }

        ~MI_QualifierDecl()
        {
            unsafe
            {
                if (this.ptr != IntPtr.Zero)
                {
                    IntPtr valuePtr = this.ptr + MI_QualifierDeclValueOffset;
                    Marshal.FreeHGlobal(*(IntPtr*)valuePtr);
                }
            }

            Marshal.FreeHGlobal(this.ptr);
        }

        private static int MI_QualifierDeclMembersSize = Marshal.SizeOf<MI_QualifierDeclMembers>();
        private static int MI_QualifierDeclValueOffset = (int)Marshal.OffsetOf<MI_QualifierDeclMembers>("value");

        private IntPtr ptr;

        private MI_QualifierDecl(string name,
            MI_Type type,
            MI_Flags scope,
            MI_Flags flavor,
            UInt32 subscript,
            IntPtr value)
        {
            this.AllocateQualifierDecl(name, type, scope, flavor, subscript, value);
        }

        private MI_QualifierDecl(string name,
            MI_Type type,
            MI_Flags scope,
            MI_Flags flavor,
            UInt32 subscript,
            bool value)
        {
            var valuePtr = Marshal.AllocHGlobal(Marshal.SizeOf<Byte>());
            Marshal.WriteByte(valuePtr, (byte)(value ? 1 : 0));
            this.AllocateQualifierDecl(name, type, scope, flavor, subscript, valuePtr);
        }

        private void AllocateQualifierDecl(string name,
            MI_Type type,
            MI_Flags scope,
            MI_Flags flavor,
            UInt32 subscript,
            IntPtr value)
        {
            var dest = new MI_QualifierDeclMembers();
            dest.name = MI_PlatformSpecific.StringToPtr(name);
            dest.type = type;
            dest.scope = (UInt32)scope;
            dest.flavor = (UInt32)flavor;
            dest.subscript = subscript;
            dest.value = value;

            this.ptr = Marshal.AllocHGlobal(MI_QualifierDeclMembersSize);
            Marshal.StructureToPtr(dest, this.ptr, false);
        }

        internal static MI_QualifierDecl NewDirectPtr(string name,
            MI_Type type,
            MI_Flags scope,
            MI_Flags flavor,
            UInt32 subscript,
            IntPtr value)
        {
            return new MI_QualifierDecl(name, type, scope, flavor, subscript, value);
        }

        internal static MI_QualifierDecl NewDirectPtr(string name,
            MI_Type type,
            MI_Flags scope,
            MI_Flags flavor,
            UInt32 subscript,
            bool value)
        {
            return new MI_QualifierDecl(name, type, scope, flavor, subscript, value);
        }
    }
}
