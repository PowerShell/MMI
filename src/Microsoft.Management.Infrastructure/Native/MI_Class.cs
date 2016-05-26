using System;
using System.Runtime.InteropServices;

namespace Microsoft.Management.Infrastructure.Native
{
    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal struct MI_ClassPtr
    {
        internal IntPtr ptr;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal struct MI_ClassOutPtr
    {
        internal IntPtr ptr;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal struct MI_ClassArrayPtr
    {
        internal IntPtr[] ptr;

        public static implicit operator MI_ClassArrayPtr(MI_Class[] classes)
        {
            if (classes == null)
            {
                throw new InvalidCastException();
            }

            IntPtr[] ptrs = new IntPtr[classes.Length];
            for (int i = 0; i < classes.Length; i++)
            {
                ptrs[i] = classes[i].Ptr;
            }

            return new MI_ClassArrayPtr() { ptr = ptrs };
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
    internal class MI_Class
    {
        internal MI_Result GetElement(
            string name,
            out MI_Value value,
            out bool valueExists,
            out MI_Type type,
            out string referenceClass,
            out MI_QualifierSet qualifierSet,
            out MI_Flags flags,
            out UInt32 index
            )
        {
            MI_String referenceClassLocal = MI_String.NewIndirectPtr();
            MI_Value valueLocal = new MI_Value();
            MI_QualifierSet qualifierSetLocal = MI_QualifierSet.NewDirectPtr();

            MI_Result resultLocal = this.ft.GetElement(this,
                name,
                valueLocal,
                out valueExists,
                out type,
                referenceClassLocal,
                qualifierSetLocal,
                out flags,
                out index);

            referenceClass = referenceClassLocal.Value;
            value = valueLocal;
            qualifierSet = qualifierSetLocal;
            return resultLocal;
        }

        internal MI_Result GetElementAt(
            UInt32 index,
            out string name,
            out MI_Value value,
            out bool valueExists,
            out MI_Type type,
            out string referenceClass,
            out MI_QualifierSet qualifierSet,
            out MI_Flags flags
            )
        {
            MI_String nameLocal = MI_String.NewIndirectPtr();
            MI_String referenceClassLocal = MI_String.NewIndirectPtr();
            MI_Value valueLocal = new MI_Value();
            MI_QualifierSet qualifierSetLocal = MI_QualifierSet.NewDirectPtr();

            MI_Result resultLocal = this.ft.GetElementAt(this,
                index,
                nameLocal,
                valueLocal,
                out valueExists,
                out type,
                referenceClassLocal,
                qualifierSetLocal,
                out flags);

            name = nameLocal.Value;
            referenceClass = referenceClassLocal.Value;
            value = valueLocal;
            qualifierSet = qualifierSetLocal;
            return resultLocal;
        }

        internal MI_Result GetClassQualifierSet(
            out MI_QualifierSet qualifierSet
            )
        {
            MI_QualifierSet qualifierSetLocal = MI_QualifierSet.NewDirectPtr();
            MI_Result resultLocal = this.ft.GetClassQualifierSet(this,
                qualifierSetLocal);

            qualifierSet = qualifierSetLocal;
            return resultLocal;
        }

        internal MI_Result GetMethod(
            string name,
            out MI_QualifierSet qualifierSet,
            out MI_ParameterSet parameterSet,
            out UInt32 index
            )
        {
            MI_QualifierSet qualifierSetLocal = MI_QualifierSet.NewDirectPtr();
            MI_ParameterSet parameterSetLocal = MI_ParameterSet.NewDirectPtr();
            MI_Result resultLocal = this.ft.GetMethod(this,
                name,
                qualifierSetLocal,
                parameterSetLocal,
                out index);

            qualifierSet = qualifierSetLocal;
            parameterSet = parameterSetLocal;
            return resultLocal;
        }

        internal MI_Result GetMethodAt(
            UInt32 index,
            out string name,
            out MI_QualifierSet qualifierSet,
            out MI_ParameterSet parameterSet
            )
        {
            MI_String nameLocal = MI_String.NewIndirectPtr();
            MI_QualifierSet qualifierSetLocal = MI_QualifierSet.NewDirectPtr();
            MI_ParameterSet parameterSetLocal = MI_ParameterSet.NewDirectPtr();

            MI_Result resultLocal = this.ft.GetMethodAt(this,
                index,
                nameLocal,
                qualifierSetLocal,
                parameterSetLocal);

            name = nameLocal.Value;
            qualifierSet = qualifierSetLocal;
            parameterSet = parameterSetLocal;
            return resultLocal;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        private struct MI_ClassMembers
        {
            internal IntPtr ft;
            internal MI_ClassDeclPtr classDecl;
            internal string namespaceName;
            internal string serverName;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            internal IntPtr[] reserved;
        }

        // Marshal implements these with Reflection - pay this hit only once
        private static int MI_ClassMembersFTOffset = (int)Marshal.OffsetOf<MI_ClassMembers>("ft");

        private static int MI_ClassMembersSize = Marshal.SizeOf<MI_ClassMembers>();

        private MI_ClassPtr ptr;
        private bool isDirect;
        private Lazy<MI_ClassFT> mft;

        ~MI_Class()
        {
            Marshal.FreeHGlobal(this.ptr.ptr);
        }

        private MI_Class(bool isDirect)
        {
            this.isDirect = isDirect;
            this.mft = new Lazy<MI_ClassFT>(this.MarshalFT);

            var necessarySize = this.isDirect ? MI_ClassMembersSize : NativeMethods.IntPtrSize;
            this.ptr.ptr = Marshal.AllocHGlobal(necessarySize);

            unsafe
            {
                NativeMethods.memset((byte*)this.ptr.ptr, 0, (uint)necessarySize);
            }
        }

        internal static MI_Class NewDirectPtr()
        {
            return new MI_Class(true);
        }

        internal static MI_Class NewIndirectPtr()
        {
            return new MI_Class(false);
        }

        internal static MI_Class NewFromDirectPtr(IntPtr ptr)
        {
            var res = new MI_Class(false);
            Marshal.WriteIntPtr(res.ptr.ptr, ptr);
            return res;
        }

        public static implicit operator MI_ClassPtr(MI_Class instance)
        {
            // If the indirect pointer is zero then the object has not
            // been initialized and it is not valid to refer to its data
            if (instance != null && instance.Ptr == IntPtr.Zero)
            {
                throw new InvalidCastException();
            }

            return new MI_ClassPtr() { ptr = instance == null ? IntPtr.Zero : instance.Ptr };
        }

        public static implicit operator MI_ClassOutPtr(MI_Class instance)
        {
            // We are not currently supporting the ability to get the address
            // of our direct pointer, though it is technically feasible
            if (instance != null && instance.isDirect)
            {
                throw new InvalidCastException();
            }

            return new MI_ClassOutPtr() { ptr = instance == null ? IntPtr.Zero : instance.ptr.ptr };
        }

        internal static MI_Class Null { get { return null; } }
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

        internal MI_Result GetClassName(
            out string className
            )
        {
            MI_String classNameLocal = MI_String.NewIndirectPtr();

            MI_Result resultLocal = this.ft.GetClassName(this,
                classNameLocal);

            className = classNameLocal.Value;
            return resultLocal;
        }

        internal MI_Result GetNameSpace(
            out string nameSpace
            )
        {
            MI_String nameSpaceLocal = MI_String.NewIndirectPtr();

            MI_Result resultLocal = this.ft.GetNameSpace(this,
                nameSpaceLocal);

            nameSpace = nameSpaceLocal.Value;
            return resultLocal;
        }

        internal MI_Result GetServerName(
            out string serverName
            )
        {
            MI_String serverNameLocal = MI_String.NewIndirectPtr();

            MI_Result resultLocal = this.ft.GetServerName(this,
                serverNameLocal);

            serverName = serverNameLocal.Value;
            return resultLocal;
        }

        internal MI_Result GetElementCount(
            out UInt32 count
            )
        {
            MI_Result resultLocal = this.ft.GetElementCount(this,
                out count);
            return resultLocal;
        }

        internal MI_Result GetMethodCount(
            out UInt32 count
            )
        {
            MI_Result resultLocal = this.ft.GetMethodCount(this,
                out count);
            return resultLocal;
        }

        internal MI_Result GetParentClassName(
            out string name
            )
        {
            MI_String nameLocal = MI_String.NewIndirectPtr();

            MI_Result resultLocal = this.ft.GetParentClassName(this,
                nameLocal);

            name = nameLocal.Value;
            return resultLocal;
        }

        internal MI_Result GetParentClass(
            out MI_Class parentClass
            )
        {
            MI_Class parentClassLocal = MI_Class.NewIndirectPtr();

            MI_Result resultLocal = this.ft.GetParentClass(this,
                parentClassLocal);

            parentClass = parentClassLocal;
            return resultLocal;
        }

        internal MI_Result Delete()
        {
            return this.ft.Delete(this);
        }

        internal MI_Result Clone(
            out MI_Class newClass
            )
        {
            MI_Class newClassLocal = MI_Class.NewIndirectPtr();

            MI_Result resultLocal = this.ft.Clone(this,
                newClassLocal);

            newClass = newClassLocal;
            return resultLocal;
        }

        private MI_ClassFT ft { get { return this.mft.Value; } }

        private MI_ClassFT MarshalFT()
        {
            return NativeMethods.GetFTAsOffsetFromPtr<MI_ClassFT>(this.Ptr, MI_Class.MI_ClassMembersFTOffset);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal class MI_ClassFT
        {
            internal MI_Class_GetClassName GetClassName;
            internal MI_Class_GetNameSpace GetNameSpace;
            internal MI_Class_GetServerName GetServerName;
            internal MI_Class_GetElementCount GetElementCount;
            internal MI_Class_GetElement GetElement;
            internal MI_Class_GetElementAt GetElementAt;
            internal MI_Class_GetClassQualifierSet GetClassQualifierSet;
            internal MI_Class_GetMethodCount GetMethodCount;
            internal MI_Class_GetMethodAt GetMethodAt;
            internal MI_Class_GetMethod GetMethod;
            internal MI_Class_GetParentClassName GetParentClassName;
            internal MI_Class_GetParentClass GetParentClass;
            internal MI_Class_Delete Delete;
            internal MI_Class_Clone Clone;

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_GetClassName(
                MI_ClassPtr self,
                [In, Out] MI_String className
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_GetNameSpace(
                MI_ClassPtr self,
                [In, Out] MI_String nameSpace
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_GetServerName(
                MI_ClassPtr self,
                [In, Out] MI_String serverName
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_GetElementCount(
                MI_ClassPtr self,
                out UInt32 count
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_GetElement(
                MI_ClassPtr self,
                string name,
                [In, Out] MI_Value.MIValueBlock value,
                [MarshalAs(UnmanagedType.U1)] out bool valueExists,
                out MI_Type type,
                [In, Out] MI_String referenceClass,
                [In, Out] MI_QualifierSetPtr qualifierSet,
                out MI_Flags flags,
                out UInt32 index
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_GetElementAt(
                MI_ClassPtr self,
                UInt32 index,
                [In, Out] MI_String name,
                [In, Out] MI_Value.MIValueBlock value,
                [MarshalAs(UnmanagedType.U1)] out bool valueExists,
                out MI_Type type,
                [In, Out] MI_String referenceClass,
                [In, Out] MI_QualifierSetPtr qualifierSet,
                out MI_Flags flags
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_GetClassQualifierSet(
                MI_ClassPtr self,
                [In, Out] MI_QualifierSetPtr qualifierSet
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_GetMethodCount(
                MI_ClassPtr self,
                out UInt32 count
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_GetMethodAt(
                MI_ClassPtr self,
                UInt32 index,
                [In, Out] MI_String name,
                [In, Out] MI_QualifierSetPtr qualifierSet,
                [In, Out] MI_ParameterSetPtr parameterSet
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_GetMethod(
                MI_ClassPtr self,
                string name,
                [In, Out] MI_QualifierSetPtr qualifierSet,
                [In, Out] MI_ParameterSetPtr parameterSet,
                out UInt32 index
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_GetParentClassName(
                MI_ClassPtr self,
                [In, Out] MI_String name
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_GetParentClass(
                MI_ClassPtr self,
                [In, Out] MI_ClassOutPtr parentClass
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_Delete(
                MI_ClassPtr self
                );

            [UnmanagedFunctionPointer(MI_PlatformSpecific.MiCallConvention, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
            internal delegate MI_Result MI_Class_Clone(
                MI_ClassPtr self,
                [In, Out] MI_ClassOutPtr newClass
                );
        }
    }
}