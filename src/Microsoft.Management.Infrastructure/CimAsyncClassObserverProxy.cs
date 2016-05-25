/*============================================================================
 * Copyright (C) Microsoft Corporation, All rights reserved. 
 *============================================================================
 */

using System;
using Microsoft.Management.Infrastructure.Options;
using NativeObject;

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
            OperationCallbackProcessingContext callbackProcessingContext,
            MI_Operation operationHandle,
            MI_Class ClassHandle,
            bool moreResults,
            MI_Result operationResult,
            String errorMessage,
            MI_Instance errorDetailsHandle)
        {
            CimClass currentItem = null;
            if ((ClassHandle != null) && (!ClassHandle.IsInvalid))
            {
                if (!_shortenLifetimeOfResults)
                {
                    ClassHandle = ClassHandle.Clone();
                }
                currentItem = new CimClass(ClassHandle);
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
            //operationCallbacks.classResult = this.ClassCallback;
        }
    }
}
