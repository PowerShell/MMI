using System;
using System.Runtime.InteropServices;

namespace Microsoft.Management.Infrastructure.Native
{
    internal class MI_ClassDecl : MI_NativeObject
    {
        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal struct MI_ClassDeclPtr
        {
            internal IntPtr ptr;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal struct MI_ClassDeclOutPtr
        {
            internal IntPtr ptr;
        }

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
            internal MI_ClassDeclPtr superClassDecl;
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

        public static implicit operator MI_ClassDeclPtr(MI_ClassDecl instance)
        {
            // If the indirect pointer is zero then the object has not
            // been initialized and it is not valid to refer to its data
            if (instance != null && instance.Ptr == IntPtr.Zero)
            {
                throw new InvalidCastException();
            }

            return new MI_ClassDeclPtr() { ptr = instance == null ? IntPtr.Zero : instance.Ptr };
        }

        public static implicit operator MI_ClassDeclOutPtr(MI_ClassDecl instance)
        {
            // We are not currently supporting the ability to get the address
            // of our direct pointer, though it is technically feasible
            if (instance != null && instance.isDirect)
            {
                throw new InvalidCastException();
            }

            return new MI_ClassDeclOutPtr() { ptr = instance == null ? IntPtr.Zero : instance.allocatedData };
        }

        internal static MI_ClassDecl Null { get { return null; } }

        protected override int MembersSize {  get { return MI_ClassDeclMembersSize; } }
    }
}