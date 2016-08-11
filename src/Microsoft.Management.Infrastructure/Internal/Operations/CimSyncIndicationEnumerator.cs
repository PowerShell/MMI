/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


using Microsoft.Management.Infrastructure.Native;
using System.Diagnostics;

namespace Microsoft.Management.Infrastructure.Internal.Operations
{
    internal class CimSyncIndicationEnumerator : CimSyncEnumeratorBase<CimSubscriptionResult>
    {
        internal CimSyncIndicationEnumerator(bool shortenLifetimeOfResults)
            : base(shortenLifetimeOfResults)
        {
        }

        internal override MI_Result NativeMoveNext(MI_Operation operationHandle, out CimSubscriptionResult currentItem,
                    out bool moreResults, out MI_Result operationResult,
            out string errorMessage, out MI_Instance errorDetailsHandle)
        {
            Debug.Assert(operationHandle != null, "Caller should verify operationHandle != null");

            currentItem = null;

            MI_Instance instanceHandle;
            string bookmark;
            string machineID;
            MI_Result functionResult = operationHandle.GetIndication(
                out instanceHandle,
                out bookmark,
                out machineID,
                out moreResults,
                out operationResult,
                out errorMessage,
                out errorDetailsHandle);

            if ((instanceHandle != null) && !instanceHandle.IsNull)
            {
                if (!this.ShortenLifetimeOfResults)
                {
                    instanceHandle = instanceHandle.Clone();
                }
                currentItem = new CimSubscriptionResult(instanceHandle, bookmark, machineID);
            }

            return functionResult;
        }
    }
}
