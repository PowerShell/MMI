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
    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal struct MI_Array
    {
        internal IntPtr data;
        internal UInt32 size;

        public static int MI_ArraySize = (int)Marshal.SizeOf<MI_Array>();
        
        public static T[] ReadAsManagedPointerArray<T>(IntPtr miArrayPtr, Func<IntPtr, T> conversion)
        {
            if (miArrayPtr == IntPtr.Zero)
            {
                throw new ArgumentNullException();
            }

            unsafe
            {
                MI_Array* arrayPtr = (MI_Array*)miArrayPtr;
                if (arrayPtr->data == IntPtr.Zero)
                {
                    return null;
                }

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

                // Reuse of an MI_Array without freeing is unsupported
                if (arrayPtr->data != IntPtr.Zero || arrayPtr->size != 0)
                {
                    throw new InvalidOperationException();
                }

                // No special case for null since previous reuse check forces
                // the entire MI_Array structure to be zeroed, which
                // is what we wanted anyway
                if (ptrs != null)
                {
                    arrayPtr->data = Marshal.AllocHGlobal(NativeMethods.IntPtrSize * ptrs.Length);
                    Marshal.Copy(ptrs, 0, arrayPtr->data, ptrs.Length);
                    arrayPtr->size = (uint)ptrs.Length;
                }
            }
        }

        public static void WriteNativeObjectPointers(IntPtr miArrayPtr, MI_NativeObject[] objects)
        {
            var size = objects.Length;
            IntPtr[] ptrs = new IntPtr[size];
            for (int i = 0; i < size; i++)
            {
                ptrs[i] = objects[i].Ptr;
            }

            WritePointerArray(miArrayPtr, ptrs);
        }
    }
}
