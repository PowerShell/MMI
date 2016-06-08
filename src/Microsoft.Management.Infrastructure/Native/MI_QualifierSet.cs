using System;
using System.Runtime.InteropServices;

namespace Microsoft.Management.Infrastructure.Native
{
    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal class MI_QualifierSet
    {
        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal struct MI_QualifierSetPtr
        {
            internal IntPtr ptr;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal struct MI_QualifierSetOutPtr
        {
            internal IntPtr ptr;
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

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        private struct MI_QualifierSetMembers
        {
            internal UInt64 reserved1;
            internal IntPtr reserved2;
            internal IntPtr ft;
        }

        // Marshal implements these with Reflection - pay this hit only once
        private static int MI_QualifierSetMembersFTOffset = (int)Marshal.OffsetOf<MI_QualifierSetMembers>("ft");

        private static int MI_QualifierSetMembersSize = Marshal.SizeOf<MI_QualifierSetMembers>();

        private MI_QualifierSetPtr ptr;
        private bool isDirect;
        private Lazy<MI_QualifierSetFT> mft;

        ~MI_QualifierSet()
        {
            Marshal.FreeHGlobal(this.ptr.ptr);
        }

        private MI_QualifierSet(bool isDirect)
        {
            this.isDirect = isDirect;
            this.mft = new Lazy<MI_QualifierSetFT>(this.MarshalFT);

            var necessarySize = this.isDirect ? MI_QualifierSetMembersSize : NativeMethods.IntPtrSize;
            this.ptr.ptr = Marshal.AllocHGlobal(necessarySize);

            unsafe
            {
                NativeMethods.memset((byte*)this.ptr.ptr, 0, (uint)necessarySize);
            }
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
            var res = new MI_QualifierSet(false);
            Marshal.WriteIntPtr(res.ptr.ptr, ptr);
            return res;
        }

        public static implicit operator MI_QualifierSetPtr(MI_QualifierSet instance)
        {
            // If the indirect pointer is zero then the object has not
            // been initialized and it is not valid to refer to its data
            if (instance != null && instance.Ptr == IntPtr.Zero)
            {
                throw new InvalidCastException();
            }

            return new MI_QualifierSetPtr() { ptr = instance == null ? IntPtr.Zero : instance.Ptr };
        }

        public static implicit operator MI_QualifierSetOutPtr(MI_QualifierSet instance)
        {
            // We are not currently supporting the ability to get the address
            // of our direct pointer, though it is technically feasible
            if (instance != null && instance.isDirect)
            {
                throw new InvalidCastException();
            }

            return new MI_QualifierSetOutPtr() { ptr = instance == null ? IntPtr.Zero : instance.ptr.ptr };
        }

        internal static MI_QualifierSet Null { get { return null; } }
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

        internal MI_Result GetQualifierCount(
            out UInt32 count
            )
        {
            MI_Result resultLocal = this.ft.GetQualifierCount(this,
                out count);
            return resultLocal;
        }

        private MI_QualifierSetFT ft { get { return this.mft.Value; } }

        private MI_QualifierSetFT MarshalFT()
        {
            return MI_FunctionTableCache.GetFTAsOffsetFromPtr<MI_QualifierSetFT>(this.Ptr, MI_QualifierSet.MI_QualifierSetMembersFTOffset);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal class MI_QualifierSetFT
        {
            internal MI_QualifierSet_GetQualifierCount GetQualifierCount;
            internal MI_QualifierSet_GetQualifierAt GetQualifierAt;
            internal MI_QualifierSet_GetQualifier GetQualifier;

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_QualifierSet_GetQualifierCount(
                MI_QualifierSetPtr self,
                out UInt32 count
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_QualifierSet_GetQualifierAt(
                MI_QualifierSetPtr self,
                UInt32 index,
                [In, Out] MI_String name,
                out MI_Type qualifierType,
                out MI_Flags qualifierFlags,
                [In, Out] MI_Value.MIValueBlock qualifierValue
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_QualifierSet_GetQualifier(
                MI_QualifierSetPtr self,
                string name,
                out MI_Type qualifierType,
                out MI_Flags qualifierFlags,
                [In, Out] MI_Value.MIValueBlock qualifierValue,
                out UInt32 index
                );
        }
    }
}