/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/
namespace Microsoft.Management.Infrastructure.Native
{
    public enum MI_ImpersonationType : uint
    {
        Default = 0,
        None = 1,
        Identify = 2,
        Impersonate = 3,
        Delegate = 4,
    }
}
