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
    internal class MI_DeserializerCallbacks
    {
        // Ignore warnings that say that we never use all of these IntPtrs
        // Most of the containers get away with having supposedly unused fields
        // but that's because most of them are tagged for interop and the compiler
        // is smart enough to realize that they're probably important
#pragma warning disable CS0169
        IntPtr classObjectNeededContext;
        internal MI_Deserializer.MI_Deserializer_ClassObjectNeeded classObjectNeeded;

        IntPtr includedFileContext;
        IntPtr getIncludedFileContent;
        IntPtr freeIncludedFileContent;

        IntPtr reserved_instanceResultContext;
        IntPtr reserved_instanceResult;

        IntPtr reserved_classResultcontext;
        IntPtr reserved_classResult;

        IntPtr classObjectNeededOnIdContext;
        IntPtr classObjectNeededOnId;

        IntPtr classObjectAndIdContext;
        IntPtr classObjectAndId;

        IntPtr qualifierDeclNeededContext;
        IntPtr qualifierDeclNeeded;
#pragma warning restore CS0169

        internal MI_DeserializerCallbacksNative GetNativeCallbacks(string format)
        {
            MI_DeserializerCallbacksNative callbacksNative = new MI_DeserializerCallbacksNative();

            callbacksNative.classObjectNeeded = MI_DeserializerCallbacks.GetNativeClassObjectNeededCallback(format, this.classObjectNeeded);

            return callbacksNative;
        }

        internal static MI_Deserializer.MI_Deserializer_ClassObjectNeededNative GetNativeClassObjectNeededCallback(string format, MI_Deserializer.MI_Deserializer_ClassObjectNeeded managedCallback)
        {
            if (managedCallback == null)
            {
                return null;
            }

            return delegate (
                IntPtr context,
                IntPtr serverNamePtr,
                IntPtr namespaceNamePtr,
                IntPtr classNamePtr,
                IntPtr requestedClassObject)
            {
                MI_String serverName = MI_String.NewFromDirectPtr(serverNamePtr);
                MI_String namespaceName = MI_String.NewFromDirectPtr(namespaceNamePtr);
                MI_String className = MI_String.NewFromDirectPtr(classNamePtr);

                MI_Class classObject;

                try
                {
                    var localResult = managedCallback(serverName.Value, namespaceName.Value, className.Value, out classObject);
                    if (localResult == MI_Result.MI_RESULT_OK)
                    {
                        IntPtr outPtr;
                        if (MI_SerializationFormat.MOF.Equals(format, StringComparison.OrdinalIgnoreCase))
                        {
                            // The MOF deserializer helpfully tries to manage the class objects returned by the
                            // callback and will cheerfully delete them without warning. Return a copy instead.
                            MI_Class tmp;
                            localResult = classObject.Clone(out tmp);
                            if (localResult != MI_Result.MI_RESULT_OK)
                            {
                                return localResult;
                            }

                            outPtr = tmp.Ptr;
                        }
                        else
                        {
                            outPtr = classObject.Ptr;
                        }

                        Marshal.WriteIntPtr(requestedClassObject, outPtr);
                    }

                    return localResult;
                }
                catch
                {
                    return MI_Result.MI_RESULT_FAILED;
                }
            };
        }

        [StructLayout(LayoutKind.Sequential, CharSet = MI_PlatformSpecific.AppropriateCharSet)]
        internal class MI_DeserializerCallbacksNative
        {
            IntPtr classObjectNeededContext;
            internal MI_Deserializer.MI_Deserializer_ClassObjectNeededNative classObjectNeeded;

            IntPtr includedFileContext;
            IntPtr getIncludedFileContent;
            IntPtr freeIncludedFileContent;

            IntPtr reserved_instanceResultContext;
            IntPtr reserved_instanceResult;

            IntPtr reserved_classResultcontext;
            IntPtr reserved_classResult;

            IntPtr classObjectNeededOnIdContext;
            IntPtr classObjectNeededOnId;

            IntPtr classObjectAndIdContext;
            IntPtr classObjectAndId;

            IntPtr qualifierDeclNeededContext;
            IntPtr qualifierDeclNeeded;
        }
    }
}
