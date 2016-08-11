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
    internal class MI_Class : MI_NativeObjectWithFT<MI_Class.MI_ClassFT>
    {
        internal MI_Result GetElement(
            string name,
            out MI_Value value,
            out bool valueExists,
            out MI_Type type,
            out string referenceClass,
            out MI_QualifierSet qualifierSet,
            out MI_Flags flags,
            out UInt32 index
            )
        {
            MI_String referenceClassLocal = MI_String.NewIndirectPtr();
            MI_Value valueLocal = new MI_Value();
            MI_QualifierSet qualifierSetLocal = MI_QualifierSet.NewDirectPtr();

            MI_Result resultLocal = this.ft.GetElement(this,
                name,
                valueLocal,
                out valueExists,
                out type,
                referenceClassLocal,
                qualifierSetLocal,
                out flags,
                out index);

            referenceClass = referenceClassLocal.Value;
            value = valueLocal;
            qualifierSet = qualifierSetLocal;
            return resultLocal;
        }

        internal MI_Result GetElementAt(
            UInt32 index,
            out string name,
            out MI_Value value,
            out bool valueExists,
            out MI_Type type,
            out string referenceClass,
            out MI_QualifierSet qualifierSet,
            out MI_Flags flags
            )
        {
            MI_String nameLocal = MI_String.NewIndirectPtr();
            MI_String referenceClassLocal = MI_String.NewIndirectPtr();
            MI_Value valueLocal = new MI_Value();
            MI_QualifierSet qualifierSetLocal = MI_QualifierSet.NewDirectPtr();

            MI_Result resultLocal = this.ft.GetElementAt(this,
                index,
                nameLocal,
                valueLocal,
                out valueExists,
                out type,
                referenceClassLocal,
                qualifierSetLocal,
                out flags);

            name = nameLocal.Value;
            referenceClass = referenceClassLocal.Value;
            value = valueLocal;
            qualifierSet = qualifierSetLocal;
            return resultLocal;
        }

        internal MI_Result GetClassQualifierSet(
            out MI_QualifierSet qualifierSet
            )
        {
            MI_QualifierSet qualifierSetLocal = MI_QualifierSet.NewDirectPtr();
            MI_Result resultLocal = this.ft.GetClassQualifierSet(this,
                qualifierSetLocal);

            qualifierSet = qualifierSetLocal;
            return resultLocal;
        }

        internal MI_Result GetMethod(
            string name,
            out MI_QualifierSet qualifierSet,
            out MI_ParameterSet parameterSet,
            out UInt32 index
            )
        {
            MI_QualifierSet qualifierSetLocal = MI_QualifierSet.NewDirectPtr();
            MI_ParameterSet parameterSetLocal = MI_ParameterSet.NewDirectPtr();
            MI_Result resultLocal = this.ft.GetMethod(this,
                name,
                qualifierSetLocal,
                parameterSetLocal,
                out index);

            qualifierSet = qualifierSetLocal;
            parameterSet = parameterSetLocal;
            return resultLocal;
        }

        internal MI_Result GetMethodAt(
            UInt32 index,
            out string name,
            out MI_QualifierSet qualifierSet,
            out MI_ParameterSet parameterSet
            )
        {
            MI_String nameLocal = MI_String.NewIndirectPtr();
            MI_QualifierSet qualifierSetLocal = MI_QualifierSet.NewDirectPtr();
            MI_ParameterSet parameterSetLocal = MI_ParameterSet.NewDirectPtr();

            MI_Result resultLocal = this.ft.GetMethodAt(this,
                index,
                nameLocal,
                qualifierSetLocal,
                parameterSetLocal);

            name = nameLocal.Value;
            qualifierSet = qualifierSetLocal;
            parameterSet = parameterSetLocal;
            return resultLocal;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        private struct MI_ClassMembers
        {
            internal IntPtr ft;
            internal MI_ClassDecl.DirectPtr classDecl;
            internal string namespaceName;
            internal string serverName;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            internal IntPtr[] reserved;
        }

        // Marshal implements these with Reflection - pay this hit only once
        private static int MI_ClassMembersFTOffset = (int)Marshal.OffsetOf<MI_ClassMembers>("ft");
        private static int MI_ClassMembersSize = Marshal.SizeOf<MI_ClassMembers>();
        
        private MI_Class(bool isDirect) : base(isDirect)
        {
        }

        private MI_Class(IntPtr existingPtr) : base(existingPtr)
        {
        }

        internal static MI_Class NewDirectPtr()
        {
            return new MI_Class(true);
        }

        internal static MI_Class NewIndirectPtr()
        {
            return new MI_Class(false);
        }

        internal static MI_Class NewFromDirectPtr(IntPtr ptr)
        {
            return new MI_Class(ptr);
        }
        
        internal static MI_Class Null { get { return null; } }

        protected override int FunctionTableOffset { get { return MI_ClassMembersFTOffset; } }

        protected override int MembersSize { get { return MI_ClassMembersSize; } }

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

        internal MI_Result GetServerName(
            out string serverName
            )
        {
            MI_String serverNameLocal = MI_String.NewIndirectPtr();

            MI_Result resultLocal = this.ft.GetServerName(this,
                serverNameLocal);

            serverName = serverNameLocal.Value;
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

        internal MI_Result GetMethodCount(
            out UInt32 count
            )
        {
            MI_Result resultLocal = this.ft.GetMethodCount(this,
                out count);
            return resultLocal;
        }

        internal MI_Result GetParentClassName(
            out string name
            )
        {
            MI_String nameLocal = MI_String.NewIndirectPtr();

            MI_Result resultLocal = this.ft.GetParentClassName(this,
                nameLocal);

            name = nameLocal.Value;
            return resultLocal;
        }

        internal MI_Result GetParentClass(
            out MI_Class parentClass
            )
        {
            MI_Class parentClassLocal = MI_Class.NewIndirectPtr();

            MI_Result resultLocal = this.ft.GetParentClass(this,
                parentClassLocal);

            parentClass = parentClassLocal;
            return resultLocal;
        }

        internal MI_Result Delete()
        {
            return this.ft.Delete(this);
        }

        internal MI_Result Clone(
            out MI_Class newClass
            )
        {
            MI_Class newClassLocal = MI_Class.NewIndirectPtr();

            MI_Result resultLocal = this.ft.Clone(this,
                newClassLocal);

            newClass = newClassLocal;
            return resultLocal;
        }
        
        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal class MI_ClassFT
        {
            internal MI_Class_GetClassName GetClassName;
            internal MI_Class_GetNameSpace GetNameSpace;
            internal MI_Class_GetServerName GetServerName;
            internal MI_Class_GetElementCount GetElementCount;
            internal MI_Class_GetElement GetElement;
            internal MI_Class_GetElementAt GetElementAt;
            internal MI_Class_GetClassQualifierSet GetClassQualifierSet;
            internal MI_Class_GetMethodCount GetMethodCount;
            internal MI_Class_GetMethodAt GetMethodAt;
            internal MI_Class_GetMethod GetMethod;
            internal MI_Class_GetParentClassName GetParentClassName;
            internal MI_Class_GetParentClass GetParentClass;
            internal MI_Class_Delete Delete;
            internal MI_Class_Clone Clone;

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_GetClassName(
                DirectPtr self,
                [In, Out] MI_String className
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_GetNameSpace(
                DirectPtr self,
                [In, Out] MI_String nameSpace
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_GetServerName(
                DirectPtr self,
                [In, Out] MI_String serverName
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_GetElementCount(
                DirectPtr self,
                out UInt32 count
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_GetElement(
                DirectPtr self,
                string name,
                [In, Out] MI_Value.DirectPtr value,
                [MarshalAs(UnmanagedType.U1)] out bool valueExists,
                out MI_Type type,
                [In, Out] MI_String referenceClass,
                [In, Out] MI_QualifierSet.DirectPtr qualifierSet,
                out MI_Flags flags,
                out UInt32 index
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_GetElementAt(
                DirectPtr self,
                UInt32 index,
                [In, Out] MI_String name,
                [In, Out] MI_Value.DirectPtr value,
                [MarshalAs(UnmanagedType.U1)] out bool valueExists,
                out MI_Type type,
                [In, Out] MI_String referenceClass,
                [In, Out] MI_QualifierSet.DirectPtr qualifierSet,
                out MI_Flags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_GetClassQualifierSet(
                DirectPtr self,
                [In, Out] MI_QualifierSet.DirectPtr qualifierSet
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_GetMethodCount(
                DirectPtr self,
                out UInt32 count
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_GetMethodAt(
                DirectPtr self,
                UInt32 index,
                [In, Out] MI_String name,
                [In, Out] MI_QualifierSet.DirectPtr qualifierSet,
                [In, Out] MI_ParameterSet.DirectPtr parameterSet
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_GetMethod(
                DirectPtr self,
                string name,
                [In, Out] MI_QualifierSet.DirectPtr qualifierSet,
                [In, Out] MI_ParameterSet.DirectPtr parameterSet,
                out UInt32 index
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_GetParentClassName(
                DirectPtr self,
                [In, Out] MI_String name
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_GetParentClass(
                DirectPtr self,
                [In, Out] IndirectPtr parentClass
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_Delete(
                DirectPtr self
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_Clone(
                DirectPtr self,
                [In, Out] IndirectPtr newClass
                );
        }
    }
}
