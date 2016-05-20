/*============================================================================
 * Copyright (C) Microsoft Corporation, All rights reserved. 
 *============================================================================
 */
using NativeObject;

namespace Microsoft.Management.Infrastructure.Options
{
    public enum ImpersonationType
    {
        None = MI_ImpersonationType.None,
        Default = MI_ImpersonationType.Default,
        Delegate = MI_ImpersonationType.Delegate,
        Identify = MI_ImpersonationType.Identify,
        Impersonate = MI_ImpersonationType.Impersonate,
    };
}

namespace Microsoft.Management.Infrastructure.Options.Internal
{
    internal static class ImpersonationTypeExtensionMethods
    {
        public static MI_ImpersonationType ToNativeType(this ImpersonationType impersonationType)
        {
            return (MI_ImpersonationType) impersonationType;
        }
    }
}