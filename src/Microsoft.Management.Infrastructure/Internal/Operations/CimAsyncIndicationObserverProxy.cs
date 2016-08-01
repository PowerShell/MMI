/*============================================================================
 * Copyright (C) Microsoft Corporation, All rights reserved.
 *============================================================================
 */

using Microsoft.Management.Infrastructure.Native;
using Microsoft.Management.Infrastructure.Options;
using System;

namespace Microsoft.Management.Infrastructure.Internal.Operations
{
    internal class CimAsyncIndicationObserverProxy : CimAsyncObserverProxyBase<CimSubscriptionResult>
    {
        private readonly bool _shortenLifetimeOfResults;

        internal CimAsyncIndicationObserverProxy(IObserver<CimSubscriptionResult> observer, bool shortenLifetimeOfResults)
            : base(observer)
        {
            this._shortenLifetimeOfResults = shortenLifetimeOfResults;
        }

        internal void IndicationResultCallback(
            MI_Operation operationHandle,
            object callbackProcessingContext,
            MI_Instance instanceHandle,
            String bookMark,
            String machineID,
            bool moreResults,
            MI_Result operationResult,
            String errorMessage,
            MI_Instance errorDetailsHandle,
            MI_OperationCallbacks.MI_OperationCallback_ResultAcknowledgement resultAcknowledgement)
        {
            CimSubscriptionResult currentItem = null;
            if ((instanceHandle != null) && (!instanceHandle.IsNull))
            {
                if (!_shortenLifetimeOfResults)
                {
                    instanceHandle = instanceHandle.Clone();
                }
                currentItem = new CimSubscriptionResult(instanceHandle, bookMark, machineID);
            }

            try
            {
                this.ProcessNativeCallback((CimOperationCallbackProcessingContext)callbackProcessingContext, currentItem, moreResults, operationResult, errorMessage, errorDetailsHandle);
            }
            finally
            {
                if (_shortenLifetimeOfResults)
                {
                    if (currentItem != null)
                    {
                        currentItem.Dispose();
                    }
                }
            }
        }

        public override void RegisterAcceptedAsyncCallbacks(MI_OperationCallbacks operationCallbacks, CimOperationOptions operationOptions)
        {
            base.RegisterAcceptedAsyncCallbacks(operationCallbacks, operationOptions);
            operationCallbacks.indicationResult = this.IndicationResultCallback;
        }
    }
}