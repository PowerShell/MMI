/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


using Microsoft.Management.Infrastructure.Native;

namespace Microsoft.Management.Infrastructure.Options
{
    public enum ImpersonationType : uint
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
            return (MI_ImpersonationType)impersonationType;
        }
    }
}
