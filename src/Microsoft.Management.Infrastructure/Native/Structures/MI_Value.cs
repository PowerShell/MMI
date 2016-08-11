/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/
using System;

namespace Microsoft.Management.Infrastructure.Native
{
    using System.Runtime.InteropServices;
    
    internal class MI_Value : MI_NativeObject
    {
        private static readonly int ByteSize = Marshal.SizeOf<byte>();

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
        
        private MI_Type? type;

        internal MI_Value() : base(true)
        {
        }

        protected override int MembersSize { get { return MI_ValueSize; } }

        ~MI_Value()
        {
            this.Dispose(false);
        }

        public override void Dispose()
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

                // If we get an explicit dispose we should wipe
                // the allocated pointer while we're in here
                // but otherwise we should let the finalizer chain
                // handle that as needed
                if (disposing)
                {
                    Marshal.FreeHGlobal(this.allocatedData);
                    this.allocatedData = IntPtr.Zero;
                }
            }

            base.Dispose();
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
                    inner = *((IntPtr*)this.allocatedData);
                }

                return inner == IntPtr.Zero ? null : MI_PlatformSpecific.PtrToString(inner);
            }
            set
            {
                this.Free();

                this.type = MI_Type.MI_STRING;
                IntPtr inner = MI_PlatformSpecific.StringToPtr(value);
                Marshal.WriteIntPtr(this.allocatedData, inner);
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

                byte nativeBool = Marshal.ReadByte(this.allocatedData);
                return nativeBool != 0;
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_BOOLEAN;
                Marshal.WriteByte(this.allocatedData, (byte)(value ? 1 : 0));
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

                IntPtr inner = Marshal.ReadIntPtr(this.allocatedData);
                return inner == IntPtr.Zero ? null : MI_Instance.NewFromDirectPtr(inner);
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_INSTANCE;
                Marshal.WriteIntPtr(this.allocatedData, value == null ? IntPtr.Zero : value.Ptr);
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

                IntPtr inner = Marshal.ReadIntPtr(this.allocatedData);
                return inner == IntPtr.Zero ? null : MI_Instance.NewFromDirectPtr(inner);
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_REFERENCE;
                Marshal.WriteIntPtr(this.allocatedData, value == null ? IntPtr.Zero : value.Ptr);
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

                var nativeStrings = MI_Array.ReadAsManagedPointerArray(this.allocatedData, MI_String.NewFromDirectPtr);
                if (nativeStrings == null)
                {
                    return null;
                }

                var res = new string[nativeStrings.Length];
                for (int i = 0; i < nativeStrings.Length; i++)
                {
                    res[i] = nativeStrings[i].Value;
                }

                return res;
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_STRINGA;
                this.WipeData();

                if (value != null)
                {
                    var size = value.Length;
                    IntPtr[] ptrs = new IntPtr[size];
                    for (int i = 0; i < size; i++)
                    {
                        ptrs[i] = MI_PlatformSpecific.StringToPtr(value[i]);
                    }

                    NativeMethods.memset(this.allocatedData, 0, MI_Array.MI_ArraySize);
                    MI_Array.WritePointerArray(this.allocatedData, ptrs);
                }
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

                unsafe
                {
                    MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                    if (arrayPtr->data == IntPtr.Zero)
                    {
                        return null;
                    }

                    byte* dataPtr = (byte*)arrayPtr->data;
                    var size = arrayPtr->size;
                    var res = new bool[size];
                    for (int i = 0; i < size; i++)
                    {
                        res[i] = (dataPtr[i]) == 1;
                    }

                    return res;
                }
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_BOOLEANA;

                if (value == null)
                {
                    this.WipeData();
                }
                else
                {
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
                    Marshal.StructureToPtr(array, this.allocatedData, false);
                }
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

                return MI_Array.ReadAsManagedPointerArray(this.allocatedData, MI_Instance.NewFromDirectPtr);
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_INSTANCEA;
                this.WipeData();

                if (value != null)
                {
                    MI_Array.WriteNativeObjectPointers(this.allocatedData, value);
                }
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

                return MI_Array.ReadAsManagedPointerArray(this.allocatedData, MI_Instance.NewFromDirectPtr);
            }
            set
            {
                this.Free();
                this.type = MI_Type.MI_REFERENCEA;
                this.WipeData();

                if (value != null)
                {
                    MI_Array.WriteNativeObjectPointers(this.allocatedData, value);
                }
            }
        }

        internal static int MI_ValueSize = Marshal.SizeOf<MI_ValueLayout>();

        internal static MI_Value NewDirectPtr()
        {
            return new MI_Value();
        }

        internal static MI_Value Null { get { return null; } }

        internal MI_Type? Type { get { return this.type; } }

        private void WipeData()
        {
            NativeMethods.memset(this.allocatedData, 0, MI_Array.MI_ArraySize);
        }

        private void Free()
        {
            if (this.type.HasValue && (this.type.Value & MI_TypeFlags.MI_ARRAY) != 0)
            {
                unsafe
                {
                    Marshal.FreeHGlobal(((MI_Array*)this.allocatedData)->data);
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
                    res = *(byte*)this.allocatedData;
                }

                return res;
            }

            set
            {
                this.Free();
                this.type = MI_Type.MI_UINT8;
                unsafe
                {
                    *(byte*)this.allocatedData = value;
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
                    res = *(sbyte*)this.allocatedData;
                }

                return res;
            }

            set
            {
                this.Free();
                this.type = MI_Type.MI_SINT8;
                unsafe
                {
                    *(sbyte*)this.allocatedData = value;
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
                    res = *(UInt16*)this.allocatedData;
                }

                return res;
            }

            set
            {
                this.Free();
                this.type = MI_Type.MI_UINT16;
                unsafe
                {
                    *(UInt16*)this.allocatedData = value;
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
                    res = *(Int16*)this.allocatedData;
                }

                return res;
            }

            set
            {
                this.Free();
                this.type = MI_Type.MI_SINT16;
                unsafe
                {
                    *(Int16*)this.allocatedData = value;
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
                    res = *(UInt32*)this.allocatedData;
                }

                return res;
            }

            set
            {
                this.Free();
                this.type = MI_Type.MI_UINT32;
                unsafe
                {
                    *(UInt32*)this.allocatedData = value;
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
                    res = *(Int32*)this.allocatedData;
                }

                return res;
            }

            set
            {
                this.Free();
                this.type = MI_Type.MI_SINT32;
                unsafe
                {
                    *(Int32*)this.allocatedData = value;
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
                    res = *(UInt64*)this.allocatedData;
                }

                return res;
            }

            set
            {
                this.Free();
                this.type = MI_Type.MI_UINT64;
                unsafe
                {
                    *(UInt64*)this.allocatedData = value;
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
                    res = *(Int64*)this.allocatedData;
                }

                return res;
            }

            set
            {
                this.Free();
                this.type = MI_Type.MI_SINT64;
                unsafe
                {
                    *(Int64*)this.allocatedData = value;
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
                    res = *(float*)this.allocatedData;
                }

                return res;
            }

            set
            {
                this.Free();
                this.type = MI_Type.MI_REAL32;
                unsafe
                {
                    *(float*)this.allocatedData = value;
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
                    res = *(double*)this.allocatedData;
                }

                return res;
            }

            set
            {
                this.Free();
                this.type = MI_Type.MI_REAL64;
                unsafe
                {
                    *(double*)this.allocatedData = value;
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
                    res = *(char*)this.allocatedData;
                }

                return res;
            }

            set
            {
                this.Free();
                this.type = MI_Type.MI_CHAR16;
                unsafe
                {
                    *(char*)this.allocatedData = value;
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
                    res = *(MI_Datetime*)this.allocatedData;
                }

                return res;
            }

            set
            {
                this.Free();
                this.type = MI_Type.MI_DATETIME;
                unsafe
                {
                    *(MI_Datetime*)this.allocatedData = value;
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
                    MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                    if (arrayPtr->data == IntPtr.Zero)
                    {
                        return null;
                    }

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

                if (value == null)
                {
                    this.WipeData();
                }
                else
                {
                    uint count = (uint)value.Length;
                    unsafe
                    {
                        MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                        arrayPtr->size = count;
                        arrayPtr->data = Marshal.AllocHGlobal(sizeof(byte) * (int)count);
                        fixed (byte* src = value)
                        {
                            NativeMethods.memcpy((byte*)(arrayPtr->data), (byte*)src, sizeof(byte), count);
                        }
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
                    MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                    if (arrayPtr->data == IntPtr.Zero)
                    {
                        return null;
                    }

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
                if (value == null)
                {
                    this.WipeData();
                }
                else
                {
                    uint count = (uint)value.Length;
                    unsafe
                    {
                        MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                        arrayPtr->size = count;
                        arrayPtr->data = Marshal.AllocHGlobal(sizeof(sbyte) * (int)count);
                        fixed (sbyte* src = value)
                        {
                            NativeMethods.memcpy((byte*)(arrayPtr->data), (byte*)src, sizeof(sbyte), count);
                        }
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
                    MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                    if (arrayPtr->data == IntPtr.Zero)
                    {
                        return null;
                    }

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

                if (value == null)
                {
                    this.WipeData();
                }
                else
                {
                    uint count = (uint)value.Length;
                    unsafe
                    {
                        MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                        arrayPtr->size = count;
                        arrayPtr->data = Marshal.AllocHGlobal(sizeof(UInt16) * (int)count);
                        fixed (UInt16* src = value)
                        {
                            NativeMethods.memcpy((byte*)(arrayPtr->data), (byte*)src, sizeof(UInt16), count);
                        }
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
                    MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                    if (arrayPtr->data == IntPtr.Zero)
                    {
                        return null;
                    }

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

                if (value == null)
                {
                    this.WipeData();
                }
                else
                {
                    uint count = (uint)value.Length;
                    unsafe
                    {
                        MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                        arrayPtr->size = count;
                        arrayPtr->data = Marshal.AllocHGlobal(sizeof(Int16) * (int)count);
                        fixed (Int16* src = value)
                        {
                            NativeMethods.memcpy((byte*)(arrayPtr->data), (byte*)src, sizeof(Int16), count);
                        }
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
                    MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                    if (arrayPtr->data == IntPtr.Zero)
                    {
                        return null;
                    }

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
                if (value == null)
                {
                    this.WipeData();
                }
                else
                {
                    uint count = (uint)value.Length;
                    unsafe
                    {
                        MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                        arrayPtr->size = count;
                        arrayPtr->data = Marshal.AllocHGlobal(sizeof(UInt32) * (int)count);
                        fixed (UInt32* src = value)
                        {
                            NativeMethods.memcpy((byte*)(arrayPtr->data), (byte*)src, sizeof(UInt32), count);
                        }
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
                    MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                    if (arrayPtr->data == IntPtr.Zero)
                    {
                        return null;
                    }

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
                if (value == null)
                {
                    this.WipeData();
                }
                else
                {
                    this.type = MI_Type.MI_SINT32A;
                    uint count = (uint)value.Length;
                    unsafe
                    {
                        MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                        arrayPtr->size = count;
                        arrayPtr->data = Marshal.AllocHGlobal(sizeof(Int32) * (int)count);
                        fixed (Int32* src = value)
                        {
                            NativeMethods.memcpy((byte*)(arrayPtr->data), (byte*)src, sizeof(Int32), count);
                        }
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
                    MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                    if (arrayPtr->data == IntPtr.Zero)
                    {
                        return null;
                    }

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
                if (value == null)
                {
                    this.WipeData();
                }
                else
                {
                    uint count = (uint)value.Length;
                    unsafe
                    {
                        MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                        arrayPtr->size = count;
                        arrayPtr->data = Marshal.AllocHGlobal(sizeof(UInt64) * (int)count);
                        fixed (UInt64* src = value)
                        {
                            NativeMethods.memcpy((byte*)(arrayPtr->data), (byte*)src, sizeof(UInt64), count);
                        }
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
                    MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                    if (arrayPtr->data == IntPtr.Zero)
                    {
                        return null;
                    }

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
                if (value == null)
                {
                    this.WipeData();
                }
                else
                {
                    uint count = (uint)value.Length;
                    unsafe
                    {
                        MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                        arrayPtr->size = count;
                        arrayPtr->data = Marshal.AllocHGlobal(sizeof(Int64) * (int)count);
                        fixed (Int64* src = value)
                        {
                            NativeMethods.memcpy((byte*)(arrayPtr->data), (byte*)src, sizeof(Int64), count);
                        }
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
                    MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                    if (arrayPtr->data == IntPtr.Zero)
                    {
                        return null;
                    }

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
                if (value == null)
                {
                    this.WipeData();
                }
                else
                {
                    uint count = (uint)value.Length;
                    unsafe
                    {
                        MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                        arrayPtr->size = count;
                        arrayPtr->data = Marshal.AllocHGlobal(sizeof(float) * (int)count);
                        fixed (float* src = value)
                        {
                            NativeMethods.memcpy((byte*)(arrayPtr->data), (byte*)src, sizeof(float), count);
                        }
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
                    MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                    if (arrayPtr->data == IntPtr.Zero)
                    {
                        return null;
                    }

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
                if (value == null)
                {
                    this.WipeData();
                }
                else
                {
                    uint count = (uint)value.Length;
                    unsafe
                    {
                        MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                        arrayPtr->size = count;
                        arrayPtr->data = Marshal.AllocHGlobal(sizeof(double) * (int)count);
                        fixed (double* src = value)
                        {
                            NativeMethods.memcpy((byte*)(arrayPtr->data), (byte*)src, sizeof(double), count);
                        }
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
                    MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                    if (arrayPtr->data == IntPtr.Zero)
                    {
                        return null;
                    }

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
                if (value == null)
                {
                    this.WipeData();
                }
                else
                {
                    uint count = (uint)value.Length;
                    unsafe
                    {
                        MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                        arrayPtr->size = count;
                        arrayPtr->data = Marshal.AllocHGlobal(sizeof(char) * (int)count);
                        fixed (char* src = value)
                        {
                            NativeMethods.memcpy((byte*)(arrayPtr->data), (byte*)src, sizeof(char), count);
                        }
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
                    MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                    if (arrayPtr->data == IntPtr.Zero)
                    {
                        return null;
                    }

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
                if (value == null)
                {
                    this.WipeData();
                }
                else
                {
                    uint count = (uint)value.Length;
                    unsafe
                    {
                        MI_Array* arrayPtr = (MI_Array*)this.allocatedData;
                        arrayPtr->size = count;
                        arrayPtr->data = Marshal.AllocHGlobal(sizeof(MI_Datetime) * (int)count);
                        fixed (MI_Datetime* src = value)
                        {
                            NativeMethods.memcpy((byte*)(arrayPtr->data), (byte*)src, sizeof(MI_Datetime), count);
                        }
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
