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
    internal class MI_ParameterSet : MI_NativeObjectWithFT<MI_ParameterSet.MI_ParameterSetFT>
    {

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        private struct MI_ParameterSetMembers
        {
            internal UInt64 reserved1;
            internal IntPtr reserved2;
            internal IntPtr ft;
        }

        static MI_ParameterSet()
        {
            CheckMembersTableMatchesNormalLayout<MI_ParameterSetMembers>("ft");
        }

        internal MI_Result GetParameterAt(
            UInt32 index,
            out string name,
            out MI_Type parameterType,
            out string referenceClass,
            out MI_QualifierSet qualifierSet
            )
        {
            MI_String nameLocal = MI_String.NewIndirectPtr();
            MI_String referenceClassLocal = MI_String.NewIndirectPtr();
            MI_QualifierSet qualifierSetLocal = MI_QualifierSet.NewDirectPtr();
            MI_Result resultLocal = this.ft.GetParameterAt(this,
                index,
                nameLocal,
                out parameterType,
                referenceClassLocal,
                qualifierSetLocal);

            name = nameLocal.Value;
            referenceClass = referenceClassLocal.Value;
            qualifierSet = qualifierSetLocal;
            return resultLocal;
        }

        internal MI_Result GetParameter(
            string name,
            out MI_Type parameterType,
            out string referenceClass,
            out MI_QualifierSet qualifierSet,
            out UInt32 index
            )
        {
            MI_String referenceClassLocal = MI_String.NewIndirectPtr();
            MI_QualifierSet qualifierSetLocal = MI_QualifierSet.NewDirectPtr();

            MI_Result resultLocal = this.ft.GetParameter(this,
                name,
                out parameterType,
                referenceClassLocal,
                qualifierSetLocal,
                out index);

            referenceClass = referenceClassLocal.Value;
            qualifierSet = qualifierSetLocal;
            return resultLocal;
        }
        
        private MI_ParameterSet(bool isDirect) : base(isDirect)
        {
        }

        private MI_ParameterSet(IntPtr existingPtr) : base(existingPtr)
        {
        }

        internal static MI_ParameterSet NewDirectPtr()
        {
            return new MI_ParameterSet(true);
        }

        internal static MI_ParameterSet NewIndirectPtr()
        {
            return new MI_ParameterSet(false);
        }

        internal static MI_ParameterSet NewFromDirectPtr(IntPtr ptr)
        {
            return new MI_ParameterSet(ptr);
        }
        
        internal static MI_ParameterSet Null { get { return null; } }

        internal MI_Result GetMethodReturnType(
            out MI_Type returnType,
            MI_QualifierSet qualifierSet
            )
        {
            MI_Result resultLocal = this.ft.GetMethodReturnType(this,
                out returnType,
                qualifierSet);
            return resultLocal;
        }

        internal MI_Result GetParameterCount(
            out UInt32 count
            )
        {
            MI_Result resultLocal = this.ft.GetParameterCount(this,
                out count);
            return resultLocal;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal class MI_ParameterSetFT
        {
            internal MI_ParameterSet_GetMethodReturnType GetMethodReturnType;
            internal MI_ParameterSet_GetParameterCount GetParameterCount;
            internal MI_ParameterSet_GetParameterAt GetParameterAt;
            internal MI_ParameterSet_GetParameter GetParameter;

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_ParameterSet_GetMethodReturnType(
                DirectPtr self,
                out MI_Type returnType,
                [In, Out] MI_QualifierSet.DirectPtr qualifierSet
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_ParameterSet_GetParameterCount(
                DirectPtr self,
                out UInt32 count
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_ParameterSet_GetParameterAt(
                DirectPtr self,
                UInt32 index,
                [In, Out] MI_String name,
                out MI_Type parameterType,
                [In, Out] MI_String referenceClass,
                [In, Out] MI_QualifierSet.DirectPtr qualifierSet
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_ParameterSet_GetParameter(
                DirectPtr self,
                string name,
                out MI_Type parameterType,
                [In, Out] MI_String referenceClass,
                [In, Out] MI_QualifierSet.DirectPtr qualifierSet,
                out UInt32 index
                );
        }
    }
}
