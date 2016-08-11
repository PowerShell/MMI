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
    internal class MI_ClassDecl : MI_NativeObject
    {
        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        private struct MI_ClassDeclMembers
        {
            internal UInt32 flags;
            internal UInt32 code;
            internal string name;
            internal IntPtr qualifiers;
            internal UInt32 numQualifiers;
            internal IntPtr properties;
            internal UInt32 numProperties;
            internal UInt32 size;
            internal string superClass;
            internal DirectPtr superClassDecl;
            internal IntPtr methods;
            internal UInt32 numMethods;
            internal IntPtr schema;
            internal IntPtr providerFT;
            internal IntPtr owningClass;
        }

        // Marshal implements these with Reflection - pay this hit only once
        private static int MI_ClassDeclMembersSize = Marshal.SizeOf<MI_ClassDeclMembers>();

        private MI_ClassDecl(bool isDirect) : base(isDirect)
        {
        }

        private MI_ClassDecl(IntPtr existingPtr) : base(existingPtr)
        {
        }

        internal static MI_ClassDecl NewDirectPtr()
        {
            return new MI_ClassDecl(true);
        }

        internal static MI_ClassDecl NewIndirectPtr()
        {
            return new MI_ClassDecl(false);
        }

        internal static MI_ClassDecl NewFromDirectPtr(IntPtr ptr)
        {
            return new MI_ClassDecl(ptr);
        }

        internal static MI_ClassDecl Null { get { return null; } }

        protected override int MembersSize {  get { return MI_ClassDeclMembersSize; } }
    }
}
