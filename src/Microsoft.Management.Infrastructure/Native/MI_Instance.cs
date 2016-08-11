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
    internal class MI_Instance : MI_NativeObjectWithFT<MI_Instance.MI_InstanceFT>, IDisposable
    {
        internal MI_Result GetElement(
            string name,
            out MI_Value value,
            out MI_Type type,
            out MI_Flags flags,
            out UInt32 index
            )
        {
            MI_Value valueLocal = new MI_Value();
            MI_Result resultLocal = this.ft.GetElement(this,
                name,
                valueLocal,
                out type,
                out flags,
                out index);
            value = valueLocal;
            return resultLocal;
        }

        internal MI_Result GetElementAt(
            UInt32 index,
            out string name,
            out MI_Value value,
            out MI_Type type,
            out MI_Flags flags
            )
        {
            MI_Value valueLocal = new MI_Value();
            MI_String nameLocal = MI_String.NewIndirectPtr();
            MI_Result resultLocal = this.ft.GetElementAt(this,
                index,
                nameLocal,
                valueLocal,
                out type,
                out flags);

            value = valueLocal;
            name = nameLocal.Value;
            return resultLocal;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        private struct MI_InstanceMembers
        {
            internal IntPtr ft;
            internal MI_ClassDecl.DirectPtr classDecl;
            internal string serverName;
            internal string nameSpace;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            internal IntPtr[] reserved;
        }

        // Marshal implements these with Reflection - pay this hit only once
        private static int MI_InstanceMembersFTOffset = (int)Marshal.OffsetOf<MI_InstanceMembers>("ft");
        private static int MI_InstanceMembersSize = Marshal.SizeOf<MI_InstanceMembers>();
        
        private MI_Instance(bool isDirect) : base(isDirect)
        {
        }

        private MI_Instance(IntPtr existingPtr) : base(existingPtr)
        {
        }

        internal static MI_Instance NewDirectPtr()
        {
            return new MI_Instance(true);
        }

        internal static MI_Instance NewIndirectPtr()
        {
            return new MI_Instance(false);
        }

        internal static MI_Instance NewFromDirectPtr(IntPtr ptr)
        {
            return new MI_Instance(ptr);
        }

        internal void AssertValidInternalState()
        {
            throw new NotImplementedException();
        }

        internal static MI_Instance Null { get { return null; } }

        protected override int FunctionTableOffset { get { return MI_InstanceMembersFTOffset; } }

        protected override int MembersSize { get { return MI_InstanceMembersSize; } }

        internal MI_Result Clone(
            out MI_Instance newInstance
            )
        {
            MI_Instance newInstanceLocal = MI_Instance.NewIndirectPtr();

            MI_Result resultLocal = this.ft.Clone(this,
                newInstanceLocal);

            newInstance = newInstanceLocal;
            return resultLocal;
        }

        internal MI_Result Destruct()
        {
            return this.ft.Destruct(this);
        }

        internal MI_Result Delete()
        {
            // Note that we are NOT tolerant of double-delete here
            // This interface is internal, and if a caller messes
            // up the memory pattern we really want to know about it
            MI_Result localResult = this.ft.Delete(this);
            this.ZeroPtr();
            return localResult;
        }

        internal MI_Result IsA(
            MI_ClassDecl classDecl,
            out bool flag
            )
        {
            MI_Result resultLocal = this.ft.IsA(this,
                classDecl,
                out flag);
            return resultLocal;
        }

        internal MI_Result GetClassName(
            out string className
            )
        {
            MI_String classNameLocal = MI_String.NewIndirectPtr();

            MI_Result resultLocal = this.ft.GetClassName(this,
                classNameLocal);

            className = classNameLocal.Value;
            return resultLocal;
        }

        internal MI_Result SetNameSpace(
            string nameSpace
            )
        {
            MI_Result resultLocal = this.ft.SetNameSpace(this,
                nameSpace);
            return resultLocal;
        }

        internal MI_Result GetNameSpace(
            out string nameSpace
            )
        {
            MI_String nameSpaceLocal = MI_String.NewIndirectPtr();

            MI_Result resultLocal = this.ft.GetNameSpace(this,
                nameSpaceLocal);

            nameSpace = nameSpaceLocal.Value;
            return resultLocal;
        }

        internal MI_Result GetElementCount(
            out UInt32 count
            )
        {
            MI_Result resultLocal = this.ft.GetElementCount(this,
                out count);
            return resultLocal;
        }

        internal MI_Result AddElement(
            string name,
            MI_Value value,
            MI_Type type,
            MI_Flags flags
            )
        {
            if (value != null && value.Type.HasValue && value.Type != type)
            {
                throw new InvalidCastException();
            }

            MI_Result resultLocal = this.ft.AddElement(this,
                name,
                value,
                type,
                flags);
            return resultLocal;
        }

        internal MI_Result SetElement(
            string name,
            MI_Value value,
            MI_Type type,
            MI_Flags flags
            )
        {
            if (value != null && value.Type.HasValue && value.Type != type)
            {
                throw new InvalidCastException();
            }

            MI_Result resultLocal = this.ft.SetElement(this,
                name,
                value,
                type,
                flags);
            return resultLocal;
        }

        internal MI_Result SetElementAt(
            UInt32 index,
            MI_Value value,
            MI_Type type,
            MI_Flags flags
            )
        {
            if (value != null && value.Type.HasValue && value.Type != type)
            {
                throw new InvalidCastException();
            }

            MI_Result resultLocal = this.ft.SetElementAt(this,
                index,
                value,
                type,
                flags);
            return resultLocal;
        }

        internal MI_Result ClearElement(
            string name
            )
        {
            MI_Result resultLocal = this.ft.ClearElement(this,
                name);
            return resultLocal;
        }

        internal MI_Result ClearElementAt(
            UInt32 index
            )
        {
            MI_Result resultLocal = this.ft.ClearElementAt(this,
                index);
            return resultLocal;
        }

        internal MI_Result GetServerName(
            out string name
            )
        {
            MI_String nameLocal = MI_String.NewIndirectPtr();

            MI_Result resultLocal = this.ft.GetServerName(this,
                nameLocal);

            name = nameLocal.Value;
            return resultLocal;
        }

        internal MI_Result SetServerName(
            string name
            )
        {
            MI_Result resultLocal = this.ft.SetServerName(this,
                name);
            return resultLocal;
        }

        internal MI_Result GetClass(
            out MI_Class instanceClass
            )
        {
            MI_Class instanceClassLocal = MI_Class.NewIndirectPtr();

            MI_Result resultLocal = this.ft.GetClass(this,
                instanceClassLocal);

            instanceClass = instanceClassLocal;
            return resultLocal;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal class MI_InstanceFT
        {
            internal MI_Instance_Clone Clone;
            internal MI_Instance_Destruct Destruct;
            internal MI_Instance_Delete Delete;
            internal MI_Instance_IsA IsA;
            internal MI_Instance_GetClassName GetClassName;
            internal MI_Instance_SetNameSpace SetNameSpace;
            internal MI_Instance_GetNameSpace GetNameSpace;
            internal MI_Instance_GetElementCount GetElementCount;
            internal MI_Instance_AddElement AddElement;
            internal MI_Instance_SetElement SetElement;
            internal MI_Instance_SetElementAt SetElementAt;
            internal MI_Instance_GetElement GetElement;
            internal MI_Instance_GetElementAt GetElementAt;
            internal MI_Instance_ClearElement ClearElement;
            internal MI_Instance_ClearElementAt ClearElementAt;
            internal MI_Instance_GetServerName GetServerName;
            internal MI_Instance_SetServerName SetServerName;
            internal MI_Instance_GetClass GetClass;

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Instance_Clone(
                DirectPtr self,
                [In, Out] IndirectPtr newInstance
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Instance_Destruct(
                DirectPtr self
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Instance_Delete(
                DirectPtr self
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Instance_IsA(
                DirectPtr self,
                [In, Out] MI_ClassDecl.DirectPtr classDecl,
                [MarshalAs(UnmanagedType.U1)] out bool flag
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Instance_GetClassName(
                DirectPtr self,
                [In, Out] MI_String className
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Instance_SetNameSpace(
                DirectPtr self,
                string nameSpace
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Instance_GetNameSpace(
                DirectPtr self,
                [In, Out] MI_String nameSpace
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Instance_GetElementCount(
                DirectPtr self,
                out UInt32 count
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Instance_AddElement(
                DirectPtr self,
                string name,
                [In, Out] MI_Value.DirectPtr value,
                MI_Type type,
                MI_Flags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Instance_SetElement(
                DirectPtr self,
                string name,
                [In, Out] MI_Value.DirectPtr value,
                MI_Type type,
                MI_Flags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Instance_SetElementAt(
                DirectPtr self,
                UInt32 index,
                [In, Out] MI_Value.DirectPtr value,
                MI_Type type,
                MI_Flags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Instance_GetElement(
                DirectPtr self,
                string name,
                [In, Out] MI_Value.DirectPtr value,
                out MI_Type type,
                out MI_Flags flags,
                out UInt32 index
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Instance_GetElementAt(
                DirectPtr self,
                UInt32 index,
                [In, Out] MI_String name,
                [In, Out] MI_Value.DirectPtr value,
                out MI_Type type,
                out MI_Flags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Instance_ClearElement(
                DirectPtr self,
                string name
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Instance_ClearElementAt(
                DirectPtr self,
                UInt32 index
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Instance_GetServerName(
                DirectPtr self,
                [In, Out] MI_String name
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Instance_SetServerName(
                DirectPtr self,
                string name
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Instance_GetClass(
                DirectPtr self,
                [In, Out] MI_Class.IndirectPtr instanceClass
                );
        }
    }
}
