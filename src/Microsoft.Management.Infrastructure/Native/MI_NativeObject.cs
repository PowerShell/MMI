using System;
using System.Runtime.InteropServices;

namespace Microsoft.Management.Infrastructure.Native
{
    internal abstract class MI_NativeObject
    {
        protected IntPtr allocatedData;
        protected bool isDirect;

        ~MI_NativeObject()
        {
            Marshal.FreeHGlobal(this.allocatedData);
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

        protected abstract int MembersSize { get; }

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