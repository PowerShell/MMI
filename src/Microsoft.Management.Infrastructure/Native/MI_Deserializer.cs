using System;
using System.Runtime.InteropServices;

namespace Microsoft.Management.Infrastructure.Native
{
    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal struct MI_DeserializerPtr
    {
        internal IntPtr ptr;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal struct MI_DeserializerOutPtr
    {
        internal IntPtr ptr;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal partial class MI_Deserializer
    {
        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        private struct MI_DeserializerMembers
        {
            internal UInt64 reserved1;
            internal IntPtr reserved2;
        }

        // Marshal implements these with Reflection - pay this hit only once
        private static int MI_DeserializerFTOffset = (int)Marshal.OffsetOf<MI_Deserializer>("ft");

        private static int MI_DeserializerMembersSize = Marshal.SizeOf<MI_DeserializerMembers>();

        private MI_DeserializerPtr ptr;
        private bool isDirect;
        private Lazy<MI_DeserializerFT> mft;

        ~MI_Deserializer()
        {
            Marshal.FreeHGlobal(this.ptr.ptr);
            this.mft = new Lazy<MI_DeserializerFT>(this.MarshalFT);
        }

        private MI_Deserializer(bool isDirect)
        {
            this.isDirect = isDirect;

            var necessarySize = this.isDirect ? MI_DeserializerMembersSize : NativeMethods.IntPtrSize;
            this.ptr.ptr = Marshal.AllocHGlobal(necessarySize);

            unsafe
            {
                NativeMethods.memset((byte*)this.ptr.ptr, 0, (uint)necessarySize);
            }
        }

        internal static MI_Deserializer NewDirectPtr()
        {
            return new MI_Deserializer(true);
        }

        internal static MI_Deserializer NewIndirectPtr()
        {
            return new MI_Deserializer(false);
        }

        internal static MI_Deserializer NewFromDirectPtr(IntPtr ptr)
        {
            var res = new MI_Deserializer(false);
            Marshal.WriteIntPtr(res.ptr.ptr, ptr);
            return res;
        }

        public static implicit operator MI_DeserializerPtr(MI_Deserializer instance)
        {
            // If the indirect pointer is zero then the object has not
            // been initialized and it is not valid to refer to its data
            if(instance != null && instance.Ptr == IntPtr.Zero)
            {
                throw new InvalidCastException();
            }

            return new MI_DeserializerPtr() { ptr = instance == null ? IntPtr.Zero : instance.Ptr };
        }

        public static implicit operator MI_DeserializerOutPtr(MI_Deserializer instance)
        {
            // We are not currently supporting the ability to get the address
            // of our direct pointer, though it is technically feasible 
            if(instance != null && instance.isDirect)
            {
                throw new InvalidCastException();
            }

            return new MI_DeserializerOutPtr() { ptr = instance == null ? IntPtr.Zero : instance.ptr.ptr };
        }

        internal static MI_Deserializer Null { get { return null; } }
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

        internal MI_Result DeserializeClass(
            UInt32 flags,
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
            throw new NotImplementedException();
            //MI_Class classObjectLocal = MI_Class.NewIndirectPtr();
            //MI_Instance cimErrorDetailsLocal = MI_Instance();

            //MI_Result resultLocal = this.ft.DeserializeClass(this,
            //    flags,
            //    serializedBuffer,
            //    serializedBufferLength,
            //    parentClass,
            //    serverName,
            //    namespaceName,
            //    classObjectNeeded,
            //    classObjectNeededContext,
            //    out serializedBufferRead,
            //    classObjectLocal,
            //    cimErrorDetailsLocal);

            //classObjectNeeded = classObjectNeededLocal;
            //classObject = classObjectLocal;
            //cimErrorDetails = cimErrorDetailsLocal;
            //return resultLocal;
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

            MI_Result resultLocal = this.ft.Class_GetClassName(this,
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

            MI_Result resultLocal = this.ft.Class_GetParentClassName(this,
                serializedBuffer,
                serializedBufferLength,
                parentClassName,
                out parentClassNameLength,
                cimErrorDetailsLocal);

            cimErrorDetails = cimErrorDetailsLocal;
            return resultLocal;
        }

        internal MI_Result DeserializeInstance(
            UInt32 flags,
            IntPtr serializedBuffer,
            UInt32 serializedBufferLength,
            out MI_Class classObjects,
            UInt32 numberClassObjects,
            IntPtr classObjectNeeded,
            IntPtr classObjectNeededContext,
            out UInt32 serializedBufferRead,
            out MI_Instance instanceObject,
            out MI_Instance cimErrorDetails
            )
        {
            throw new NotImplementedException();
            //MI_Class classObjectsLocal = MI_Class.NewIndirectPtr();
            //MI_Instance instanceObjectLocal = MI_Instance.NewIndirectPtr();
            //MI_Instance cimErrorDetailsLocal = MI_Instance.NewIndirectPtr();

            //MI_Result resultLocal = this.ft.DeserializeInstance(this,
            //    flags,
            //    serializedBuffer,
            //    serializedBufferLength,
            //    classObjectsLocal,
            //    numberClassObjects,
            //    classObjectNeeded,
            //    classObjectNeededContext,
            //    out serializedBufferRead,
            //    instanceObjectLocal,
            //    cimErrorDetailsLocal);

            //classObjects = classObjectsLocal;
            //classObjectNeeded = classObjectNeededLocal;
            //instanceObject = instanceObjectLocal;
            //cimErrorDetails = cimErrorDetailsLocal;
            //return resultLocal;
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

            MI_Result resultLocal = this.ft.Instance_GetClassName(this,
                serializedBuffer,
                serializedBufferLength,
                className,
                out classNameLength,
                cimErrorDetailsLocal);

            cimErrorDetails = cimErrorDetailsLocal;
            return resultLocal;
        }


        private MI_DeserializerFT ft { get { return this.mft.Value; } }

        private MI_DeserializerFT MarshalFT()
        {
            MI_DeserializerFT res = new MI_DeserializerFT();
            IntPtr ftPtr = IntPtr.Zero;
            unsafe
            {
                // Just as easily could be implemented with Marshal
                // but that would copy more than the one pointer we need
                IntPtr structurePtr = this.Ptr;
                if (structurePtr == IntPtr.Zero)
                {
                    throw new InvalidOperationException();
                }

                ftPtr = *((IntPtr*)((byte*)structurePtr + MI_DeserializerFTOffset));
            }

            if (ftPtr == IntPtr.Zero)
            {
                throw new InvalidOperationException();
            }

            // No apparent way to implement this in an unsafe block
            Marshal.PtrToStructure(ftPtr, res);
            return res;
        }

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
                MI_DeserializerPtr deserializer
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Deserializer_DeserializeClass(
                MI_DeserializerPtr deserializer,
                UInt32 flags,
                IntPtr serializedBuffer,
                UInt32 serializedBufferLength,
                [In, Out] MI_ClassPtr parentClass,
                string serverName,
                string namespaceName,
                IntPtr classObjectNeeded,
                IntPtr classObjectNeededContext,
                out UInt32 serializedBufferRead,
                [In, Out] MI_ClassOutPtr classObject,
                [In, Out] MI_InstanceOutPtr cimErrorDetails
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Deserializer_Class_GetClassName(
                MI_DeserializerPtr deserializer,
                IntPtr serializedBuffer,
                UInt32 serializedBufferLength,
                string className,
                out UInt32 classNameLength,
                [In, Out] MI_InstanceOutPtr cimErrorDetails
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Deserializer_Class_GetParentClassName(
                MI_DeserializerPtr deserializer,
                IntPtr serializedBuffer,
                UInt32 serializedBufferLength,
                string parentClassName,
                out UInt32 parentClassNameLength,
                [In, Out] MI_InstanceOutPtr cimErrorDetails
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Deserializer_DeserializeInstance(
                MI_DeserializerPtr deserializer,
                UInt32 flags,
                IntPtr serializedBuffer,
                UInt32 serializedBufferLength,
                [In, Out] MI_ClassOutPtr classObjects,
                UInt32 numberClassObjects,
                IntPtr classObjectNeeded,
                IntPtr classObjectNeededContext,
                out UInt32 serializedBufferRead,
                [In, Out] MI_InstanceOutPtr instanceObject,
                [In, Out] MI_InstanceOutPtr cimErrorDetails
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Deserializer_Instance_GetClassName(
                MI_DeserializerPtr deserializer,
                IntPtr serializedBuffer,
                UInt32 serializedBufferLength,
                string className,
                out UInt32 classNameLength,
                [In, Out] MI_InstanceOutPtr cimErrorDetails
                );
        }
    }
}