/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


using System;

namespace Microsoft.Management.Infrastructure.Internal.Operations
{
    internal sealed class EmptyDisposable : IDisposable
    {
        private EmptyDisposable()
        {
            // should only instantiate through Singleton property
        }

        /// <summary>
        /// Releases resources associated with this object
        /// </summary>
        public void Dispose()
        {
            // a no-op
        }

        private static readonly Lazy<EmptyDisposable> lazySingleton = new Lazy<EmptyDisposable>(
            valueFactory: () => new EmptyDisposable(),
            isThreadSafe: true);

        internal static EmptyDisposable Singleton
        {
            get
            {
                return lazySingleton.Value;
            }
        }
    }
}
