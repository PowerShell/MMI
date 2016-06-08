using System;
using System.Runtime.InteropServices;

namespace Microsoft.Management.Infrastructure.Native
{
    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal class MI_Session
    {
        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal struct MI_SessionPtr
        {
            internal IntPtr ptr;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal struct MI_SessionOutPtr
        {
            internal IntPtr ptr;
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

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        private struct MI_SessionMembers
        {
            internal UInt64 reserved1;
            internal IntPtr reserved2;
            internal IntPtr ft;
        }

        // Marshal implements these with Reflection - pay this hit only once
        private static int MI_SessionMembersFTOffset = (int)Marshal.OffsetOf<MI_SessionMembers>("ft");

        private static int MI_SessionMembersSize = Marshal.SizeOf<MI_SessionMembers>();

        private MI_SessionPtr ptr;
        private bool isDirect;
        private Lazy<MI_SessionFT> mft;

        ~MI_Session()
        {
            Marshal.FreeHGlobal(this.ptr.ptr);
        }

        private MI_Session(bool isDirect)
        {
            this.isDirect = isDirect;
            this.mft = new Lazy<MI_SessionFT>(this.MarshalFT);

            var necessarySize = this.isDirect ? MI_SessionMembersSize : NativeMethods.IntPtrSize;
            this.ptr.ptr = Marshal.AllocHGlobal(necessarySize);

            unsafe
            {
                NativeMethods.memset((byte*)this.ptr.ptr, 0, (uint)necessarySize);
            }
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
            var res = new MI_Session(false);
            Marshal.WriteIntPtr(res.ptr.ptr, ptr);
            return res;
        }

        internal void AssertValidInternalState()
        {
            throw new NotImplementedException();
        }

        public static implicit operator MI_SessionPtr(MI_Session instance)
        {
            // If the indirect pointer is zero then the object has not
            // been initialized and it is not valid to refer to its data
            if (instance != null && instance.Ptr == IntPtr.Zero)
            {
                throw new InvalidCastException();
            }

            return new MI_SessionPtr() { ptr = instance == null ? IntPtr.Zero : instance.Ptr };
        }

        public static implicit operator MI_SessionOutPtr(MI_Session instance)
        {
            // We are not currently supporting the ability to get the address
            // of our direct pointer, though it is technically feasible
            if (instance != null && instance.isDirect)
            {
                throw new InvalidCastException();
            }

            return new MI_SessionOutPtr() { ptr = instance == null ? IntPtr.Zero : instance.ptr.ptr };
        }

        internal static MI_Session Null { get { return null; } }
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

        internal MI_Result Close(
            IntPtr completionContext,
            NativeMethods.MI_Session_Close_CompletionCallback completionCallback
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

        private MI_SessionFT ft { get { return this.mft.Value; } }

        private MI_SessionFT MarshalFT()
        {
            return MI_FunctionTableCache.GetFTAsOffsetFromPtr<MI_SessionFT>(this.Ptr, MI_Session.MI_SessionMembersFTOffset);
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
                MI_SessionPtr session,
                IntPtr completionContext,
                NativeMethods.MI_Session_Close_CompletionCallback completionCallback
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Session_GetApplication(
                MI_SessionPtr session,
                [In, Out] MI_Application.MI_ApplicationPtr application
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_GetInstance(
                MI_SessionPtr session,
                MI_OperationFlags flags,
                [In, Out] MI_OperationOptions.MI_OperationOptionsPtr options,
                string namespaceName,
                [In, Out] MI_Instance.MI_InstancePtr inboundInstance,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.MI_OperationPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_ModifyInstance(
                MI_SessionPtr session,
                MI_OperationFlags flags,
                [In, Out] MI_OperationOptions.MI_OperationOptionsPtr options,
                string namespaceName,
                [In, Out] MI_Instance.MI_InstancePtr inboundInstance,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.MI_OperationPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_CreateInstance(
                MI_SessionPtr session,
                MI_OperationFlags flags,
                [In, Out] MI_OperationOptions.MI_OperationOptionsPtr options,
                string namespaceName,
                [In, Out] MI_Instance.MI_InstancePtr inboundInstance,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.MI_OperationPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_DeleteInstance(
                MI_SessionPtr session,
                MI_OperationFlags flags,
                [In, Out] MI_OperationOptions.MI_OperationOptionsPtr options,
                string namespaceName,
                [In, Out] MI_Instance.MI_InstancePtr inboundInstance,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.MI_OperationPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_Invoke(
                MI_SessionPtr session,
                MI_OperationFlags flags,
                [In, Out] MI_OperationOptions.MI_OperationOptionsPtr options,
                string namespaceName,
                string className,
                string methodName,
                [In, Out] MI_Instance.MI_InstancePtr inboundInstance,
                [In, Out] MI_Instance.MI_InstancePtr inboundProperties,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.MI_OperationPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_EnumerateInstances(
                MI_SessionPtr session,
                MI_OperationFlags flags,
                [In, Out] MI_OperationOptions.MI_OperationOptionsPtr options,
                string namespaceName,
                string className,
                [MarshalAs(UnmanagedType.U1)] bool keysOnly,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.MI_OperationPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_QueryInstances(
                MI_SessionPtr session,
                MI_OperationFlags flags,
                [In, Out] MI_OperationOptions.MI_OperationOptionsPtr options,
                string namespaceName,
                string queryDialect,
                string queryExpression,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.MI_OperationPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_AssociatorInstances(
                MI_SessionPtr session,
                MI_OperationFlags flags,
                [In, Out] MI_OperationOptions.MI_OperationOptionsPtr options,
                string namespaceName,
                [In, Out] MI_Instance.MI_InstancePtr instanceKeys,
                string assocClass,
                string resultClass,
                string role,
                string resultRole,
                [MarshalAs(UnmanagedType.U1)] bool keysOnly,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.MI_OperationPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_ReferenceInstances(
                MI_SessionPtr session,
                MI_OperationFlags flags,
                [In, Out] MI_OperationOptions.MI_OperationOptionsPtr options,
                string namespaceName,
                [In, Out] MI_Instance.MI_InstancePtr instanceKeys,
                string resultClass,
                string role,
                [MarshalAs(UnmanagedType.U1)] bool keysOnly,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.MI_OperationPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_Subscribe(
                MI_SessionPtr session,
                MI_OperationFlags flags,
                [In, Out] MI_OperationOptions.MI_OperationOptionsPtr options,
                string namespaceName,
                string queryDialect,
                string queryExpression,
                [In, Out] MI_SubscriptionDeliveryOptions.MI_SubscriptionDeliveryOptionsPtr deliverOptions,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.MI_OperationPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_GetClass(
                MI_SessionPtr session,
                MI_OperationFlags flags,
                [In, Out] MI_OperationOptions.MI_OperationOptionsPtr options,
                string namespaceName,
                string className,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.MI_OperationPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_EnumerateClasses(
                MI_SessionPtr session,
                MI_OperationFlags flags,
                [In, Out] MI_OperationOptions.MI_OperationOptionsPtr options,
                string namespaceName,
                string className,
                [MarshalAs(UnmanagedType.U1)] bool classNamesOnly,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.MI_OperationPtr operation
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate void MI_Session_TestConnection(
                MI_SessionPtr session,
                MI_OperationFlags flags,
                MI_OperationCallbacks.MI_OperationCallbacksNative callbacks,
                [In, Out] MI_Operation.MI_OperationPtr operation
                );
        }
    }
}