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
    internal class MI_Deserializer : MI_NativeObjectWithFT<MI_Deserializer.MI_MOFDeserializerFT>
    {
        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        private struct MI_DeserializerMembers
        {
            internal UInt64 reserved1;
            internal IntPtr reserved2;
        }

        // Marshal implements these with Reflection - pay this hit only once
        internal static int MI_DeserializerMembersReserved2Offset = (int)Marshal.OffsetOf<MI_Deserializer.MI_DeserializerMembers>("reserved2");
        private static int MI_DeserializerMembersSize = Marshal.SizeOf<MI_DeserializerMembers>();
        
        private string format;
        private MI_Deserializer(string format) : base(true)
        {
            this.format = format;
        }

        private MI_Deserializer(string format, Func<MI_MOFDeserializerFT> mftThunk) : base(true, mftThunk)
        {
            this.format = format;
        }

        internal static MI_Deserializer NewDirectPtr(string format)
        {
            if (MI_SerializationFormat.MOF.Equals(format, StringComparison.OrdinalIgnoreCase))
            {
                // Nothing, see fallthrough later
            }
            else if (MI_SerializationFormat.XML.Equals(format, StringComparison.Ordinal))
            {
#if !_LINUX
                MI_MOFDeserializerFT tmp = new MI_MOFDeserializerFT();
                tmp.deserializerFT = MI_SerializationFTHelpers.XMLDeserializationFT;
                return new MI_Deserializer(format, () => tmp);
#endif
            }
            else
            {
                throw new NotImplementedException();
            }

            return new MI_Deserializer(format);
        }

        internal static MI_Deserializer Null { get { return null; } }

        protected override int FunctionTableOffset { get { return MI_DeserializerMembersReserved2Offset; } }

        protected override int MembersSize { get { return MI_DeserializerMembersSize; } }

        internal MI_Result Close()
        {
            return this.commonFT.Close(this);
        }

        internal MI_Result DeserializeClass(
            MI_SerializerFlags flags,
            IntPtr serializedBuffer,
            UInt32 serializedBufferLength,
            MI_Class parentClass,
            string serverName,
            string namespaceName,
            IntPtr classObjectNeeded,
            IntPtr classObjectNeededContext,
            out UInt32 serializedBufferRead,
            out MI_Class classObject,
            out MI_Instance cimErrorDetails
            )
        {
            MI_Class classObjectLocal = MI_Class.NewIndirectPtr();
            MI_Instance cimErrorDetailsLocal = MI_Instance.NewIndirectPtr();

            MI_Result resultLocal = this.commonFT.DeserializeClass(this,
                flags,
                serializedBuffer,
                serializedBufferLength,
                parentClass,
                serverName,
                namespaceName,
                classObjectNeeded,
                classObjectNeededContext,
                out serializedBufferRead,
                classObjectLocal,
                cimErrorDetailsLocal);

            classObject = classObjectLocal;
            cimErrorDetails = cimErrorDetailsLocal;
            return resultLocal;
        }

        internal MI_Result DeserializeClass(
            MI_SerializerFlags flags,
            byte[] serializedBuffer,
            MI_Class parentClass,
            string serverName,
            string namespaceName,
            IntPtr classObjectNeeded,
            IntPtr classObjectNeededContext,
            out UInt32 serializedBufferRead,
            out MI_Class classObject,
            out MI_Instance cimErrorDetails
            )
        {
            if (serializedBuffer == null || serializedBuffer.Length == 0)
            {
                throw new InvalidOperationException();
            }

            IntPtr clientBuffer = Marshal.AllocHGlobal(serializedBuffer.Length);
            try
            {
                Marshal.Copy(serializedBuffer, 0, clientBuffer, serializedBuffer.Length);
                return this.DeserializeClass(flags,
                    clientBuffer,
                    (uint)serializedBuffer.Length,
                    parentClass,
                    serverName,
                    namespaceName,
                    classObjectNeeded,
                    classObjectNeededContext,
                    out serializedBufferRead,
                    out classObject,
                    out cimErrorDetails);
            }
            finally
            {
                Marshal.FreeHGlobal(clientBuffer);
            }
        }

        internal MI_Result Class_GetClassName(
            IntPtr serializedBuffer,
            UInt32 serializedBufferLength,
            string className,
            out UInt32 classNameLength,
            out MI_Instance cimErrorDetails
            )
        {
            MI_Instance cimErrorDetailsLocal = MI_Instance.NewIndirectPtr();

            MI_Result resultLocal = this.commonFT.Class_GetClassName(this,
                serializedBuffer,
                serializedBufferLength,
                className,
                out classNameLength,
                cimErrorDetailsLocal);

            cimErrorDetails = cimErrorDetailsLocal;
            return resultLocal;
        }

        internal MI_Result Class_GetParentClassName(
            IntPtr serializedBuffer,
            UInt32 serializedBufferLength,
            string parentClassName,
            out UInt32 parentClassNameLength,
            out MI_Instance cimErrorDetails
            )
        {
            MI_Instance cimErrorDetailsLocal = MI_Instance.NewIndirectPtr();

            MI_Result resultLocal = this.commonFT.Class_GetParentClassName(this,
                serializedBuffer,
                serializedBufferLength,
                parentClassName,
                out parentClassNameLength,
                cimErrorDetailsLocal);

            cimErrorDetails = cimErrorDetailsLocal;
            return resultLocal;
        }

        internal MI_Result DeserializeInstance(
            MI_SerializerFlags flags,
            byte[] serializedBuffer,
            MI_Class[] classObjects,
            MI_Deserializer_ClassObjectNeeded classObjectNeeded,
            out UInt32 serializedBufferRead,
            out MI_Instance instanceObject,
            out MI_Instance cimErrorDetails
            )
        {
            if (serializedBuffer == null || serializedBuffer.Length == 0)
            {
                throw new InvalidOperationException();
            }

            MI_Deserializer_ClassObjectNeededNative nativeCallback = MI_DeserializerCallbacks.GetNativeClassObjectNeededCallback(this.format, classObjectNeeded);

            IntPtr clientBuffer = Marshal.AllocHGlobal(serializedBuffer.Length);
            try
            {
                Marshal.Copy(serializedBuffer, 0, clientBuffer, serializedBuffer.Length);
                return this.DeserializeInstance(flags,
                    clientBuffer,
                    (UInt32)(serializedBuffer.Length),
                    classObjects,
                    nativeCallback,
                    IntPtr.Zero,
                    out serializedBufferRead,
                    out instanceObject,
                    out cimErrorDetails);
            }
            finally
            {
                Marshal.FreeHGlobal(clientBuffer);
            }
        }

        internal MI_Result DeserializeClassArray(
                MI_SerializerFlags flags,
                MI_OperationOptions options,
                MI_DeserializerCallbacks deserializerCallbacks,
                IntPtr serializedBuffer,
                UInt32 serializedBufferLength,
                MI_Class[] classDefinitions,
                string serverName,
                string namespaceName,
                out UInt32 serializedBufferRead,
                out MI_ExtendedArray classes,
                out MI_Instance cimErrorDetails)
        {
            if (!MI_SerializationFormat.MOF.Equals(this.format, StringComparison.OrdinalIgnoreCase))
            {
                throw new NotImplementedException();
            }

            MI_Class.ArrayPtr classPtrs = MI_Class.GetPointerArray(classDefinitions);
            MI_Instance cimErrorDetailsLocal = MI_Instance.NewIndirectPtr();
            MI_ExtendedArray classesLocal = MI_ExtendedArray.NewIndirectPtr();
            MI_ExtendedArray classDetailsArray = MI_ExtendedArray.NewDirectPtr(classPtrs.Ptrs);
            MI_DeserializerCallbacks.MI_DeserializerCallbacksNative nativeCallbacks = deserializerCallbacks.GetNativeCallbacks(this.format);

            classes = null;

            var resLocal = this.ft.DeserializeClassArray_MOF(
                this,
                flags,
                options,
                nativeCallbacks,
                serializedBuffer,
                serializedBufferLength,
                classDetailsArray,
                serverName,
                namespaceName,
                out serializedBufferRead,
                classesLocal,
                cimErrorDetailsLocal);

            cimErrorDetails = cimErrorDetailsLocal;
            classes = classesLocal;

            return resLocal;
        }

        internal MI_Result DeserializeClassArray(
                MI_SerializerFlags flags,
                MI_OperationOptions options,
                MI_DeserializerCallbacks deserializerCallbacks,
                byte[] serializedBuffer,
                MI_Class[] classDefinitions,
                string serverName,
                string namespaceName,
                out UInt32 serializedBufferRead,
                out MI_ExtendedArray classes,
                out MI_Instance cimErrorDetails)
        {
            if (!MI_SerializationFormat.MOF.Equals(this.format, StringComparison.OrdinalIgnoreCase))
            {
                throw new NotImplementedException();
            }

            if (serializedBuffer == null || serializedBuffer.Length == 0)
            {
                throw new InvalidOperationException();
            }

            IntPtr clientBuffer = Marshal.AllocHGlobal(serializedBuffer.Length);
            try
            {
                Marshal.Copy(serializedBuffer, 0, clientBuffer, serializedBuffer.Length);
                return this.DeserializeClassArray(
                    flags,
                    options,
                    deserializerCallbacks,
                    clientBuffer,
                    (uint)serializedBuffer.Length,
                    classDefinitions,
                    serverName,
                    namespaceName,
                    out serializedBufferRead,
                    out classes,
                    out cimErrorDetails);
            }
            finally
            {
                Marshal.FreeHGlobal(clientBuffer);
            }
        }

        internal MI_Result DeserializeInstanceArray(
                MI_SerializerFlags flags,
                MI_OperationOptions options,
                MI_DeserializerCallbacks deserializerCallbacks,
                IntPtr serializedBuffer,
                UInt32 serializedBufferLength,
                MI_Class[] classDefinitions,
                out UInt32 serializedBufferRead,
                out MI_ExtendedArray instances,
                out MI_Instance cimErrorDetails)
        {
            if (!MI_SerializationFormat.MOF.Equals(this.format, StringComparison.OrdinalIgnoreCase))
            {
                throw new NotImplementedException();
            }

            MI_Class.ArrayPtr classPtrs = MI_Class.GetPointerArray(classDefinitions);
            MI_Instance cimErrorDetailsLocal = MI_Instance.NewIndirectPtr();
            MI_ExtendedArray resultingArray = MI_ExtendedArray.NewIndirectPtr();
            MI_ExtendedArray classDetailsArray = MI_ExtendedArray.NewDirectPtr(classPtrs.Ptrs);
            MI_DeserializerCallbacks.MI_DeserializerCallbacksNative nativeCallbacks = deserializerCallbacks.GetNativeCallbacks(this.format);

            instances = null;
            
            var resLocal = this.ft.DeserializeInstanceArray_MOF(
                this,
                flags,
                options,
                nativeCallbacks,
                serializedBuffer,
                serializedBufferLength,
                classDetailsArray,
                out serializedBufferRead,
                resultingArray,
                cimErrorDetailsLocal);

            cimErrorDetails = cimErrorDetailsLocal;
            instances = resultingArray;

            return resLocal;
        }

        internal MI_Result DeserializeInstanceArray(
                MI_SerializerFlags flags,
                MI_OperationOptions options,
                MI_DeserializerCallbacks deserializerCallbacks,
                byte[] serializedBuffer,
                MI_Class[] classDefinitions,
                out UInt32 serializedBufferRead,
                out MI_ExtendedArray instances,
                out MI_Instance cimErrorDetails)
        {
            if (!MI_SerializationFormat.MOF.Equals(this.format, StringComparison.OrdinalIgnoreCase))
            {
                throw new NotImplementedException();
            }

            if (serializedBuffer == null || serializedBuffer.Length == 0)
            {
                throw new InvalidOperationException();
            }

            IntPtr clientBuffer = Marshal.AllocHGlobal(serializedBuffer.Length);
            try
            {
                Marshal.Copy(serializedBuffer, 0, clientBuffer, serializedBuffer.Length);
                instances = null;

                return this.DeserializeInstanceArray(
                    flags,
                    options,
                    deserializerCallbacks,
                    clientBuffer,
                    (uint)serializedBuffer.Length,
                    classDefinitions,
                    out serializedBufferRead,
                    out instances,
                    out cimErrorDetails);
            }
            finally
            {
                Marshal.FreeHGlobal(clientBuffer);
            }
        }

        internal MI_Result Instance_GetClassName(
            IntPtr serializedBuffer,
            UInt32 serializedBufferLength,
            string className,
            out UInt32 classNameLength,
            out MI_Instance cimErrorDetails
            )
        {
            MI_Instance cimErrorDetailsLocal = MI_Instance.NewIndirectPtr();

            MI_Result resultLocal = this.commonFT.Instance_GetClassName(this,
                serializedBuffer,
                serializedBufferLength,
                className,
                out classNameLength,
                cimErrorDetailsLocal);

            cimErrorDetails = cimErrorDetailsLocal;
            return resultLocal;
        }

        private MI_Result DeserializeInstance(
            MI_SerializerFlags flags,
            IntPtr serializedBuffer,
            UInt32 serializedBufferLength,
            MI_Class[] classObjects,
            MI_Deserializer_ClassObjectNeededNative classObjectNeeded,
            IntPtr classObjectNeededContext,
            out UInt32 serializedBufferRead,
            out MI_Instance instanceObject,
            out MI_Instance cimErrorDetails
            )
        {
            if (classObjectNeededContext != IntPtr.Zero)
            {
                throw new NotImplementedException();
            }

            MI_Instance instanceObjectLocal = MI_Instance.NewIndirectPtr();
            MI_Instance cimErrorDetailsLocal = MI_Instance.NewIndirectPtr();
            MI_Class.ArrayPtr classArrayPtr = MI_Class.GetPointerArray(classObjects);

            MI_Result resultLocal = this.commonFT.DeserializeInstance(this,
                flags,
                serializedBuffer,
                serializedBufferLength,
                classArrayPtr.Ptrs,
                (uint)classObjects.Length,
                classObjectNeeded,
                IntPtr.Zero,
                out serializedBufferRead,
                instanceObjectLocal,
                cimErrorDetailsLocal);

            instanceObject = instanceObjectLocal;
            cimErrorDetails = cimErrorDetailsLocal;
            return resultLocal;
        }

        [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal delegate MI_Result MI_Deserializer_ClassObjectNeeded(
            string serverName,
            string namespaceName,
            string className,
            out MI_Class requestedClassObject
            );

        [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal delegate MI_Result MI_Deserializer_ClassObjectNeededNative(
            IntPtr context,
            IntPtr serverName,
            IntPtr namespaceName,
            IntPtr className,
            IntPtr requestedClassObject
            );

        private MI_DeserializerFT commonFT { get { return this.mft.Value.deserializerFT; } }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal class MI_DeserializerFT
        {
            internal MI_Deserializer_Close Close;
            internal MI_Deserializer_DeserializeClass DeserializeClass;
            internal MI_Deserializer_Class_GetClassName Class_GetClassName;
            internal MI_Deserializer_Class_GetParentClassName Class_GetParentClassName;
            internal MI_Deserializer_DeserializeInstance DeserializeInstance;
            internal MI_Deserializer_Instance_GetClassName Instance_GetClassName;

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Deserializer_Close(
                DirectPtr deserializer
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Deserializer_DeserializeClass(
                DirectPtr deserializer,
                MI_SerializerFlags flags,
                IntPtr serializedBuffer,
                UInt32 serializedBufferLength,
                [In, Out] MI_Class.DirectPtr parentClass,
                string serverName,
                string namespaceName,
                IntPtr classObjectNeeded,
                IntPtr classObjectNeededContext,
                out UInt32 serializedBufferRead,
                [In, Out] MI_Class.IndirectPtr classObject,
                [In, Out] MI_Instance.IndirectPtr cimErrorDetails
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Deserializer_Class_GetClassName(
                DirectPtr deserializer,
                IntPtr serializedBuffer,
                UInt32 serializedBufferLength,
                string className,
                out UInt32 classNameLength,
                [In, Out] MI_Instance.IndirectPtr cimErrorDetails
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Deserializer_Class_GetParentClassName(
                DirectPtr deserializer,
                IntPtr serializedBuffer,
                UInt32 serializedBufferLength,
                string parentClassName,
                out UInt32 parentClassNameLength,
                [In, Out] MI_Instance.IndirectPtr cimErrorDetails
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Deserializer_DeserializeInstance(
                DirectPtr deserializer,
                MI_SerializerFlags flags,
                IntPtr serializedBuffer,
                UInt32 serializedBufferLength,
                IntPtr[] classObjects,
                UInt32 numberClassObjects,
                MI_Deserializer_ClassObjectNeededNative classObjectNeeded,
                IntPtr classObjectNeededContext,
                out UInt32 serializedBufferRead,
                [In, Out] MI_Instance.IndirectPtr instanceObject,
                [In, Out] MI_Instance.IndirectPtr cimErrorDetails
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Deserializer_Instance_GetClassName(
                DirectPtr deserializer,
                IntPtr serializedBuffer,
                UInt32 serializedBufferLength,
                string className,
                out UInt32 classNameLength,
                [In, Out] MI_Instance.IndirectPtr cimErrorDetails
                );
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal class MI_MOFDeserializerFT
        {
            internal MI_DeserializerFT deserializerFT;
            internal MI_Deserializer_DeserializeClassArray_MOF DeserializeClassArray_MOF;
            internal MI_Deserializer_DeserializeInstanceArray_MOF DeserializeInstanceArray_MOF;

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Deserializer_DeserializeClassArray_MOF(
                DirectPtr deserializer,
                MI_SerializerFlags flags,
                MI_OperationOptions.DirectPtr options,
                MI_DeserializerCallbacks.MI_DeserializerCallbacksNative deserializerCallbacks,
                IntPtr serializedBuffer,
                UInt32 serializedBufferLength,
                MI_ExtendedArray.DirectPtr classes,
                string serverName,
                string namespaceName,
                out UInt32 serializedBufferRead,
                [In, Out] MI_ExtendedArray.IndirectPtr resultingArray,
                [In, Out] MI_Instance.IndirectPtr cimErrorDetails
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Deserializer_DeserializeInstanceArray_MOF(
                DirectPtr deserializer,
                MI_SerializerFlags flags,
                MI_OperationOptions.DirectPtr options,
                MI_DeserializerCallbacks.MI_DeserializerCallbacksNative deserializerCallbacks,
                IntPtr serializedBuffer,
                UInt32 serializedBufferLength,
                MI_ExtendedArray.DirectPtr classes,
                out UInt32 serializedBufferRead,
                [In, Out] MI_ExtendedArray.IndirectPtr resultingArray,
                [In, Out] MI_Instance.IndirectPtr cimErrorDetails
                );
        }
    }
}
