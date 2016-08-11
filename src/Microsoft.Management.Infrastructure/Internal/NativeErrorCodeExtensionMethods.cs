

using System;
using Microsoft.Management.Infrastructure.Native;

namespace Microsoft.Management.Infrastructure.Internal
{
    internal static class NativeErrorCodeExtensionMethods
    {
        public static NativeErrorCode ToNativeErrorCode(this MI_Result miResult)
        {
            return (NativeErrorCode)miResult;
        }
    }
}