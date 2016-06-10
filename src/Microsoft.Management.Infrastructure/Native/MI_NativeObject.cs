using System;
using System.Runtime.InteropServices;

namespace Microsoft.Management.Infrastructure.Native
{
    internal abstract class MI_NativeObject
    {
        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        protected struct MI_NativeObjectNormalMembersLayout
        {
            internal UInt64 reserved1;
            internal IntPtr reserved2;
            internal IntPtr ft;
        }

        protected static readonly int MI_NativeObjectNormalMembersLayoutSize = Marshal.SizeOf<MI_NativeObjectNormalMembersLayout>();

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal struct DirectPtr
        {
            internal IntPtr ptr;

            public static implicit operator DirectPtr(MI_NativeObject instance)
            {
                // If the indirect pointer is zero then the object has not
                // been initialized and it is not valid to refer to its data
                if (instance != null && instance.Ptr == IntPtr.Zero)
                {
                    throw new InvalidCastException();
                }

                return new DirectPtr() { ptr = instance == null ? IntPtr.Zero : instance.Ptr };
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal struct IndirectPtr
        {
            internal IntPtr ptr;
            
            public static implicit operator IndirectPtr(MI_NativeObject instance)
            {
                // We are not currently supporting the ability to get the address
                // of our direct pointer, though it is technically feasible
                if (instance != null && instance.isDirect)
                {
                    throw new InvalidCastException();
                }

                return new IndirectPtr() { ptr = instance == null ? IntPtr.Zero : instance.allocatedData };
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal struct ArrayPtr
        {
            internal IntPtr[] ptr;
        }

        protected IntPtr allocatedData;
        protected bool isDirect;

        ~MI_NativeObject()
        {
            if (this.allocatedData != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(this.allocatedData);
            }
        }

        protected MI_NativeObject(bool isDirect)
        {
            this.isDirect = isDirect;
            var necessarySize = this.isDirect ? this.MembersSize : NativeMethods.IntPtrSize;
            this.allocatedData = Marshal.AllocHGlobal(necessarySize);

            unsafe
            {
                NativeMethods.memset((byte*)this.allocatedData, 0, (uint)necessarySize);
            }
        }

        protected MI_NativeObject(IntPtr existingPtr) : this(false)
        {
            Marshal.WriteIntPtr(this.allocatedData, existingPtr);
        }

        protected virtual int MembersSize { get { return MI_NativeObjectNormalMembersLayoutSize; } }

        internal static ArrayPtr GetPointerArray(MI_NativeObject[] objects)
        {
            if (objects == null)
            {
                throw new InvalidCastException();
            }

            IntPtr[] ptrs = new IntPtr[objects.Length];
            for (int i = 0; i < objects.Length; i++)
            {
                ptrs[i] = objects[i].Ptr;
            }

            return new ArrayPtr() { ptr = ptrs };
        }

        internal bool IsNull { get { return this.Ptr == IntPtr.Zero; } }

        internal IntPtr Ptr
        {
            get
            {
                IntPtr structurePtr = this.allocatedData;
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
    }
}