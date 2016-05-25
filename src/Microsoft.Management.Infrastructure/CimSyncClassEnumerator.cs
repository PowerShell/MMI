/*============================================================================
 * Copyright (C) Microsoft Corporation, All rights reserved. 
 *============================================================================
 */

using System.Diagnostics;
using NativeObject;

namespace Microsoft.Management.Infrastructure.Internal.Operations
{
    internal class CimSyncClassEnumerator : CimSyncEnumeratorBase<CimClass>
    {
        internal CimSyncClassEnumerator(bool shortenLifetimeOfResults)
            : base(shortenLifetimeOfResults)
        {
        }

        internal override MI_Result NativeMoveNext(MI_Operation operationHandle, out CimClass currentItem,
                    out bool moreResults, out MI_Result operationResult,
            out string errorMessage, out MI_Instance errorDetailsHandle)
        {
            Debug.Assert(operationHandle != null, "Caller should verify operationHandle != null");

            currentItem = null;

            MI_Class classHandle;
            MI_Result functionResult = operationHandle.GetClass(
                out classHandle,
                out moreResults,
                out operationResult,
                out errorMessage,
                out errorDetailsHandle);

            if ((classHandle != null) && !classHandle.IsInvalid)
            {
                if (!this.ShortenLifetimeOfResults)
                {
                    classHandle = classHandle.Clone();
                }
                currentItem = new CimClass(classHandle);
            }

            return functionResult;
        }
    }
}
