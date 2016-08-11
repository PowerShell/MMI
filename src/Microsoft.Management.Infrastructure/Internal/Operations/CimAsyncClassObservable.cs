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

namespace Microsoft.Management.Infrastructure.Internal.Operations
{
    internal class CimAsyncClassObservable : CimAsyncObservableBase<CimAsyncClassObserverProxy, CimClass>
    {
        private readonly bool _shortenLifetimeOfResults;

        internal CimAsyncClassObservable(
            CimOperationOptions operationOptions,
            Func<CimAsyncCallbacksReceiverBase, MI_Operation> operationStarter)
            : base(operationOptions, operationStarter)
        {
            this._shortenLifetimeOfResults = operationOptions.GetShortenLifetimeOfResults();
        }

        internal override CimAsyncClassObserverProxy CreateObserverProxy(IObserver<CimClass> observer)
        {
            Debug.Assert(observer != null, "Caller should verify observer != null");
            return new CimAsyncClassObserverProxy(observer, _shortenLifetimeOfResults);
        }
    }
}
