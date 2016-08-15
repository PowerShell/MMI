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
    internal enum MI_OperationCallback_ResponseType : uint
    {
        No = 0,
        Yes = 1,
        NoToAll = 2,
        YesToAll = 3
    }
}
