/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


namespace Microsoft.Management.Infrastructure.Internal.Operations
{
    internal enum CancellationMode
    {
        NoCancellationOccured,
        ThrowOperationCancelledException,
        SilentlyStopProducingResults,
        IgnoreCancellationRequests
    }
}
