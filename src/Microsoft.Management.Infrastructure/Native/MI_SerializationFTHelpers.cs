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
    internal class MI_SerializationFTHelpers
    {
        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        private struct MI_ClientFT_V1
        {
            public IntPtr g_applicationFT;
            public IntPtr _sessionFT;
            public IntPtr _operationFT;
            public IntPtr _hostedProviderFT;
            public IntPtr _serializerFT;
            public IntPtr _deserializerFT;
            public IntPtr g_subscriptionDeliveryOptionsFT;
            public IntPtr g_destinationOptionsFT;
            public IntPtr g_operationOptionsFT;
            public IntPtr _utilitiesFT;
        }

        private struct SerializationFTPair
        {
            public MI_Serializer.MI_SerializerFT SerializationFT;
            public MI_Deserializer.MI_DeserializerFT DeserializationFT;
        }

        private static Lazy<SerializationFTPair> XmlSerializationFTs = new Lazy<SerializationFTPair>(() => GetXmlSerializationFTs());

        internal static MI_Serializer.MI_SerializerFT XMLSerializationFT { get { return XmlSerializationFTs.Value.SerializationFT; } }

        internal static MI_Deserializer.MI_DeserializerFT XMLDeserializationFT { get { return XmlSerializationFTs.Value.DeserializationFT; } }

        private static SerializationFTPair GetXmlSerializationFTs()
        {
            IntPtr module = NativeMethods.LoadLibrary("mi.dll");
            if (module == IntPtr.Zero)
            {
                throw new InvalidOperationException();
            }

            try
            {
                IntPtr exportedAddress = NativeMethods.GetProcAddress(module, "mi_clientFT_V1");
                IntPtr structAddress = Marshal.ReadIntPtr(exportedAddress);
                MI_ClientFT_V1 clientFT = Marshal.PtrToStructure<MI_ClientFT_V1>(structAddress);
                return new SerializationFTPair()
                {
                    SerializationFT = Marshal.PtrToStructure<MI_Serializer.MI_SerializerFT>(clientFT._serializerFT),
                    DeserializationFT = Marshal.PtrToStructure<MI_Deserializer.MI_DeserializerFT>(clientFT._deserializerFT),
                };
            }
            finally
            {
                NativeMethods.FreeLibrary(module);
            }
        }
    }
}
