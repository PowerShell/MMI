using System;

namespace NativeObject
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public class MI_String
    {
        private IntPtr ptr;

        private MI_String()
        {
        }

        public MI_String(IntPtr ptr)
        {
            this.ptr = ptr;
        }

        public static implicit operator IntPtr(MI_String wrapper)
        {
            return wrapper.ptr;
        }

        public static MI_String NewIndirectPtr()
        {
            return new MI_String();
        }

        public string Value
        {
            get
            {
                return this.ptr == IntPtr.Zero ? null : MI_PlatformSpecific.PtrToString(this.ptr);
            }
        }
    }
}