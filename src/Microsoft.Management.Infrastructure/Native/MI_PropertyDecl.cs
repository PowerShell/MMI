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
    internal class MI_PropertyDecl
    {
        private struct MI_PropertyDeclMembers
        {
            public MI_Flags flags;
            public UInt32 code;
            public IntPtr name;
            public IntPtr qualifiers;
            public UInt32 numQualifiers;
            public MI_Type type;
            public IntPtr className;
            public UInt32 subscript;
            public UInt32 offset;
            public IntPtr origin;
            public IntPtr propagator;
            public IntPtr value;
        }

        ~MI_PropertyDecl()
        {
            unsafe
            {
                if (this.ptr != IntPtr.Zero)
                {
                    IntPtr valuePtr = this.ptr + MI_PropertyDeclValueOffset;
                    Marshal.FreeHGlobal(*(IntPtr*)valuePtr);
                }
            }

            Marshal.FreeHGlobal(this.ptr);
        }

        private static int MI_PropertyDeclMembersSize = Marshal.SizeOf<MI_PropertyDeclMembers>();
        private static int MI_PropertyDeclValueOffset = (int)Marshal.OffsetOf<MI_PropertyDeclMembers>("value");

        private IntPtr ptr;

        private MI_PropertyDecl(MI_Flags flags,
            UInt32 code,
            string name,
            IntPtr qualifiers,
            UInt32 numQualifiers,
            MI_Type type,
            string className,
            UInt32 subscript,
            UInt32 offset,
            string origin,
            string propagator,
            IntPtr value)
        {
            this.AllocateQualifierDecl(
                flags,
                code,
                name,
                qualifiers,
                numQualifiers,
                type,
                className,
                subscript,
                offset,
                origin,
                propagator,
                value);
        }

        private void AllocateQualifierDecl(MI_Flags flags,
            UInt32 code,
            string name,
            IntPtr qualifiers,
            UInt32 numQualifiers,
            MI_Type type,
            string className,
            UInt32 subscript,
            UInt32 offset,
            string origin,
            string propagator,
            IntPtr value)
        {
            var dest = new MI_PropertyDeclMembers();
            dest.flags = flags;
            dest.code = code;
            dest.name = MI_PlatformSpecific.StringToPtr(name);
            dest.qualifiers = qualifiers;
            dest.numQualifiers = numQualifiers;
            dest.type = type;
            dest.className = MI_PlatformSpecific.StringToPtr(className);
            dest.subscript = subscript;
            dest.offset = offset;
            dest.origin = MI_PlatformSpecific.StringToPtr(origin);
            dest.propagator = MI_PlatformSpecific.StringToPtr(propagator);
            dest.value = value;

            this.ptr = Marshal.AllocHGlobal(MI_PropertyDeclMembersSize);
            Marshal.StructureToPtr(dest, this.ptr, false);
        }

        internal static MI_PropertyDecl NewDirectPtr(MI_Flags flags,
            UInt32 code,
            string name,
            IntPtr qualifiers,
            UInt32 numQualifiers,
            MI_Type type,
            string className,
            UInt32 subscript,
            UInt32 offset,
            string origin,
            string propagator,
            IntPtr value)
        {
            return new MI_PropertyDecl(
                flags,
                code,
                name,
                qualifiers,
                numQualifiers,
                type,
                className,
                subscript,
                offset,
                origin,
                propagator,
                value);
        }
    }
}
