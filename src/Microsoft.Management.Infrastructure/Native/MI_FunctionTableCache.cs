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
using System.Collections.Concurrent;

namespace Microsoft.Management.Infrastructure.Native
{
    internal static class MI_FunctionTableCache
    {
        internal static ConcurrentDictionary<IntPtr, object> functionPtrCache;

        static MI_FunctionTableCache()
        {
            MI_FunctionTableCache.functionPtrCache = new ConcurrentDictionary<IntPtr, object>();
        }
        
        internal static unsafe T GetFTAsOffsetFromPtr<T>(IntPtr ptr, int offset) where T : new()
        {
            IntPtr ftPtr = IntPtr.Zero;
            unsafe
            {
                // Just as easily could be implemented with Marshal
                // but that would copy more than the one pointer we need
                IntPtr structurePtr = ptr;
                if (structurePtr == IntPtr.Zero)
                {
                    throw new InvalidOperationException();
                }

                ftPtr = *((IntPtr*)((byte*)structurePtr + offset));
            }

            if (ftPtr == IntPtr.Zero)
            {
                throw new InvalidOperationException();
            }

            object existingFtObject;
            if(MI_FunctionTableCache.functionPtrCache.TryGetValue(ftPtr, out existingFtObject))
            {
                return (T)existingFtObject;
            }
            else
            {
                // No apparent way to implement this in an unsafe block
                T res = new T();
                Marshal.PtrToStructure(ftPtr, res);

                // No need to check result. If another thread managed to get here first
                // then it will write the same logical value since that's the
                // whole principal we're relying on for the optimization
                MI_FunctionTableCache.functionPtrCache.TryAdd(ftPtr, res);

                return res;
            }

        }
    }
}
