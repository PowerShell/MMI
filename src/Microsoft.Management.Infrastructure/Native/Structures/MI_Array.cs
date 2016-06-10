using System;
using System.Runtime.InteropServices;

namespace Microsoft.Management.Infrastructure.Native
{
    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal struct MI_Array
    {
        internal IntPtr data;
        internal UInt32 size;

        public static int MI_ArraySize = (int)Marshal.SizeOf<MI_Array>();
        
        public static T[] ReadAsManagedPointerArray<T>(IntPtr miArrayPtr, Func<IntPtr, T> conversion)
        {
            unsafe
            {
                MI_Array* arrayPtr = (MI_Array*)miArrayPtr;
                uint arraySize = arrayPtr->size;
                T[] res = new T[arraySize];
                for (int i = 0; i < arraySize; i++)
                {
                    res[i] = conversion(((IntPtr*)(arrayPtr->data))[i]);
                }

                return res;
            }
        }

        public static void WritePointerArray(IntPtr miArrayPtr, IntPtr[] ptrs)
        {
            unsafe
            {
                MI_Array* arrayPtr = (MI_Array*)miArrayPtr;
                if (arrayPtr->data != IntPtr.Zero)
                {
                    throw new InvalidOperationException();
                }

                arrayPtr->data = Marshal.AllocHGlobal(NativeMethods.IntPtrSize * ptrs.Length);
                Marshal.Copy(ptrs, 0, arrayPtr->data, ptrs.Length);
                arrayPtr->size = (uint)ptrs.Length;
            }
        }
    }
}