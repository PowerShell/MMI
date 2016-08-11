/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


using Microsoft.Management.Infrastructure.Generic;
using System;
using System.Diagnostics;

namespace Microsoft.Management.Infrastructure
{
    /// <summary>
    /// Represents a regular - return value and all the out parameter values.
    /// </summary>
    /// <seealso cref="CimMethodStreamedResult"/>
    /// <seealso cref="CimSession.InvokeMethod(string, CimInstance, string, CimMethodParametersCollection)"/>
    public class CimMethodResult : CimMethodResultBase, IDisposable
    {
        private CimMethodParametersCollection _backingMethodParametersCollection;

        internal CimMethodResult(CimInstance backingInstance)
        {
            Debug.Assert(backingInstance != null, "Caller should verify backingInstance != null");
            this._backingMethodParametersCollection = new CimMethodParametersCollection(backingInstance);
        }

        public CimMethodParameter ReturnValue
        {
            get
            {
                this.AssertNotDisposed();
                return this.OutParameters["ReturnValue"];
            }
        }

        public CimReadOnlyKeyedCollection<CimMethodParameter> OutParameters
        {
            get
            {
                this.AssertNotDisposed();
                return this._backingMethodParametersCollection;
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Releases resources associated with this object
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources associated with this object
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                this._backingMethodParametersCollection.Dispose();
                this._backingMethodParametersCollection = null;
            }

            _disposed = true;
        }

        internal void AssertNotDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
        }

        private bool _disposed;

        #endregion IDisposable Members
    }
}
