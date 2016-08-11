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
    internal class CimAsyncMethodResultObservable : CimAsyncObservableBase<CimAsyncMethodResultObserverProxy, CimMethodResultBase>
    {
        private readonly bool _shortenLifetimeOfResults;
        private readonly Guid _CimSessionInstanceID;
        private readonly string _CimSessionComputerName;

        internal CimAsyncMethodResultObservable(
            CimOperationOptions operationOptions,
            Guid cimSessionInstanceID,
            string cimSessionComputerName,
            Func<CimAsyncCallbacksReceiverBase, MI_Operation> operationStarter)
            : base(operationOptions, operationStarter)
        {
            this._shortenLifetimeOfResults = operationOptions.GetShortenLifetimeOfResults();
            this._CimSessionInstanceID = cimSessionInstanceID;
            this._CimSessionComputerName = cimSessionComputerName;
        }

        internal override CimAsyncMethodResultObserverProxy CreateObserverProxy(IObserver<CimMethodResultBase> observer)
        {
            Debug.Assert(observer != null, "Caller should verify observer != null");
            return new CimAsyncMethodResultObserverProxy(observer,
                this._CimSessionInstanceID,
                this._CimSessionComputerName,
                this._shortenLifetimeOfResults);
        }
    }
}
