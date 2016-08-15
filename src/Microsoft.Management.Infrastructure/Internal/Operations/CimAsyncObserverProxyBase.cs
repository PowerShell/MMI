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
    internal class CimAsyncObserverProxyBase<T> : CimAsyncCallbacksReceiverBase
        where T : class
    {
        private readonly IObserver<T> _observer;

        internal CimAsyncObserverProxyBase(IObserver<T> observer)
        {
            Debug.Assert(observer != null, "Caller should verify that observer != null");

            this._observer = observer;
        }

        private bool _reportOperationStarted = false;

        internal void SetReportOperationStarted(bool reportOperationStarted)
        {
            this._reportOperationStarted = reportOperationStarted;
        }

        internal void ProcessNativeCallback(
            CimOperationCallbackProcessingContext callbackProcessingContext,
            T currentItem,
            bool moreResults,
            MI_Result operationResult,
            string errorMessage,
            MI_Instance errorDetailsHandle)
        {
            Debug.Assert(callbackProcessingContext != null, "We should never get called with a null callbackProcessingContext");

            if (!moreResults)
            {
                this.DisposeOperationWhenPossible();
            }

            if ((currentItem == null) && (operationResult == MI_Result.MI_RESULT_OK))
            {
                // process the ACK message if and only if operationResult == OK
                if (this._reportOperationStarted)
                {
                    this.OnNextInternal(callbackProcessingContext, currentItem);
                }
            }
            else if (currentItem != null)
            {
                Debug.Assert(operationResult == MI_Result.MI_RESULT_OK, "Assumming that instances are reported back only on success");
                this.OnNextInternal(callbackProcessingContext, currentItem);
            }

            CimException exception = CimException.GetExceptionIfMiResultFailure(operationResult, errorMessage, errorDetailsHandle);
            if (exception != null)
            {
                Debug.Assert(operationResult != MI_Result.MI_RESULT_OK, "Assumming that exceptions are reported back only on failure");
                Debug.Assert(!moreResults, "Assumming that an error means end of results");

                // this throw-catch is needed to fill-out 1) WER data and 2) exception's stack trace
                try
                {
                    throw exception;
                }
                catch (CimException filledOutException)
                {
                    exception = filledOutException;
                }
            }

            if (!moreResults)
            {
                this.InvokeWhenOperationIsSet(
                    cimOperation => this.ProcessEndOfResultsWorker(callbackProcessingContext, cimOperation, exception));
            }
        }

        private void ProcessEndOfResultsWorker(
            CimOperationCallbackProcessingContext callbackProcessingContext,
            CimOperation cimOperation,
            Exception exception)
        {
            if (exception == null)
            {
                this.OnCompletedInternal(callbackProcessingContext);
            }
            else
            {
                CancellationMode cancellationMode = cimOperation.CancellationMode;
                switch (cancellationMode)
                {
                    case CancellationMode.NoCancellationOccured:
                    case CancellationMode.IgnoreCancellationRequests:
                        this.OnErrorInternal(callbackProcessingContext, exception);
                        break;

                    case CancellationMode.ThrowOperationCancelledException:
                        this.OnErrorInternal(callbackProcessingContext, new OperationCanceledException(exception.Message, exception));
                        break;

                    case CancellationMode.SilentlyStopProducingResults:
                        break;

                    default:
                        Debug.Assert(false, "Unrecognized CancellationMode");
                        break;
                }
            }
        }

        private void OnErrorInternal(CimOperationCallbackProcessingContext callbackProcessingContext, Exception exception)
        {
            this.CallIntoUserCallback(
                callbackProcessingContext,
                () => this._observer.OnError(exception),
                suppressFurtherUserCallbacks: true);
        }

        private void OnCompletedInternal(CimOperationCallbackProcessingContext callbackProcessingContext)
        {
            this.CallIntoUserCallback(
                callbackProcessingContext,
                this._observer.OnCompleted,
                suppressFurtherUserCallbacks: true);
        }

        private void OnNextInternal(CimOperationCallbackProcessingContext callbackProcessingContext, T item)
        {
            this.CallIntoUserCallback(
                callbackProcessingContext,
                () => this._observer.OnNext(item));
        }

        internal override void ReportInternalError(CimOperationCallbackProcessingContext callbackProcessingContext, Exception internalError)
        {
            this.OnErrorInternal(callbackProcessingContext, internalError);
        }
    }
}
