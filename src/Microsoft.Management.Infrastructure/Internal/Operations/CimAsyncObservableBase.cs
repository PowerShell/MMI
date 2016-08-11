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
using Microsoft.Management.Infrastructure.Options.Internal;
using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Management.Infrastructure.Internal.Operations
{
    internal abstract class CimAsyncObservableBase<TObserverProxy, TResult> : IObservable<TResult>
        where TObserverProxy : CimAsyncObserverProxyBase<TResult>
        where TResult : class
    {
        private readonly Func<CimAsyncCallbacksReceiverBase, MI_Operation> _operationStarter;
        private readonly CancellationToken? _cancellationToken;
        private readonly bool _reportOperationStarted;

        internal CimAsyncObservableBase(
            CimOperationOptions operationOptions,
            Func<CimAsyncCallbacksReceiverBase, MI_Operation> operationStarter)
        {
            Debug.Assert(operationStarter != null, "Caller should verify operationStarter != null");
            this._operationStarter = operationStarter;

            this._cancellationToken = operationOptions.GetCancellationToken();
            this._reportOperationStarted = operationOptions.GetReportOperationStarted();
        }

        internal abstract TObserverProxy CreateObserverProxy(IObserver<TResult> observer);

        #region IObservable<CimInstance> Members

        public IDisposable Subscribe(IObserver<TResult> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException("observer");
            }

            TObserverProxy observerProxy = this.CreateObserverProxy(observer);
            observerProxy.SetReportOperationStarted(this._reportOperationStarted);

            MI_Operation operationHandle = this._operationStarter(observerProxy);
            CimOperation operation = new CimOperation(operationHandle, this._cancellationToken);

            observerProxy.SetOperation(operation);
            return new CimAsyncCancellationDisposable(operation);
        }

        #endregion IObservable<CimInstance> Members
    }
}
