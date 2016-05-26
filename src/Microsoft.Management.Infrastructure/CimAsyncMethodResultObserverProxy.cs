/*============================================================================
 * Copyright (C) Microsoft Corporation, All rights reserved.
 *============================================================================
 */

using Microsoft.Management.Infrastructure.Options;
using NativeObject;
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
            OperationCallbackProcessingContext callbackProcessingContext,
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
                var backingInstance = new CimInstance(instanceHandle, null);
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

        internal void StreamedParameterCallback(
            OperationCallbackProcessingContext callbackProcessingContext,
            MI_Operation operationHandle,
            string parameterName,
            object parameterValue,
            MI_Type parameterType)
        {
            parameterValue = CimInstance.ConvertFromNativeLayer(
                                value: (MI_Value)parameterValue,
                                sharedParentHandle: null,
                                clone: !this._shortenLifetimeOfResults);
            {
                var cimInstance = parameterValue as CimInstance;
                if (cimInstance != null)
                {
                    cimInstance.SetCimSessionComputerName(this._CimSessionComputerName);
                    cimInstance.SetCimSessionInstanceId(this._CimSessionInstanceID);
                }

                var cimInstances = parameterValue as CimInstance[];
                if (cimInstances != null)
                {
                    foreach (var i in cimInstances)
                    {
                        if (i != null)
                        {
                            i.SetCimSessionComputerName(this._CimSessionComputerName);
                            i.SetCimSessionInstanceId(this._CimSessionInstanceID);
                        }
                    }
                }
            }

            try
            {
                CimMethodResultBase currentItem = new CimMethodStreamedResult(parameterName, parameterValue, parameterType.ToCimType());
                this.ProcessNativeCallback(callbackProcessingContext, currentItem, true, MI_Result.MI_RESULT_OK, null, null);
            }
            finally
            {
                if (this._shortenLifetimeOfResults)
                {
                    var cimInstance = parameterValue as CimInstance;
                    if (cimInstance != null)
                    {
                        cimInstance.Dispose();
                    }

                    var cimInstances = parameterValue as CimInstance[];
                    if (cimInstances != null)
                    {
                        foreach (var i in cimInstances)
                        {
                            if (i != null)
                            {
                                i.Dispose();
                            }
                        }
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