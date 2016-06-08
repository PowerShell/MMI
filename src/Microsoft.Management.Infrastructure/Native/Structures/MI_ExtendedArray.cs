using System;
using System.Runtime.InteropServices;

namespace Microsoft.Management.Infrastructure.Native
{
    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal class MI_ExtendedArray
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

        private MI_ExtendedArray.MI_ExtendedArrayPtr ptr;
        private bool isDirect;
        private Lazy<MI_ExtendedArrayFT> mft;

        ~MI_ExtendedArray()
        {
            if (this.isDirect)
            {
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.Ptr;
                    if (arrayPtr->data != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(arrayPtr->data);
                    }
                }
            }

            Marshal.FreeHGlobal(this.ptr.ptr);
        }

        private MI_ExtendedArray(bool isDirect)
        {
            this.isDirect = isDirect;
            this.mft = new Lazy<MI_ExtendedArrayFT>(this.MarshalFT);

            var necessarySize = this.isDirect ? MI_ExtendedArrayMembersSize : NativeMethods.IntPtrSize;
            this.ptr.ptr = Marshal.AllocHGlobal(necessarySize);

            unsafe
            {
                NativeMethods.memset((byte*)this.ptr.ptr, 0, (uint)necessarySize);
            }
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
            var res = new MI_ExtendedArray(false);
            Marshal.WriteIntPtr(res.ptr.ptr, ptr);
            return res;
        }

        private MI_ExtendedArrayFT ft { get { return this.mft.Value; } }

        private MI_ExtendedArrayFT MarshalFT()
        {
            return MI_FunctionTableCache.GetFTAsOffsetFromPtr<MI_ExtendedArrayFT>(this.Ptr, MI_ExtendedArray.MI_ExtendedArrayMembersFTOffset);
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

            return new MI_ExtendedArrayOutPtr() { ptr = instance == null ? IntPtr.Zero : instance.ptr.ptr };
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