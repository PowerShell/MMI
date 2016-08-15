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
    internal class MI_Application : MI_NativeObjectWithFT<MI_Application.MI_ApplicationFT>
    {
        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        private struct MI_ApplicationMembers
        {
            internal UInt64 reserved1;
            internal IntPtr reserved2;
            internal IntPtr ft;
        }

        static MI_Application()
        {
            CheckMembersTableMatchesNormalLayout<MI_ApplicationMembers>("ft");
        }

        internal static MI_Result Initialize(string applicationId, out MI_Instance extendedError, out MI_Application application)
        {
            MI_Application applicationLocal = MI_Application.NewDirectPtr();
            MI_Instance extendedErrorLocal = MI_Instance.NewIndirectPtr();

            MI_Result result = NativeMethods.MI_Application_InitializeV1(0, applicationId, extendedErrorLocal, applicationLocal);

            extendedError = extendedErrorLocal;
            application = applicationLocal;
            return result;
        }

        internal MI_Result NewSession(
            string protocol,
            string destination,
            MI_DestinationOptions options,
            MI_SessionCreationCallbacks callbacks,
            out MI_Instance extendedError,
            out MI_Session session
            )
        {
            if (callbacks != null)
            {
                throw new NotImplementedException();
            }

            MI_Instance extendedErrorLocal = MI_Instance.NewIndirectPtr();
            MI_Session sessionLocal = MI_Session.NewDirectPtr();

            MI_Result resultLocal = this.ft.NewSession(this,
                protocol,
                destination,
                options,
                null,
                extendedErrorLocal,
                sessionLocal);

            extendedError = extendedErrorLocal;
            session = sessionLocal;
            return resultLocal;
        }
        
        private MI_Application(bool isDirect) : base(isDirect)
        {
        }

        private MI_Application(IntPtr existingPtr) : base(existingPtr)
        {
        }

        internal static MI_Application NewDirectPtr()
        {
            return new MI_Application(true);
        }

        internal static MI_Application NewIndirectPtr()
        {
            return new MI_Application(false);
        }

        internal static MI_Application NewFromDirectPtr(IntPtr ptr)
        {
            return new MI_Application(ptr);
        }

        internal static MI_Application Null { get { return null; } }

        internal MI_Result Close()
        {
            return this.ft.Close(this);
        }

        internal MI_Result NewHostedProvider(
            string namespaceName,
            string providerName,
            IntPtr mi_Main,
            out MI_Instance extendedError,
            IntPtr provider
            )
        {
            MI_Instance extendedErrorLocal = MI_Instance.NewIndirectPtr();

            MI_Result resultLocal = this.ft.NewHostedProvider(this,
                namespaceName,
                providerName,
                mi_Main,
                extendedErrorLocal,
                provider);

            extendedError = extendedErrorLocal;
            return resultLocal;
        }

        internal MI_Result NewInstance(
            string className,
            MI_ClassDecl classRTTI,
            out MI_Instance instance
            )
        {
            MI_Instance instanceLocal = MI_Instance.NewIndirectPtr();

            MI_Result resultLocal = this.ft.NewInstance(this,
                className,
                classRTTI,
                instanceLocal);

            instance = instanceLocal;
            return resultLocal;
        }

        internal MI_Result NewDestinationOptions(
            out MI_DestinationOptions options
            )
        {
            MI_DestinationOptions optionsLocal = MI_DestinationOptions.NewDirectPtr();
            MI_Result resultLocal = this.ft.NewDestinationOptions(this,
                optionsLocal);

            options = optionsLocal;
            return resultLocal;
        }

        internal MI_Result NewOperationOptions(
            bool customOptionsMustUnderstand,
            out MI_OperationOptions operationOptions
            )
        {
            MI_OperationOptions operationOptionsLocal = MI_OperationOptions.NewDirectPtr();

            MI_Result resultLocal = this.ft.NewOperationOptions(this,
                customOptionsMustUnderstand,
                operationOptionsLocal);

            operationOptions = operationOptionsLocal;
            return resultLocal;
        }

        internal MI_Result NewSubscriptionDeliveryOptions(
            MI_SubscriptionDeliveryType deliveryType,
            out MI_SubscriptionDeliveryOptions deliveryOptions
            )
        {
            MI_SubscriptionDeliveryOptions deliveryOptionsLocal =
                MI_SubscriptionDeliveryOptions.NewIndirectPtr();

            MI_Result resultLocal = this.ft.NewSubscriptionDeliveryOptions(this,
                deliveryType,
                deliveryOptionsLocal);

            deliveryOptions = deliveryOptionsLocal;
            return resultLocal;
        }

        internal MI_Result NewSerializer(
            MI_SerializerFlags flags,
            string format,
            out MI_Serializer serializer
            )
        {
            MI_Serializer serializerLocal = MI_Serializer.NewDirectPtr(format);

            MI_Result resultLocal;
#if !_LINUX
            if (MI_SerializationFormat.XML.Equals(format, StringComparison.Ordinal))
            {
                resultLocal = this.ft.NewSerializer(this,
                    flags,
                    format,
                    serializerLocal);
            }
            else if (MI_SerializationFormat.MOF.Equals(format, StringComparison.Ordinal))
            {
                resultLocal = NativeMethods.MI_Application_NewSerializer_Mof(this,
                    flags,
                    format,
                    serializerLocal);
            }
            else
            {
                throw new NotImplementedException();
            }
#else
            resultLocal = this.ft.NewSerializer(this,
                flags,
                format,
                serializerLocal);
#endif
            serializer = serializerLocal;
            return resultLocal;
        }

        internal MI_Result NewDeserializer(
            MI_SerializerFlags flags,
            string format,
            out MI_Deserializer deserializer
            )
        {
            MI_Deserializer deserializerLocal = MI_Deserializer.NewDirectPtr(format);

            MI_Result resultLocal;
#if !_LINUX
            if (MI_SerializationFormat.XML.Equals(format, StringComparison.Ordinal))
            {
                resultLocal = this.ft.NewDeserializer(this,
                    flags,
                    format,
                    deserializerLocal);
            }
            else if (MI_SerializationFormat.MOF.Equals(format, StringComparison.Ordinal))
            {
                resultLocal = NativeMethods.MI_Application_NewDeserializer_Mof(this,
                    flags,
                    format,
                    deserializerLocal);
            }
            else
            {
                throw new NotImplementedException();
            }
#else
            resultLocal = this.ft.NewDeserializer(this,
                flags,
                format,
                deserializerLocal);
#endif

            deserializer = deserializerLocal;
            return resultLocal;
        }

        internal MI_Result NewInstanceFromClass(
            string className,
            MI_Class classObject,
            out MI_Instance instance
            )
        {
            MI_Instance instanceLocal = MI_Instance.NewIndirectPtr();

            MI_Result resultLocal = this.ft.NewInstanceFromClass(this,
                className,
                classObject,
                instanceLocal);

            instance = instanceLocal;
            return resultLocal;
        }

#if !_LINUX

        internal MI_Result NewClass(
            MI_ClassDecl classDecl,
            string namespaceName,
            string serverName,
            out MI_Class classObject
            )
        {
            MI_Class classObjectLocal = MI_Class.NewIndirectPtr();

            MI_Result resultLocal = this.ft.NewClass(this,
                classDecl,
                namespaceName,
                serverName,
                classObjectLocal);

            classObject = classObjectLocal;
            return resultLocal;
        }

#endif

        internal MI_Result NewParameterSet(
            MI_ClassDecl classDecl,
            out MI_ParameterSet parameterSet
            )
        {
            MI_ParameterSet parameterSetLocal = MI_ParameterSet.NewIndirectPtr();

            MI_Result resultLocal = this.ft.NewInstance(this,
                "Parameters",
                classDecl,
                (MI_Instance.IndirectPtr)parameterSetLocal);

            parameterSet = parameterSetLocal;
            return resultLocal;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal class MI_ApplicationFT
        {
            internal MI_Application_Close Close;
            internal MI_Application_NewSession NewSession;
            internal MI_Application_NewHostedProvider NewHostedProvider;
            internal MI_Application_NewInstance NewInstance;
            internal MI_Application_NewDestinationOptions NewDestinationOptions;
            internal MI_Application_NewOperationOptions NewOperationOptions;
            internal MI_Application_NewSubscriptionDeliveryOptions NewSubscriptionDeliveryOptions;
            internal MI_Application_NewSerializer NewSerializer;
            internal MI_Application_NewDeserializer NewDeserializer;
            internal MI_Application_NewInstanceFromClass NewInstanceFromClass;
#if !_LINUX
            internal MI_Application_NewClass NewClass;
#endif

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Application_Close(
                DirectPtr application
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Application_NewSession(
                DirectPtr application,
                string protocol,
                string destination,
                [In, Out] MI_DestinationOptions.DirectPtr options,
                MI_SessionCreationCallbacks.MI_SessionCreationCallbacksNative callbacks,
                [In, Out] MI_Instance.IndirectPtr extendedError,
                [In, Out] MI_Session.DirectPtr session
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Application_NewHostedProvider(
                DirectPtr application,
                string namespaceName,
                string providerName,
                IntPtr mi_Main,
                [In, Out] MI_Instance.IndirectPtr extendedError,
                IntPtr provider
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Application_NewInstance(
                DirectPtr application,
                string className,
                [In, Out] MI_ClassDecl.DirectPtr classRTTI,
                [In, Out] MI_Instance.IndirectPtr instance
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Application_NewDestinationOptions(
                DirectPtr application,
                [In, Out] MI_DestinationOptions.DirectPtr options
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Application_NewOperationOptions(
                DirectPtr application,
                [MarshalAs(UnmanagedType.U1)] bool customOptionsMustUnderstand,
                [In, Out] MI_OperationOptions.DirectPtr operationOptions
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Application_NewSubscriptionDeliveryOptions(
                DirectPtr application,
                MI_SubscriptionDeliveryType deliveryType,
                [In, Out] MI_SubscriptionDeliveryOptions.DirectPtr deliveryOptions
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Application_NewSerializer(
                DirectPtr application,
                MI_SerializerFlags flags,
                string format,
                MI_Serializer.DirectPtr serializer
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Application_NewDeserializer(
                DirectPtr application,
                MI_SerializerFlags flags,
                string format,
                MI_Deserializer.DirectPtr deserializer
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Application_NewInstanceFromClass(
                DirectPtr application,
                string className,
                [In, Out] MI_Class.DirectPtr classObject,
                [In, Out] MI_Instance.IndirectPtr instance
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Application_NewClass(
                DirectPtr application,
                MI_ClassDecl classDecl,
                string namespaceName,
                string serverName,
                [In, Out] MI_Class.IndirectPtr classObject
                );
        }
    }
}
