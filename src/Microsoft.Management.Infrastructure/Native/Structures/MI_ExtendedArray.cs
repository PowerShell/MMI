using System;
using System.Runtime.InteropServices;

namespace Microsoft.Management.Infrastructure.Native
{
    internal class MI_ExtendedArray : MI_NativeObjectWithFT<MI_ExtendedArray.MI_ExtendedArrayFT>
    {
        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal struct MI_ExtendedArrayPtr
        {
            internal IntPtr ptr;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal struct MI_ExtendedArrayOutPtr
        {
            internal IntPtr ptr;
        }

        internal struct MI_ExtendedArrayMembers
        {
            internal MI_Array array;
            internal IntPtr reserved1;
            internal IntPtr reserved2;
            internal IntPtr reserved3;
            internal IntPtr reserved4;
        }

        // Marshal implements these with Reflection - pay this hit only once
        private static int MI_ExtendedArrayMembersFTOffset = (int)Marshal.OffsetOf<MI_ExtendedArrayMembers>("reserved2");
        private static int MI_ExtendedArrayMembersSize = Marshal.SizeOf<MI_ExtendedArrayMembers>();
        
        ~MI_ExtendedArray()
        {
            if (this.isDirect)
            {
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.Ptr;
                    if (arrayPtr != null && arrayPtr->data != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(arrayPtr->data);
                        arrayPtr->data = IntPtr.Zero;
                    }
                }
            }

            // Note that we don't free the actual pointer
            // Finalizers run from most derived to least derived,
            // so the base class finalizer will free the memory as required
        }

        private MI_ExtendedArray(bool isDirect) : base(isDirect)
        {
        }

        private MI_ExtendedArray(IntPtr existingPtr) : base(existingPtr)
        {
        }

        internal static MI_ExtendedArray NewDirectPtr()
        {
            return new MI_ExtendedArray(true);
        }

        internal static MI_ExtendedArray NewIndirectPtr()
        {
            return new MI_ExtendedArray(false);
        }

        internal static MI_ExtendedArray NewFromDirectPtr(IntPtr ptr)
        {
            return new MI_ExtendedArray(ptr);
        }

        internal void AssertValidInternalState()
        {
            throw new NotImplementedException();
        }

        public static implicit operator MI_ExtendedArray.MI_ExtendedArrayPtr(MI_ExtendedArray instance)
        {
            // If the indirect pointer is zero then the object has not
            // been initialized and it is not valid to refer to its data
            if (instance != null && instance.Ptr == IntPtr.Zero)
            {
                throw new InvalidCastException();
            }

            return new MI_ExtendedArray.MI_ExtendedArrayPtr() { ptr = instance == null ? IntPtr.Zero : instance.Ptr };
        }

        public static implicit operator MI_ExtendedArrayOutPtr(MI_ExtendedArray instance)
        {
            // We are not currently supporting the ability to get the address
            // of our direct pointer, though it is technically feasible
            if (instance != null && instance.isDirect)
            {
                throw new InvalidCastException();
            }

            return new MI_ExtendedArrayOutPtr() { ptr = instance == null ? IntPtr.Zero : instance.allocatedData };
        }

        public T[] ReadAsManagedPointerArray<T>(Func<IntPtr, T> conversion)
        {
            unsafe
            {
                MI_Array* arrayPtr = (MI_Array*)this.Ptr;
                uint arraySize = arrayPtr->size;
                T[] res = new T[arraySize];
                for (int i = 0; i < arraySize; i++)
                {
                    res[i] = conversion(((IntPtr*)(arrayPtr->data))[i]);
                }

                return res;
            }
        }

        public void WritePointerArray(IntPtr[] ptrs)
        {
            unsafe
            {
                MI_Array* arrayPtr = (MI_Array*)this.Ptr;
                if (arrayPtr->data != IntPtr.Zero)
                {
                    throw new InvalidOperationException();
                }

                arrayPtr->data = Marshal.AllocHGlobal(NativeMethods.IntPtrSize * ptrs.Length);
                Marshal.Copy(ptrs, 0, arrayPtr->data, ptrs.Length);
                arrayPtr->size = (uint)ptrs.Length;
            }
        }

        internal static MI_ExtendedArray Null { get { return null; } }

        protected override int FunctionTableOffset { get { return MI_ExtendedArrayMembersFTOffset; } }

        protected override int MembersSize { get { return MI_ExtendedArrayMembersSize; } }

        internal MI_Result Delete()
        {
            return this.ft.Delete(this);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal class MI_ExtendedArrayFT
        {
            internal MI_ExtendedArray_Delete Delete;

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_ExtendedArray_Delete(
                MI_ExtendedArrayPtr self
                );
        }
    }
}