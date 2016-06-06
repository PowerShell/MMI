﻿using System;
using System.Runtime.InteropServices;

namespace Microsoft.Management.Infrastructure.Native
{
    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal class MI_ParameterSet
    {
        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal struct MI_ParameterSetPtr
        {
            internal IntPtr ptr;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal struct MI_ParameterSetOutPtr
        {
            internal IntPtr ptr;
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

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        private struct MI_ParameterSetMembers
        {
            internal UInt64 reserved1;
            internal IntPtr reserved2;
            internal IntPtr ft;
        }

        // Marshal implements these with Reflection - pay this hit only once
        private static int MI_ParameterSetMembersFTOffset = (int)Marshal.OffsetOf<MI_ParameterSetMembers>("ft");

        private static int MI_ParameterSetMembersSize = Marshal.SizeOf<MI_ParameterSetMembers>();

        private MI_ParameterSetPtr ptr;
        private bool isDirect;
        private Lazy<MI_ParameterSetFT> mft;

        ~MI_ParameterSet()
        {
            Marshal.FreeHGlobal(this.ptr.ptr);
        }

        private MI_ParameterSet(bool isDirect)
        {
            this.isDirect = isDirect;
            this.mft = new Lazy<MI_ParameterSetFT>(this.MarshalFT);

            var necessarySize = this.isDirect ? MI_ParameterSetMembersSize : NativeMethods.IntPtrSize;
            this.ptr.ptr = Marshal.AllocHGlobal(necessarySize);

            unsafe
            {
                NativeMethods.memset((byte*)this.ptr.ptr, 0, (uint)necessarySize);
            }
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
            var res = new MI_ParameterSet(false);
            Marshal.WriteIntPtr(res.ptr.ptr, ptr);
            return res;
        }

        public static implicit operator MI_ParameterSetPtr(MI_ParameterSet instance)
        {
            // If the indirect pointer is zero then the object has not
            // been initialized and it is not valid to refer to its data
            if (instance != null && instance.Ptr == IntPtr.Zero)
            {
                throw new InvalidCastException();
            }

            return new MI_ParameterSetPtr() { ptr = instance == null ? IntPtr.Zero : instance.Ptr };
        }

        public static implicit operator MI_ParameterSetOutPtr(MI_ParameterSet instance)
        {
            // We are not currently supporting the ability to get the address
            // of our direct pointer, though it is technically feasible
            if (instance != null && instance.isDirect)
            {
                throw new InvalidCastException();
            }

            return new MI_ParameterSetOutPtr() { ptr = instance == null ? IntPtr.Zero : instance.ptr.ptr };
        }

        public static explicit operator MI_Instance.MI_InstanceOutPtr(MI_ParameterSet instance)
        {
            // We are not currently supporting the ability to get the address
            // of our direct pointer, though it is technically feasible
            if (instance != null && instance.isDirect)
            {
                throw new InvalidCastException();
            }

            return new MI_Instance.MI_InstanceOutPtr() { ptr = instance == null ? IntPtr.Zero : instance.ptr.ptr };
        }

        internal static MI_ParameterSet Null { get { return null; } }
        internal bool IsNull { get { return this.Ptr == IntPtr.Zero; } }

        internal IntPtr Ptr
        {
            get
            {
                IntPtr structurePtr = this.ptr.ptr;
                if (!this.isDirect)
                {
                    if (structurePtr == IntPtr.Zero)
                    {
                        throw new InvalidOperationException();
                    }

                    // This can be easily implemented with Marshal.ReadIntPtr
                    // but that has function call overhead
                    unsafe
                    {
                        structurePtr = *(IntPtr*)structurePtr;
                    }
                }

                return structurePtr;
            }
        }

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

        private MI_ParameterSetFT ft { get { return this.mft.Value; } }

        private MI_ParameterSetFT MarshalFT()
        {
            return NativeMethods.GetFTAsOffsetFromPtr<MI_ParameterSetFT>(this.Ptr, MI_ParameterSet.MI_ParameterSetMembersFTOffset);
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
                MI_ParameterSetPtr self,
                out MI_Type returnType,
                [In, Out] MI_QualifierSet.MI_QualifierSetPtr qualifierSet
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_ParameterSet_GetParameterCount(
                MI_ParameterSetPtr self,
                out UInt32 count
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_ParameterSet_GetParameterAt(
                MI_ParameterSetPtr self,
                UInt32 index,
                [In, Out] MI_String name,
                out MI_Type parameterType,
                [In, Out] MI_String referenceClass,
                [In, Out] MI_QualifierSet.MI_QualifierSetPtr qualifierSet
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_ParameterSet_GetParameter(
                MI_ParameterSetPtr self,
                string name,
                out MI_Type parameterType,
                [In, Out] MI_String referenceClass,
                [In, Out] MI_QualifierSet.MI_QualifierSetPtr qualifierSet,
                out UInt32 index
                );
        }
    }
}