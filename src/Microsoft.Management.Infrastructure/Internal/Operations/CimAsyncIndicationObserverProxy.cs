/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
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
            CimOperationCallbackProcessingContext callbackProcessingContext,
            MI_Operation operationHandle,
            MI_Instance instanceHandle,
            String bookMark,
            String machineID,
            bool moreResults,
            MI_Result operationResult,
            String errorMessage,
            MI_Instance errorDetailsHandle)
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
                this.ProcessNativeCallback(callbackProcessingContext, currentItem, moreResults, operationResult, errorMessage, errorDetailsHandle);
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
            // TODO: Uncomment and fix below
            //operationCallbacks.indicationResult = this.IndicationResultCallback;
        }
    }
}
