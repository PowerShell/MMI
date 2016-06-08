using System;
using System.Runtime.InteropServices;

namespace Microsoft.Management.Infrastructure.Native
{
    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal class MI_Serializer
    {
        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal struct MI_SerializerPtr
        {
            internal IntPtr ptr;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal struct MI_SerializerOutPtr
        {
            internal IntPtr ptr;
        }

        // Marshal implements these with Reflection - pay this hit only once
        internal static int Reserved2Offset = (int)Marshal.OffsetOf<MI_Serializer.MI_SerializerMembers>("reserved2");

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        private struct MI_SerializerMembers
        {
            internal UInt64 reserved1;
            internal IntPtr reserved2;
        }

        // Marshal implements these with Reflection - pay this hit only once
        private static int MI_SerializerMembersSize = Marshal.SizeOf<MI_SerializerMembers>();

        private MI_SerializerPtr ptr;
        private bool isDirect;
        private Lazy<MI_SerializerFT> mft;

        ~MI_Serializer()
        {
            Marshal.FreeHGlobal(this.ptr.ptr);
        }

        private MI_Serializer(string format, bool isDirect)
        {
#if !_LINUX
            if (MI_SerializationFormat.XML.Equals(format, StringComparison.Ordinal))
            {
                this.mft = MI_SerializerFT.XmlSerializerFT;
            }
            else if (MI_SerializationFormat.MOF.Equals(format, StringComparison.Ordinal))
            {
                this.mft = new Lazy<MI_SerializerFT>(() => MI_SerializationFTHelpers.GetSerializerFTFromReserved2(this));
            }
            else
            {
                throw new NotImplementedException();
            }
#else
            this.mft = new Lazy<MI_SerializerFT>( () => MI_SerializationFTHelpers.GetSerializerFTFromReserved2(this) );
#endif
            this.isDirect = isDirect;

            var necessarySize = this.isDirect ? MI_SerializerMembersSize : NativeMethods.IntPtrSize;
            this.ptr.ptr = Marshal.AllocHGlobal(necessarySize);

            unsafe
            {
                NativeMethods.memset((byte*)this.ptr.ptr, 0, (uint)necessarySize);
            }
        }

        internal static MI_Serializer NewDirectPtr(string format)
        {
            return new MI_Serializer(format, true);
        }

        public static implicit operator MI_SerializerPtr(MI_Serializer instance)
        {
            // If the indirect pointer is zero then the object has not
            // been initialized and it is not valid to refer to its data
            if (instance != null && instance.Ptr == IntPtr.Zero)
            {
                throw new InvalidCastException();
            }

            return new MI_SerializerPtr() { ptr = instance == null ? IntPtr.Zero : instance.Ptr };
        }

        public static implicit operator MI_SerializerOutPtr(MI_Serializer instance)
        {
            // We are not currently supporting the ability to get the address
            // of our direct pointer, though it is technically feasible
            if (instance != null && instance.isDirect)
            {
                throw new InvalidCastException();
            }

            return new MI_SerializerOutPtr() { ptr = instance == null ? IntPtr.Zero : instance.ptr.ptr };
        }

        internal static MI_Serializer Null { get { return null; } }
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

        internal MI_Result Close()
        {
            return this.ft.Close(this);
        }

        internal MI_Result SerializeClass(
            MI_SerializerFlags flags,
            MI_Class classObject,
            IntPtr clientBuffer,
            UInt32 clientBufferLength,
            out UInt32 clientBufferNeeded
            )
        {
            MI_Result resultLocal = this.ft.SerializeClass(this,
                flags,
                classObject,
                clientBuffer,
                clientBufferLength,
                out clientBufferNeeded);
            return resultLocal;
        }

        internal MI_Result SerializeClass(
            MI_SerializerFlags flags,
            MI_Class classObject,
            out byte[] clientBuffer
            )
        {
            clientBuffer = null;
            UInt32 spaceNeeded = 0;
            MI_Result resultLocal = this.ft.SerializeClass(this,
                flags,
                classObject,
                IntPtr.Zero,
                0,
                out spaceNeeded);
            if (resultLocal == MI_Result.MI_RESULT_OK || (resultLocal == MI_Result.MI_RESULT_FAILED && spaceNeeded != 0))
            {
                UInt32 spaceUsed;
                IntPtr clientBufferLocal = Marshal.AllocHGlobal((IntPtr)spaceNeeded);
                resultLocal = this.ft.SerializeClass(this,
                    flags,
                    classObject,
                    clientBufferLocal,
                    spaceNeeded,
                    out spaceUsed);
                if (clientBufferLocal != IntPtr.Zero)
                {
                    clientBuffer = new byte[spaceNeeded];
                    Marshal.Copy(clientBufferLocal, clientBuffer, 0, (int)spaceNeeded);
                    Marshal.FreeHGlobal(clientBufferLocal);
                }
            }

            return resultLocal;
        }

        internal MI_Result SerializeInstance(
            MI_SerializerFlags flags,
            MI_Instance instanceObject,
            out byte[] clientBuffer
            )
        {
            clientBuffer = null;
            UInt32 spaceNeeded = 0;
            MI_Result resultLocal = this.ft.SerializeInstance(this,
                flags,
                instanceObject,
                IntPtr.Zero,
                0,
                out spaceNeeded);
            if (resultLocal == MI_Result.MI_RESULT_OK || (resultLocal == MI_Result.MI_RESULT_FAILED && spaceNeeded != 0))
            {
                UInt32 spaceUsed;
                IntPtr clientBufferLocal = Marshal.AllocHGlobal((IntPtr)spaceNeeded);
                resultLocal = this.ft.SerializeInstance(this,
                    flags,
                    instanceObject,
                    clientBufferLocal,
                    spaceNeeded,
                    out spaceUsed);
                if (clientBufferLocal != IntPtr.Zero)
                {
                    clientBuffer = new byte[spaceNeeded];
                    Marshal.Copy(clientBufferLocal, clientBuffer, 0, (int)spaceNeeded);
                    Marshal.FreeHGlobal(clientBufferLocal);
                }
            }

            return resultLocal;
        }

        internal MI_Result SerializeInstance(
            MI_SerializerFlags flags,
            MI_Instance instanceObject,
            IntPtr clientBuffer,
            UInt32 clientBufferLength,
            out UInt32 clientBufferNeeded
            )
        {
            MI_Result resultLocal = this.ft.SerializeInstance(this,
                flags,
                instanceObject,
                clientBuffer,
                clientBufferLength,
                out clientBufferNeeded);
            return resultLocal;
        }

        private MI_SerializerFT ft { get { return this.mft.Value; } }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal class MI_SerializerFT
        {
            internal MI_Serializer_Close Close;
            internal MI_Serializer_SerializeClass SerializeClass;
            internal MI_Serializer_SerializeInstance SerializeInstance;

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Serializer_Close(
                MI_SerializerPtr serializer
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Serializer_SerializeClass(
                MI_SerializerPtr serializer,
                MI_SerializerFlags flags,
                [In, Out] MI_Class.MI_ClassPtr classObject,
                IntPtr clientBuffer,
                UInt32 clientBufferLength,
                out UInt32 clientBufferNeeded
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Serializer_SerializeInstance(
                MI_SerializerPtr serializer,
                MI_SerializerFlags flags,
                [In, Out] MI_Instance.MI_InstancePtr instanceObject,
                IntPtr clientBuffer,
                UInt32 clientBufferLength,
                out UInt32 clientBufferNeeded
                );

            internal static Lazy<MI_SerializerFT> XmlSerializerFT = new Lazy<MI_SerializerFT>(() => MI_SerializationFTHelpers.XMLSerializationFT);
        }
    }
}