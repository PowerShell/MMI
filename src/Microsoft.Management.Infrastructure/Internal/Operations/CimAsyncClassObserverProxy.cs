/*============================================================================
 * Copyright (C) Microsoft Corporation, All rights reserved.
 *============================================================================
 */

using Microsoft.Management.Infrastructure.Native;
using Microsoft.Management.Infrastructure.Options;
using System;

namespace Microsoft.Management.Infrastructure.Internal.Operations
{
    internal class CimAsyncClassObserverProxy : CimAsyncObserverProxyBase<CimClass>
    {
        private readonly bool _shortenLifetimeOfResults;

        internal CimAsyncClassObserverProxy(IObserver<CimClass> observer, bool shortenLifetimeOfResults)
            : base(observer)
        {
            this._shortenLifetimeOfResults = shortenLifetimeOfResults;
        }

        internal void ClassCallback(
            MI_Operation operationHandle,
            object callbackProcessingContext,
            MI_Class ClassHandle,
            bool moreResults,
            MI_Result operationResult,
            String errorMessage,
            MI_Instance errorDetailsHandle,
            MI_OperationCallbacks.MI_OperationCallback_ResultAcknowledgement resultAcknowledgement)
        {
            CimClass currentItem = null;
            if ((ClassHandle != null) && (!ClassHandle.IsNull))
            {
                if (!_shortenLifetimeOfResults)
                {
                    ClassHandle = ClassHandle.Clone();
                }
                currentItem = new CimClass(ClassHandle);
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
            operationCallbacks.classResult = this.ClassCallback;
        }
    }
}