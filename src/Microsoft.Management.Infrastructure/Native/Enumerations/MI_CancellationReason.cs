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
    internal enum MI_CancellationReason : uint
    {
        MI_REASON_NONE = 0,
        MI_REASON_TIMEOUT,
        MI_REASON_SHUTDOWN,
        MI_REASON_SERVICESTOP
    }
}
