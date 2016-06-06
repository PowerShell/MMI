﻿using System;

namespace Microsoft.Management.Infrastructure.Native
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal class MI_Value
    {
        private static readonly int ByteSize = Marshal.SizeOf<byte>();

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal struct MIValueBlock
        {
            internal IntPtr data;
        }

        [StructLayout(LayoutKind.Explicit, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        private class MI_ValueLayout
        {
            [FieldOffset(0)]
            [MarshalAs(UnmanagedType.U1)]
            private bool boolean;

            [FieldOffset(0)]
            private MI_Datetime datetime;

            [FieldOffset(0)]
            private IntPtr stringPtr;

            [FieldOffset(0)]
            private IntPtr instance;

            [FieldOffset(0)]
            private IntPtr reference;

            [FieldOffset(0)]
            private MI_Array array;

            [FieldOffset(0)]
            private byte uint8;

            [FieldOffset(0)]
            private sbyte sint8;

            [FieldOffset(0)]
            private UInt16 uint16;

            [FieldOffset(0)]
            private Int16 sint16;

            [FieldOffset(0)]
            private UInt32 uint32;

            [FieldOffset(0)]
            private Int32 sint32;

            [FieldOffset(0)]
            private UInt64 uint64;

            [FieldOffset(0)]
            private Int64 sint64;

            [FieldOffset(0)]
            private float real32;

            [FieldOffset(0)]
            private double real64;

            [FieldOffset(0)]
            private char char16;
        }

        private IntPtr data;
        private MI_Type? type;

        public static implicit operator MIValueBlock(MI_Value value)
        {
            return new MIValueBlock { data = value == null ? IntPtr.Zero : value.data };
        }

        internal MI_Value()
        {
            this.data = Marshal.AllocHGlobal(MI_ValueSize);
        }

        ~MI_Value()
        {
            this.Dispose(false);
        }

        internal void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal void DisownMemory()
        {
            this.type = null;
        }

        private void Dispose(bool disposing)
        {
            // Only wipe memory if this wrapper allocated it
            if (this.type.HasValue)
            {
                this.Free();
                Marshal.FreeHGlobal(this.data);
                this.data = IntPtr.Zero;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        private struct MI_Array
        {
            internal IntPtr data;
            internal UInt32 size;
        }

        internal string String
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_STRING)
                {
                    throw new InvalidCastException();
                }

                IntPtr inner = IntPtr.Zero;
                unsafe
                {
                    inner = *((IntPtr*)this.data);
                }

                return inner == IntPtr.Zero ? null : MI_PlatformSpecific.PtrToString(inner);
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_STRING;
                IntPtr inner = MI_PlatformSpecific.StringToPtr(value);
                Marshal.WriteIntPtr(this.data, inner);
            }
        }

        internal bool Boolean
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_BOOLEAN)
                {
                    throw new InvalidCastException();
                }

                byte nativeBool = Marshal.ReadByte(this.data);
                return nativeBool != 0;
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_BOOLEAN;
                Marshal.WriteByte(this.data, (byte)(value ? 1 : 0));
            }
        }

        internal MI_Instance Instance
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_INSTANCE)
                {
                    throw new InvalidCastException();
                }

                return MI_Instance.NewFromDirectPtr(Marshal.ReadIntPtr(this.data));
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_INSTANCE;
                Marshal.WriteIntPtr(this.data, value.Ptr);
            }
        }

        internal MI_Instance Reference
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_REFERENCE)
                {
                    throw new InvalidCastException();
                }

                return MI_Instance.NewFromDirectPtr(Marshal.ReadIntPtr(this.data));
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_REFERENCE;
                Marshal.WriteIntPtr(this.data, value.Ptr);
            }
        }

        internal string[] StringA
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_STRINGA)
                {
                    throw new InvalidCastException();
                }

                MI_Array array = (MI_Array)Marshal.PtrToStructure<MI_Array>(this.data);
                var size = array.size;
                IntPtr[] ptrs = new IntPtr[size];
                Marshal.Copy(array.data, ptrs, 0, (int)size);
                string[] res = new string[size];
                for (int i = 0; i < size; i++)
                {
                    res[i] = new MI_String(ptrs[i]).Value;
                }

                return res;
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_STRINGA;

                var size = value.Length;
                IntPtr[] ptrs = new IntPtr[size];
                for (int i = 0; i < size; i++)
                {
                    ptrs[i] = MI_PlatformSpecific.StringToPtr(value[i]);
                }

                MI_Array array = new MI_Array();
                array.data = Marshal.AllocHGlobal(NativeMethods.IntPtrSize * size);
                array.size = (uint)size;
                Marshal.Copy(ptrs, 0, array.data, (int)size);
                Marshal.StructureToPtr(array, this.data, false);
            }
        }

        internal bool[] BooleanA
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_BOOLEANA)
                {
                    throw new InvalidCastException();
                }

                MI_Array array = (MI_Array)Marshal.PtrToStructure<MI_Array>(this.data);
                var size = array.size;
                byte[] bytes = new byte[size];
                Marshal.Copy(array.data, bytes, 0, (int)size);
                bool[] res = new bool[size];
                for (int i = 0; i < size; i++)
                {
                    res[i] = bytes[i] != 0;
                }

                return res;
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_BOOLEANA;

                var size = value.Length;
                byte[] bytes = new byte[size];
                for (int i = 0; i < size; i++)
                {
                    bytes[i] = (byte)(value[i] ? 1 : 0);
                }

                MI_Array array = new MI_Array();
                array.data = Marshal.AllocHGlobal(ByteSize * size);
                array.size = (uint)size;
                Marshal.Copy(bytes, 0, array.data, (int)size);
                Marshal.StructureToPtr(array, this.data, false);
            }
        }

        internal MI_Instance[] InstanceA
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_INSTANCEA)
                {
                    throw new InvalidCastException();
                }

                MI_Array array = (MI_Array)Marshal.PtrToStructure<MI_Array>(this.data);
                var size = array.size;
                IntPtr[] ptrs = new IntPtr[size];
                Marshal.Copy(array.data, ptrs, 0, (int)size);
                MI_Instance[] res = new MI_Instance[size];
                for (int i = 0; i < size; i++)
                {
                    res[i] = MI_Instance.NewFromDirectPtr(ptrs[i]);
                }

                return res;
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_INSTANCEA;

                var size = value.Length;
                IntPtr[] ptrs = new IntPtr[size];
                for (int i = 0; i < size; i++)
                {
                    ptrs[i] = value[i].Ptr;
                }

                MI_Array array = new MI_Array();
                array.data = Marshal.AllocHGlobal(NativeMethods.IntPtrSize * size);
                array.size = (uint)size;
                Marshal.Copy(ptrs, 0, array.data, (int)size);
                Marshal.StructureToPtr(array, this.data, false);
            }
        }

        internal MI_Instance[] ReferenceA
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_REFERENCEA)
                {
                    throw new InvalidCastException();
                }

                MI_Array array = (MI_Array)Marshal.PtrToStructure<MI_Array>(this.data);
                var size = array.size;
                IntPtr[] ptrs = new IntPtr[size];
                Marshal.Copy(array.data, ptrs, 0, (int)size);
                MI_Instance[] res = new MI_Instance[size];
                for (int i = 0; i < size; i++)
                {
                    res[i] = MI_Instance.NewFromDirectPtr(ptrs[i]);
                }

                return res;
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_REFERENCEA;

                var size = value.Length;
                IntPtr[] ptrs = new IntPtr[size];
                for (int i = 0; i < size; i++)
                {
                    ptrs[i] = value[i].Ptr;
                }

                MI_Array array = new MI_Array();
                array.data = Marshal.AllocHGlobal(NativeMethods.IntPtrSize * size);
                array.size = (uint)size;
                Marshal.Copy(ptrs, 0, array.data, (int)size);
                Marshal.StructureToPtr(array, this.data, false);
            }
        }

        internal static int MI_ValueSize = Marshal.SizeOf<MI_ValueLayout>();

        internal static MI_Value NewDirectPtr()
        {
            return new MI_Value();
        }

        internal static MI_Value Null { get { return null; } }

        internal MI_Type? Type { get { return this.type; } }

        private void Free()
        {
            if (this.type.HasValue && (this.type.Value & MI_TypeFlags.MI_ARRAY) != 0)
            {
                unsafe
                {
                    Marshal.FreeHGlobal(((MI_Array*)this.data)->data);
                }

                this.type = null;
            }
        }

        internal byte Uint8
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_UINT8)
                {
                    throw new InvalidCastException();
                }

                byte res;
                unsafe
                {
                    res = *(byte*)this.data;
                }

                return res;
            }

            set
            {
                this.Free();
                this.type = MI_Type.MI_UINT8;
                unsafe
                {
                    *(byte*)this.data = value;
                }
            }
        }

        internal sbyte Sint8
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_SINT8)
                {
                    throw new InvalidCastException();
                }

                sbyte res;
                unsafe
                {
                    res = *(sbyte*)this.data;
                }

                return res;
            }

            set
            {
                this.Free();
                this.type = MI_Type.MI_SINT8;
                unsafe
                {
                    *(sbyte*)this.data = value;
                }
            }
        }

        internal UInt16 Uint16
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_UINT16)
                {
                    throw new InvalidCastException();
                }

                UInt16 res;
                unsafe
                {
                    res = *(UInt16*)this.data;
                }

                return res;
            }

            set
            {
                this.Free();
                this.type = MI_Type.MI_UINT16;
                unsafe
                {
                    *(UInt16*)this.data = value;
                }
            }
        }

        internal Int16 Sint16
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_SINT16)
                {
                    throw new InvalidCastException();
                }

                Int16 res;
                unsafe
                {
                    res = *(Int16*)this.data;
                }

                return res;
            }

            set
            {
                this.Free();
                this.type = MI_Type.MI_SINT16;
                unsafe
                {
                    *(Int16*)this.data = value;
                }
            }
        }

        internal UInt32 Uint32
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_UINT32)
                {
                    throw new InvalidCastException();
                }

                UInt32 res;
                unsafe
                {
                    res = *(UInt32*)this.data;
                }

                return res;
            }

            set
            {
                this.Free();
                this.type = MI_Type.MI_UINT32;
                unsafe
                {
                    *(UInt32*)this.data = value;
                }
            }
        }

        internal Int32 Sint32
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_SINT32)
                {
                    throw new InvalidCastException();
                }

                Int32 res;
                unsafe
                {
                    res = *(Int32*)this.data;
                }

                return res;
            }

            set
            {
                this.Free();
                this.type = MI_Type.MI_SINT32;
                unsafe
                {
                    *(Int32*)this.data = value;
                }
            }
        }

        internal UInt64 Uint64
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_UINT64)
                {
                    throw new InvalidCastException();
                }

                UInt64 res;
                unsafe
                {
                    res = *(UInt64*)this.data;
                }

                return res;
            }

            set
            {
                this.Free();
                this.type = MI_Type.MI_UINT64;
                unsafe
                {
                    *(UInt64*)this.data = value;
                }
            }
        }

        internal Int64 Sint64
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_SINT64)
                {
                    throw new InvalidCastException();
                }

                Int64 res;
                unsafe
                {
                    res = *(Int64*)this.data;
                }

                return res;
            }

            set
            {
                this.Free();
                this.type = MI_Type.MI_SINT64;
                unsafe
                {
                    *(Int64*)this.data = value;
                }
            }
        }

        internal float Real32
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_REAL32)
                {
                    throw new InvalidCastException();
                }

                float res;
                unsafe
                {
                    res = *(float*)this.data;
                }

                return res;
            }

            set
            {
                this.Free();
                this.type = MI_Type.MI_REAL32;
                unsafe
                {
                    *(float*)this.data = value;
                }
            }
        }

        internal double Real64
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_REAL64)
                {
                    throw new InvalidCastException();
                }

                double res;
                unsafe
                {
                    res = *(double*)this.data;
                }

                return res;
            }

            set
            {
                this.Free();
                this.type = MI_Type.MI_REAL64;
                unsafe
                {
                    *(double*)this.data = value;
                }
            }
        }

        internal char Char16
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_CHAR16)
                {
                    throw new InvalidCastException();
                }

                char res;
                unsafe
                {
                    res = *(char*)this.data;
                }

                return res;
            }

            set
            {
                this.Free();
                this.type = MI_Type.MI_CHAR16;
                unsafe
                {
                    *(char*)this.data = value;
                }
            }
        }

        internal MI_Datetime Datetime
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_DATETIME)
                {
                    throw new InvalidCastException();
                }

                MI_Datetime res;
                unsafe
                {
                    res = *(MI_Datetime*)this.data;
                }

                return res;
            }

            set
            {
                this.Free();
                this.type = MI_Type.MI_DATETIME;
                unsafe
                {
                    *(MI_Datetime*)this.data = value;
                }
            }
        }

        internal byte[] Uint8A
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_UINT8A)
                {
                    throw new InvalidCastException();
                }

                byte[] res = null;
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.data;
                    uint count = arrayPtr->size;
                    res = new byte[count];

                    byte* srcPtr = (byte*)(arrayPtr->data);
                    fixed (byte* destPtr = res)
                    {
                        NativeMethods.memcpy((byte*)destPtr, srcPtr, sizeof(byte), count);
                    }
                }

                return res;
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_UINT8A;
                uint count = (uint)value.Length;
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.data;
                    arrayPtr->size = count;
                    arrayPtr->data = Marshal.AllocHGlobal(sizeof(byte) * (int)count);
                    fixed (byte* src = value)
                    {
                        NativeMethods.memcpy((byte*)(arrayPtr->data), (byte*)src, sizeof(byte), count);
                    }
                }
            }
        }

        internal sbyte[] Sint8A
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_SINT8A)
                {
                    throw new InvalidCastException();
                }

                sbyte[] res = null;
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.data;
                    uint count = arrayPtr->size;
                    res = new sbyte[count];

                    byte* srcPtr = (byte*)(arrayPtr->data);
                    fixed (sbyte* destPtr = res)
                    {
                        NativeMethods.memcpy((byte*)destPtr, srcPtr, sizeof(sbyte), count);
                    }
                }

                return res;
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_SINT8A;
                uint count = (uint)value.Length;
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.data;
                    arrayPtr->size = count;
                    arrayPtr->data = Marshal.AllocHGlobal(sizeof(sbyte) * (int)count);
                    fixed (sbyte* src = value)
                    {
                        NativeMethods.memcpy((byte*)(arrayPtr->data), (byte*)src, sizeof(sbyte), count);
                    }
                }
            }
        }

        internal UInt16[] Uint16A
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_UINT16A)
                {
                    throw new InvalidCastException();
                }

                UInt16[] res = null;
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.data;
                    uint count = arrayPtr->size;
                    res = new UInt16[count];

                    byte* srcPtr = (byte*)(arrayPtr->data);
                    fixed (UInt16* destPtr = res)
                    {
                        NativeMethods.memcpy((byte*)destPtr, srcPtr, sizeof(UInt16), count);
                    }
                }

                return res;
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_UINT16A;
                uint count = (uint)value.Length;
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.data;
                    arrayPtr->size = count;
                    arrayPtr->data = Marshal.AllocHGlobal(sizeof(UInt16) * (int)count);
                    fixed (UInt16* src = value)
                    {
                        NativeMethods.memcpy((byte*)(arrayPtr->data), (byte*)src, sizeof(UInt16), count);
                    }
                }
            }
        }

        internal Int16[] Sint16A
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_SINT16A)
                {
                    throw new InvalidCastException();
                }

                Int16[] res = null;
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.data;
                    uint count = arrayPtr->size;
                    res = new Int16[count];

                    byte* srcPtr = (byte*)(arrayPtr->data);
                    fixed (Int16* destPtr = res)
                    {
                        NativeMethods.memcpy((byte*)destPtr, srcPtr, sizeof(Int16), count);
                    }
                }

                return res;
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_SINT16A;
                uint count = (uint)value.Length;
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.data;
                    arrayPtr->size = count;
                    arrayPtr->data = Marshal.AllocHGlobal(sizeof(Int16) * (int)count);
                    fixed (Int16* src = value)
                    {
                        NativeMethods.memcpy((byte*)(arrayPtr->data), (byte*)src, sizeof(Int16), count);
                    }
                }
            }
        }

        internal UInt32[] Uint32A
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_UINT32A)
                {
                    throw new InvalidCastException();
                }

                UInt32[] res = null;
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.data;
                    uint count = arrayPtr->size;
                    res = new UInt32[count];

                    byte* srcPtr = (byte*)(arrayPtr->data);
                    fixed (UInt32* destPtr = res)
                    {
                        NativeMethods.memcpy((byte*)destPtr, srcPtr, sizeof(UInt32), count);
                    }
                }

                return res;
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_UINT32A;
                uint count = (uint)value.Length;
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.data;
                    arrayPtr->size = count;
                    arrayPtr->data = Marshal.AllocHGlobal(sizeof(UInt32) * (int)count);
                    fixed (UInt32* src = value)
                    {
                        NativeMethods.memcpy((byte*)(arrayPtr->data), (byte*)src, sizeof(UInt32), count);
                    }
                }
            }
        }

        internal Int32[] Sint32A
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_SINT32A)
                {
                    throw new InvalidCastException();
                }

                Int32[] res = null;
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.data;
                    uint count = arrayPtr->size;
                    res = new Int32[count];

                    byte* srcPtr = (byte*)(arrayPtr->data);
                    fixed (Int32* destPtr = res)
                    {
                        NativeMethods.memcpy((byte*)destPtr, srcPtr, sizeof(Int32), count);
                    }
                }

                return res;
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_SINT32A;
                uint count = (uint)value.Length;
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.data;
                    arrayPtr->size = count;
                    arrayPtr->data = Marshal.AllocHGlobal(sizeof(Int32) * (int)count);
                    fixed (Int32* src = value)
                    {
                        NativeMethods.memcpy((byte*)(arrayPtr->data), (byte*)src, sizeof(Int32), count);
                    }
                }
            }
        }

        internal UInt64[] Uint64A
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_UINT64A)
                {
                    throw new InvalidCastException();
                }

                UInt64[] res = null;
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.data;
                    uint count = arrayPtr->size;
                    res = new UInt64[count];

                    byte* srcPtr = (byte*)(arrayPtr->data);
                    fixed (UInt64* destPtr = res)
                    {
                        NativeMethods.memcpy((byte*)destPtr, srcPtr, sizeof(UInt64), count);
                    }
                }

                return res;
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_UINT64A;
                uint count = (uint)value.Length;
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.data;
                    arrayPtr->size = count;
                    arrayPtr->data = Marshal.AllocHGlobal(sizeof(UInt64) * (int)count);
                    fixed (UInt64* src = value)
                    {
                        NativeMethods.memcpy((byte*)(arrayPtr->data), (byte*)src, sizeof(UInt64), count);
                    }
                }
            }
        }

        internal Int64[] Sint64A
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_SINT64A)
                {
                    throw new InvalidCastException();
                }

                Int64[] res = null;
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.data;
                    uint count = arrayPtr->size;
                    res = new Int64[count];

                    byte* srcPtr = (byte*)(arrayPtr->data);
                    fixed (Int64* destPtr = res)
                    {
                        NativeMethods.memcpy((byte*)destPtr, srcPtr, sizeof(Int64), count);
                    }
                }

                return res;
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_SINT64A;
                uint count = (uint)value.Length;
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.data;
                    arrayPtr->size = count;
                    arrayPtr->data = Marshal.AllocHGlobal(sizeof(Int64) * (int)count);
                    fixed (Int64* src = value)
                    {
                        NativeMethods.memcpy((byte*)(arrayPtr->data), (byte*)src, sizeof(Int64), count);
                    }
                }
            }
        }

        internal float[] Real32A
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_REAL32A)
                {
                    throw new InvalidCastException();
                }

                float[] res = null;
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.data;
                    uint count = arrayPtr->size;
                    res = new float[count];

                    byte* srcPtr = (byte*)(arrayPtr->data);
                    fixed (float* destPtr = res)
                    {
                        NativeMethods.memcpy((byte*)destPtr, srcPtr, sizeof(float), count);
                    }
                }

                return res;
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_REAL32A;
                uint count = (uint)value.Length;
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.data;
                    arrayPtr->size = count;
                    arrayPtr->data = Marshal.AllocHGlobal(sizeof(float) * (int)count);
                    fixed (float* src = value)
                    {
                        NativeMethods.memcpy((byte*)(arrayPtr->data), (byte*)src, sizeof(float), count);
                    }
                }
            }
        }

        internal double[] Real64A
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_REAL64A)
                {
                    throw new InvalidCastException();
                }

                double[] res = null;
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.data;
                    uint count = arrayPtr->size;
                    res = new double[count];

                    byte* srcPtr = (byte*)(arrayPtr->data);
                    fixed (double* destPtr = res)
                    {
                        NativeMethods.memcpy((byte*)destPtr, srcPtr, sizeof(double), count);
                    }
                }

                return res;
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_REAL64A;
                uint count = (uint)value.Length;
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.data;
                    arrayPtr->size = count;
                    arrayPtr->data = Marshal.AllocHGlobal(sizeof(double) * (int)count);
                    fixed (double* src = value)
                    {
                        NativeMethods.memcpy((byte*)(arrayPtr->data), (byte*)src, sizeof(double), count);
                    }
                }
            }
        }

        internal char[] Char16A
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_CHAR16A)
                {
                    throw new InvalidCastException();
                }

                char[] res = null;
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.data;
                    uint count = arrayPtr->size;
                    res = new char[count];

                    byte* srcPtr = (byte*)(arrayPtr->data);
                    fixed (char* destPtr = res)
                    {
                        NativeMethods.memcpy((byte*)destPtr, srcPtr, sizeof(char), count);
                    }
                }

                return res;
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_CHAR16A;
                uint count = (uint)value.Length;
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.data;
                    arrayPtr->size = count;
                    arrayPtr->data = Marshal.AllocHGlobal(sizeof(char) * (int)count);
                    fixed (char* src = value)
                    {
                        NativeMethods.memcpy((byte*)(arrayPtr->data), (byte*)src, sizeof(char), count);
                    }
                }
            }
        }

        internal MI_Datetime[] DatetimeA
        {
            get
            {
                if (this.type.HasValue && this.type.Value != MI_Type.MI_DATETIMEA)
                {
                    throw new InvalidCastException();
                }

                MI_Datetime[] res = null;
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.data;
                    uint count = arrayPtr->size;
                    res = new MI_Datetime[count];

                    byte* srcPtr = (byte*)(arrayPtr->data);
                    fixed (MI_Datetime* destPtr = res)
                    {
                        NativeMethods.memcpy((byte*)destPtr, srcPtr, sizeof(MI_Datetime), count);
                    }
                }

                return res;
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_DATETIMEA;
                uint count = (uint)value.Length;
                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.data;
                    arrayPtr->size = count;
                    arrayPtr->data = Marshal.AllocHGlobal(sizeof(MI_Datetime) * (int)count);
                    fixed (MI_Datetime* src = value)
                    {
                        NativeMethods.memcpy((byte*)(arrayPtr->data), (byte*)src, sizeof(MI_Datetime), count);
                    }
                }
            }
        }

        internal object GetValue(MI_Type type)
        {
            switch (type)
            {
                case MI_Type.MI_BOOLEAN:
                    return this.Boolean;

                case MI_Type.MI_UINT8:
                    return this.Uint8;

                case MI_Type.MI_SINT8:
                    return this.Sint8;

                case MI_Type.MI_UINT16:
                    return this.Uint16;

                case MI_Type.MI_SINT16:
                    return this.Sint16;

                case MI_Type.MI_UINT32:
                    return this.Uint32;

                case MI_Type.MI_SINT32:
                    return this.Sint32;

                case MI_Type.MI_UINT64:
                    return this.Uint64;

                case MI_Type.MI_SINT64:
                    return this.Sint64;

                case MI_Type.MI_REAL32:
                    return this.Real32;

                case MI_Type.MI_REAL64:
                    return this.Real64;

                case MI_Type.MI_CHAR16:
                    return this.Char16;

                case MI_Type.MI_DATETIME:
                    return this.Datetime;

                case MI_Type.MI_STRING:
                    return this.String;

                case MI_Type.MI_REFERENCE:
                    return this.Reference;

                case MI_Type.MI_INSTANCE:
                    return this.Instance;

                case MI_Type.MI_BOOLEANA:
                    return this.BooleanA;

                case MI_Type.MI_UINT8A:
                    return this.Uint8A;

                case MI_Type.MI_SINT8A:
                    return this.Sint8A;

                case MI_Type.MI_UINT16A:
                    return this.Uint16A;

                case MI_Type.MI_SINT16A:
                    return this.Sint16A;

                case MI_Type.MI_UINT32A:
                    return this.Uint32A;

                case MI_Type.MI_SINT32A:
                    return this.Sint32A;

                case MI_Type.MI_UINT64A:
                    return this.Uint64A;

                case MI_Type.MI_SINT64A:
                    return this.Sint64A;

                case MI_Type.MI_REAL32A:
                    return this.Real32A;

                case MI_Type.MI_REAL64A:
                    return this.Real64A;

                case MI_Type.MI_CHAR16A:
                    return this.Char16A;

                case MI_Type.MI_DATETIMEA:
                    return this.DatetimeA;

                case MI_Type.MI_STRINGA:
                    return this.StringA;

                case MI_Type.MI_REFERENCEA:
                    return this.ReferenceA;

                case MI_Type.MI_INSTANCEA:
                    return this.InstanceA;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}