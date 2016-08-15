/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


using Microsoft.Management.Infrastructure.Native;
using System;
using System.Diagnostics;

namespace Microsoft.Management.Infrastructure.Internal.Operations
{
    internal class CimSyncInstanceEnumerator : CimSyncEnumeratorBase<CimInstance>
    {
        private readonly Guid _CimSessionInstanceID;
        private readonly string _CimSessionComputerName;

        internal CimSyncInstanceEnumerator(
            Guid cimSessionInstanceID,
            string cimSessionComputerName,
            bool shortenLifetimeOfResults)
            : base(shortenLifetimeOfResults)
        {
            this._CimSessionInstanceID = cimSessionInstanceID;
            this._CimSessionComputerName = cimSessionComputerName;
        }

        internal override MI_Result NativeMoveNext(MI_Operation operationHandle, out CimInstance currentItem, out bool moreResults, out MI_Result operationResult, out string errorMessage, out MI_Instance errorDetailsHandle)
        {
            Debug.Assert(operationHandle != null, "Caller should verify operationHandle != null");

            currentItem = null;

            MI_Instance instanceHandle;
            MI_Result functionResult = operationHandle.GetInstance(
                out instanceHandle,
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
                currentItem = new CimInstance(instanceHandle);
                currentItem.SetCimSessionComputerName(this._CimSessionComputerName);
                currentItem.SetCimSessionInstanceId(this._CimSessionInstanceID);
            }

            return functionResult;
        }
    }
}
