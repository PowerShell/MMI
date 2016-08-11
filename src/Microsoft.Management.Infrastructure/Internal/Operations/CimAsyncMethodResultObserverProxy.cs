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
    internal class CimAsyncMethodResultObserverProxy : CimAsyncObserverProxyBase<CimMethodResultBase>
    {
        private readonly bool _shortenLifetimeOfResults;
        private readonly Guid _CimSessionInstanceID;
        private readonly string _CimSessionComputerName;

        internal CimAsyncMethodResultObserverProxy(IObserver<CimMethodResultBase> observer,
            Guid cimSessionInstanceID,
            string cimSessionComputerName,
            bool shortenLifetimeOfResults)
            : base(observer)
        {
            this._shortenLifetimeOfResults = shortenLifetimeOfResults;
            this._CimSessionInstanceID = cimSessionInstanceID;
            this._CimSessionComputerName = cimSessionComputerName;
        }

        internal void InstanceResultCallback(
            CimOperationCallbackProcessingContext callbackProcessingContext,
            MI_Operation operationHandle,
            MI_Instance instanceHandle,
            bool moreResults,
            MI_Result operationResult,
            String errorMessage,
            MI_Instance errorDetailsHandle)
        {
            CimMethodResult currentItem = null;
            if ((instanceHandle != null) && (!instanceHandle.IsNull))
            {
                if (!_shortenLifetimeOfResults)
                {
                    instanceHandle = instanceHandle.Clone();
                }
                var backingInstance = new CimInstance(instanceHandle);
                backingInstance.SetCimSessionComputerName(this._CimSessionComputerName);
                backingInstance.SetCimSessionInstanceId(this._CimSessionInstanceID);
                currentItem = new CimMethodResult(backingInstance);
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
            //operationCallbacks.instanceResult = this.InstanceResultCallback;
            if ((operationOptions != null) && (operationOptions.EnableMethodResultStreaming))
            {
                // TODO: Uncomment and fix below
                //operationCallbacks.streamedParameterResult = this.StreamedParameterCallback;
            }
        }
    }
}
