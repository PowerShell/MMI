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
    internal class MI_Session : MI_NativeObjectWithFT<MI_Session.MI_SessionFT>
    {
        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        private struct MI_SessionMembers
        {
            internal UInt64 reserved1;
            internal IntPtr reserved2;
            internal IntPtr ft;
        }

        static MI_Session()
        {
            CheckMembersTableMatchesNormalLayout<MI_SessionMembers>("ft");
        }

        internal void GetInstance(
            MI_OperationFlags flags,
            MI_OperationOptions options,
            string namespaceName,
            MI_Instance inboundInstance,
            MI_OperationCallbacks callbacks,
            out MI_Operation operation
            )
        {
            if (callbacks != null)
            {
                throw new NotImplementedException();
            }

            MI_Operation operationLocal = MI_Operation.NewDirectPtr();

            this.ft.GetInstance(this,
                flags,
                options,
                namespaceName,
                inboundInstance,
                null,
                operationLocal);

            operation = operationLocal;
        }

        internal void ModifyInstance(
            MI_OperationFlags flags,
            MI_OperationOptions options,
            string namespaceName,
            MI_Instance inboundInstance,
            MI_OperationCallbacks callbacks,
            out MI_Operation operation
            )
        {
            if (callbacks != null)
            {
                throw new NotImplementedException();
            }

            MI_Operation operationLocal = MI_Operation.NewDirectPtr();

            this.ft.ModifyInstance(this,
                flags,
                options,
                namespaceName,
                inboundInstance,
                null,
                operationLocal);

            operation = operationLocal;
        }

        internal void CreateInstance(
            MI_OperationFlags flags,
            MI_OperationOptions options,
            string namespaceName,
            MI_Instance inboundInstance,
            MI_OperationCallbacks callbacks,
            out MI_Operation operation
            )
        {
            if (callbacks != null)
            {
                throw new NotImplementedException();
            }

            MI_Operation operationLocal = MI_Operation.NewDirectPtr();

            this.ft.CreateInstance(this,
                flags,
                options,
                namespaceName,
                inboundInstance,
                null,
                operationLocal);

            operation = operationLocal;
        }

        internal void DeleteInstance(
            MI_OperationFlags flags,
            MI_OperationOptions options,
            string namespaceName,
            MI_Instance inboundInstance,
            MI_OperationCallbacks callbacks,
            out MI_Operation operation
            )
        {
            if (callbacks != null)
            {
                throw new NotImplementedException();
            }

            MI_Operation operationLocal = MI_Operation.NewDirectPtr();

            this.ft.DeleteInstance(this,
                flags,
                options,
                namespaceName,
                inboundInstance,
                null,
                operationLocal);

            operation = operationLocal;
        }

        internal void Invoke(
            MI_OperationFlags flags,
            MI_OperationOptions options,
            string namespaceName,
            string className,
            string methodName,
            MI_Instance inboundInstance,
            MI_Instance inboundProperties,
            MI_OperationCallbacks callbacks,
            out MI_Operation operation
            )
        {
            if (callbacks != null)
            {
                throw new NotImplementedException();
            }

            MI_Operation operationLocal = MI_Operation.NewDirectPtr();

            this.ft.Invoke(this,
                flags,
                options,
                namespaceName,
                className,
                methodName,
                inboundInstance,
                inboundProperties,
                null,
                operationLocal);

            operation = operationLocal;
        }

        internal void EnumerateInstances(
            MI_OperationFlags flags,
            MI_OperationOptions options,
            string namespaceName,
            string className,
            bool keysOnly,
            MI_OperationCallbacks callbacks,
            out MI_Operation operation
            )
        {
            if (callbacks != null)
            {
                throw new NotImplementedException();
            }

            MI_Operation operationLocal = MI_Operation.NewDirectPtr();

            this.ft.EnumerateInstances(this,
                flags,
                options,
                namespaceName,
                className,
                keysOnly,
                null,
                operationLocal);

            operation = operationLocal;
        }

        internal void QueryInstances(
            MI_OperationFlags flags,
            MI_OperationOptions options,
            string namespaceName,
            string queryDialect,
            string queryExpression,
            MI_OperationCallbacks callbacks,
            out MI_Operation operation
            )
        {
            if (callbacks != null)
            {
                throw new NotImplementedException();
            }

            MI_Operation operationLocal = MI_Operation.NewDirectPtr();

            this.ft.QueryInstances(this,
                flags,
                options,
                namespaceName,
                queryDialect,
                queryExpression,
                null,
                operationLocal);

            operation = operationLocal;
        }

        internal void AssociatorInstances(
            MI_OperationFlags flags,
            MI_OperationOptions options,
            string namespaceName,
            MI_Instance instanceKeys,
            string assocClass,
            string resultClass,
            string role,
            string resultRole,
            bool keysOnly,
            MI_OperationCallbacks callbacks,
            out MI_Operation operation
            )
        {
            if (callbacks != null)
            {
                throw new NotImplementedException();
            }

            MI_Operation operationLocal = MI_Operation.NewDirectPtr();

            this.ft.AssociatorInstances(this,
                flags,
                options,
                namespaceName,
                instanceKeys,
                assocClass,
                resultClass,
                role,
                resultRole,
                keysOnly,
                null,
                operationLocal);

            operation = operationLocal;
        }

        internal void ReferenceInstances(
            MI_OperationFlags flags,
            MI_OperationOptions options,
            string namespaceName,
            MI_Instance instanceKeys,
            string resultClass,
            string role,
            bool keysOnly,
            MI_OperationCallbacks callbacks,
            out MI_Operation operation
            )
        {
            if (callbacks != null)
            {
                throw new NotImplementedException();
            }

            MI_Operation operationLocal = MI_Operation.NewDirectPtr();

            this.ft.ReferenceInstances(this,
                flags,
                options,
                namespaceName,
                instanceKeys,
                resultClass,
                role,
                keysOnly,
                null,
                operationLocal);

            operation = operationLocal;
        }

        internal void Subscribe(
            MI_OperationFlags flags,
            MI_OperationOptions options,
            string namespaceName,
            string queryDialect,
            string queryExpression,
            MI_SubscriptionDeliveryOptions deliverOptions,
            MI_OperationCallbacks callbacks,
            out MI_Operation operation
            )
        {
            if (callbacks != null)
            {
                throw new NotImplementedException();
            }

            MI_Operation operationLocal = MI_Operation.NewDirectPtr();

            this.ft.Subscribe(this,
                flags,
                options,
                namespaceName,
                queryDialect,
                queryExpression,
                deliverOptions,
                null,
                operationLocal);

            operation = operationLocal;
        }

        internal void GetClass(
            MI_OperationFlags flags,
            MI_OperationOptions options,
            string namespaceName,
            string className,
            MI_OperationCallbacks callbacks,
            out MI_Operation operation
            )
        {
            if (callbacks != null)
            {
                throw new NotImplementedException();
            }

            MI_Operation operationLocal = MI_Operation.NewDirectPtr();

            this.ft.GetClass(this,
                flags,
                options,
                namespaceName,
                className,
                null,
                operationLocal);

            operation = operationLocal;
        }

        internal void EnumerateClasses(
            MI_OperationFlags flags,
            MI_OperationOptions options,
            string namespaceName,
            string className,
            bool classNamesOnly,
            MI_OperationCallbacks callbacks,
            out MI_Operation operation
            )
        {
            if (callbacks != null)
            {
                throw new NotImplementedException();
            }

            MI_Operation operationLocal = MI_Operation.NewDirectPtr();

            this.ft.EnumerateClasses(this,
                flags,
                options,
                namespaceName,
                className,
                classNamesOnly,
                null,
                operationLocal);

            operation = operationLocal;
        }

        internal void TestConnection(
            MI_OperationFlags flags,
            MI_OperationCallbacks callbacks,
            out MI_Operation operation
            )
        {
            if (callbacks != null)
            {
                throw new NotImplementedException();
            }

            MI_Operation operationLocal = MI_Operation.NewDirectPtr();

            this.ft.TestConnection(this,
                flags,
                null,
                operationLocal);

            operation = operationLocal;
        }
        
        private MI_Session(bool isDirect) : base(isDirect)
        {
        }
        private MI_Session(IntPtr existingPtr) : base(existingPtr)
        {
        }

        internal static MI_Session NewDirectPtr()
        {
            return new MI_Session(true);
        }

        internal static MI_Session NewIndirectPtr()
        {
            return new MI_Session(false);
        }

        internal static MI_Session NewFromDirectPtr(IntPtr ptr)
        {
            return new MI_Session(ptr);
        }

        internal void AssertValidInternalState()
        {
            throw new NotImplementedException();
        }

        internal static MI_Session Null { get { return null; } }

        internal MI_Result Close(
            IntPtr completionContext,
            MI_SessionFT.MI_Session_Close_CompletionCallback completionCallback
            )
        {
            MI_Result resultLocal = this.ft.Close(this,
                completionContext,
                completionCallback);
            return resultLocal;
        }

        internal MI_Result GetApplication(
            MI_Application application
            )
        {
            MI_Result resultLocal = this.ft.GetApplication(this,
                application);
            return resultLocal;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal class MI_SessionFT
        {
            internal MI_Session_Close Close;
            internal MI_Session_GetApplication GetApplication;
            internal MI_Session_GetInstance GetInstance;
            internal MI_Session_ModifyInstance ModifyInstance;
            internal MI_Session_CreateInstance CreateInstance;
            internal MI_Session_DeleteInstance DeleteInstance;
            internal MI_Session_Invoke Invoke;
            internal MI_Session_EnumerateInstances EnumerateInstances;
            internal MI_Session_QueryInstances QueryInstances;
            internal MI_Session_AssociatorInstances AssociatorInstances;
            internal MI_Session_ReferenceInstances ReferenceInstances;
            internal MI_Session_Subscribe Subscribe;
            internal MI_Session_GetClass GetClass;
            internal MI_Session_EnumerateClasses EnumerateClasses;
            internal MI_Session_TestConnection TestConnection;

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Session_Close(
                DirectPtr session,
                IntPtr completionContext,
                MI_SessionFT.MI_Session_Close_CompletionCallback completionCallback
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Session_GetApplication(
                DirectPtr session,
                [In, Out] MI_Application.DirectPtr application
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_GetInstance(
                DirectPtr session,
                MI_OperationFlags flags,
                [In, Out] MI_OperationOptions.DirectPtr options,
                string namespaceName,
                [In, Out] MI_Instance.DirectPtr inboundInstance,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.DirectPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_ModifyInstance(
                DirectPtr session,
                MI_OperationFlags flags,
                [In, Out] MI_OperationOptions.DirectPtr options,
                string namespaceName,
                [In, Out] MI_Instance.DirectPtr inboundInstance,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.DirectPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_CreateInstance(
                DirectPtr session,
                MI_OperationFlags flags,
                [In, Out] MI_OperationOptions.DirectPtr options,
                string namespaceName,
                [In, Out] MI_Instance.DirectPtr inboundInstance,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.DirectPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_DeleteInstance(
                DirectPtr session,
                MI_OperationFlags flags,
                [In, Out] MI_OperationOptions.DirectPtr options,
                string namespaceName,
                [In, Out] MI_Instance.DirectPtr inboundInstance,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.DirectPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_Invoke(
                DirectPtr session,
                MI_OperationFlags flags,
                [In, Out] MI_OperationOptions.DirectPtr options,
                string namespaceName,
                string className,
                string methodName,
                [In, Out] MI_Instance.DirectPtr inboundInstance,
                [In, Out] MI_Instance.DirectPtr inboundProperties,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.DirectPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_EnumerateInstances(
                DirectPtr session,
                MI_OperationFlags flags,
                [In, Out] MI_OperationOptions.DirectPtr options,
                string namespaceName,
                string className,
                [MarshalAs(UnmanagedType.U1)] bool keysOnly,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.DirectPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_QueryInstances(
                DirectPtr session,
                MI_OperationFlags flags,
                [In, Out] MI_OperationOptions.DirectPtr options,
                string namespaceName,
                string queryDialect,
                string queryExpression,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.DirectPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_AssociatorInstances(
                DirectPtr session,
                MI_OperationFlags flags,
                [In, Out] MI_OperationOptions.DirectPtr options,
                string namespaceName,
                [In, Out] MI_Instance.DirectPtr instanceKeys,
                string assocClass,
                string resultClass,
                string role,
                string resultRole,
                [MarshalAs(UnmanagedType.U1)] bool keysOnly,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.DirectPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_ReferenceInstances(
                DirectPtr session,
                MI_OperationFlags flags,
                [In, Out] MI_OperationOptions.DirectPtr options,
                string namespaceName,
                [In, Out] MI_Instance.DirectPtr instanceKeys,
                string resultClass,
                string role,
                [MarshalAs(UnmanagedType.U1)] bool keysOnly,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.DirectPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_Subscribe(
                DirectPtr session,
                MI_OperationFlags flags,
                [In, Out] MI_OperationOptions.DirectPtr options,
                string namespaceName,
                string queryDialect,
                string queryExpression,
                [In, Out] MI_SubscriptionDeliveryOptions.DirectPtr deliverOptions,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.DirectPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_GetClass(
                DirectPtr session,
                MI_OperationFlags flags,
                [In, Out] MI_OperationOptions.DirectPtr options,
                string namespaceName,
                string className,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.DirectPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_EnumerateClasses(
                DirectPtr session,
                MI_OperationFlags flags,
                [In, Out] MI_OperationOptions.DirectPtr options,
                string namespaceName,
                string className,
                [MarshalAs(UnmanagedType.U1)] bool classNamesOnly,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.DirectPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_TestConnection(
                DirectPtr session,
                MI_OperationFlags flags,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.DirectPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention)]
            internal delegate void MI_Session_Close_CompletionCallback(IntPtr callbackContext);
        }
    }
}
