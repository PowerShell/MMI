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
    internal class MI_QualifierSet : MI_NativeObjectWithFT<MI_QualifierSet.MI_QualifierSetFT>
    {
        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        private struct MI_QualifierSetMembers
        {
            internal UInt64 reserved1;
            internal IntPtr reserved2;
            internal IntPtr ft;
        }

        static MI_QualifierSet()
        {
            CheckMembersTableMatchesNormalLayout<MI_QualifierSetMembers>("ft");
        }

        internal MI_Result GetQualifier(
            string name,
            out MI_Type qualifierType,
            out MI_Flags qualifierFlags,
            out MI_Value qualifierValue,
            out UInt32 index
            )
        {
            MI_Value qualifierValueLocal = new MI_Value();
            MI_Result resultLocal = this.ft.GetQualifier(this,
                name,
                out qualifierType,
                out qualifierFlags,
                qualifierValueLocal,
                out index);

            qualifierValue = qualifierValueLocal;
            return resultLocal;
        }

        internal MI_Result GetQualifierAt(
            UInt32 index,
            out string name,
            out MI_Type qualifierType,
            out MI_Flags qualifierFlags,
            out MI_Value qualifierValue
            )
        {
            MI_String nameLocal = MI_String.NewIndirectPtr();
            MI_Value qualifierValueLocal = new MI_Value();

            MI_Result resultLocal = this.ft.GetQualifierAt(this,
                index,
                nameLocal,
                out qualifierType,
                out qualifierFlags,
                qualifierValueLocal);

            name = nameLocal.Value;
            qualifierValue = qualifierValueLocal;
            return resultLocal;
        }
        
        private MI_QualifierSet(bool isDirect) : base(isDirect)
        {
        }

        private MI_QualifierSet(IntPtr existingPtr) : base(existingPtr)
        {
        }

        internal static MI_QualifierSet NewDirectPtr()
        {
            return new MI_QualifierSet(true);
        }

        internal static MI_QualifierSet NewIndirectPtr()
        {
            return new MI_QualifierSet(false);
        }

        internal static MI_QualifierSet NewFromDirectPtr(IntPtr ptr)
        {
            return new MI_QualifierSet(ptr);
        }
        
        internal static MI_QualifierSet Null { get { return null; } }

        internal MI_Result GetQualifierCount(
            out UInt32 count
            )
        {
            MI_Result resultLocal = this.ft.GetQualifierCount(this,
                out count);
            return resultLocal;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal class MI_QualifierSetFT
        {
            internal MI_QualifierSet_GetQualifierCount GetQualifierCount;
            internal MI_QualifierSet_GetQualifierAt GetQualifierAt;
            internal MI_QualifierSet_GetQualifier GetQualifier;

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_QualifierSet_GetQualifierCount(
                DirectPtr self,
                out UInt32 count
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_QualifierSet_GetQualifierAt(
                DirectPtr self,
                UInt32 index,
                [In, Out] MI_String name,
                out MI_Type qualifierType,
                out MI_Flags qualifierFlags,
                [In, Out] MI_Value.DirectPtr qualifierValue
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_QualifierSet_GetQualifier(
                DirectPtr self,
                string name,
                out MI_Type qualifierType,
                out MI_Flags qualifierFlags,
                [In, Out] MI_Value.DirectPtr qualifierValue,
                out UInt32 index
                );
        }
    }
}
