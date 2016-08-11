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
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Management.Infrastructure.Internal.Operations
{
    internal abstract class CimSyncEnumerableBase<TItem, TEnumerator> : IEnumerable<TItem>
        where TEnumerator : CimSyncEnumeratorBase<TItem>, IEnumerator<TItem>
        where TItem : class
    {
        private readonly CancellationToken? _cancellationToken;
        private readonly Func<CimAsyncCallbacksReceiverBase, MI_Operation> _operationStarter;

        internal CimSyncEnumerableBase(CimOperationOptions operationOptions, Func<CimAsyncCallbacksReceiverBase, MI_Operation> operationStarter)
        {
            Debug.Assert(operationStarter != null, "Caller should verify that operationStarter != null");

            this._cancellationToken = operationOptions.GetCancellationToken();
            this._operationStarter = operationStarter;
        }

        internal abstract TEnumerator CreateEnumerator();

        #region IEnumerable<CimInstance> Members

        public IEnumerator<TItem> GetEnumerator()
        {
            TEnumerator enumerator = this.CreateEnumerator();
            CimOperation operation;

            MI_Operation operationHandle = this._operationStarter(enumerator);
            operation = new CimOperation(operationHandle, this._cancellationToken);

            enumerator.SetOperation(operation);
            return enumerator;
        }

        #endregion IEnumerable<CimInstance> Members

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion IEnumerable Members
    }
}
