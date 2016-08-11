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
    internal class MI_Serializer : MI_NativeObjectWithFT<MI_Serializer.MI_SerializerFT>
    {
        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        private struct MI_SerializerMembers
        {
            internal UInt64 reserved1;
            internal IntPtr reserved2;
        }

        // Marshal implements these with Reflection - pay this hit only once
        private static int MI_SerializerMembersSize = Marshal.SizeOf<MI_SerializerMembers>();
        internal static int MI_SerializerMembersReserved2Offset = (int)Marshal.OffsetOf<MI_Serializer.MI_SerializerMembers>("reserved2");
        
        private MI_Serializer() : base(true)
        {
        }

        private MI_Serializer(Func<MI_SerializerFT> mftThunk) : base(true, mftThunk)
        {
        }

        internal static MI_Serializer NewDirectPtr(string format)
        {
            if (MI_SerializationFormat.XML.Equals(format, StringComparison.Ordinal))
            {
#if !_LINUX
                return new MI_Serializer(() => MI_SerializerFT.XmlSerializerFT.Value);
#endif
            }
            else if (MI_SerializationFormat.MOF.Equals(format, StringComparison.Ordinal))
            {
                // Nothing, see continuation after the conditional
            }
            else
            {
                throw new NotImplementedException();
            }

            // This will end up using the default behavior for the base class
            // which is to use the offset to pull the FT pointer from the object
            return new MI_Serializer();
        }
        
        internal static MI_Serializer Null { get { return null; } }

        protected override int FunctionTableOffset { get { return MI_SerializerMembersReserved2Offset; } }

        protected override int MembersSize { get { return MI_SerializerMembersSize; } }

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

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal class MI_SerializerFT
        {
            internal MI_Serializer_Close Close;
            internal MI_Serializer_SerializeClass SerializeClass;
            internal MI_Serializer_SerializeInstance SerializeInstance;

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Serializer_Close(
                DirectPtr serializer
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Serializer_SerializeClass(
                DirectPtr serializer,
                MI_SerializerFlags flags,
                [In, Out] MI_Class.DirectPtr classObject,
                IntPtr clientBuffer,
                UInt32 clientBufferLength,
                out UInt32 clientBufferNeeded
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Serializer_SerializeInstance(
                DirectPtr serializer,
                MI_SerializerFlags flags,
                [In, Out] MI_Instance.DirectPtr instanceObject,
                IntPtr clientBuffer,
                UInt32 clientBufferLength,
                out UInt32 clientBufferNeeded
                );

            internal static Lazy<MI_SerializerFT> XmlSerializerFT = new Lazy<MI_SerializerFT>(() => MI_SerializationFTHelpers.XMLSerializationFT);
        }
    }
}
