/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


using Microsoft.Management.Infrastructure.Internal;
using Microsoft.Management.Infrastructure.Native;
using System;

namespace Microsoft.Management.Infrastructure.Options
{
    /// <summary>
    /// Represents options of <see cref="CimSubscriptionDelivery"/>
    /// </summary>
    public class CimSubscriptionDeliveryOptions : IDisposable
#if (!_CORECLR)
        //
        // Only implement these interfaces on FULL CLR and not Core CLR
        //
        , ICloneable
#endif
    {
        #region Constructors

        private MI_SubscriptionDeliveryOptions _subscriptionDeliveryOptionsHandle;

        internal MI_SubscriptionDeliveryOptions SubscriptionDeliveryOptionsHandle
        {
            get
            {
                this.AssertNotDisposed();
                return this._subscriptionDeliveryOptionsHandle;
            }
        }

        /// <summary>
        /// Creates a new <see cref="CimSubscriptionDeliveryOptions"/>
        /// </summary>
        public CimSubscriptionDeliveryOptions() : this(types: CimSubscriptionDeliveryType.None)
        {
        }

        /// <summary>
        /// Creates a new <see cref="CimSubscriptionDeliveryOptions"/>
        /// </summary>
        /// <param name="types"></param>
        public CimSubscriptionDeliveryOptions(CimSubscriptionDeliveryType types)
        {
            Initialize(types);
        }

        private void Initialize(CimSubscriptionDeliveryType types)
        {
            MI_SubscriptionDeliveryOptions tmp;
            MI_Result result = CimApplication.Handle.NewSubscriptionDeliveryOptions((MI_SubscriptionDeliveryType)types, out tmp);
            CimException.ThrowIfMiResultFailure(result);
            this._subscriptionDeliveryOptionsHandle = tmp;
        }

        /// <summary>
        /// Instantiates a deep copy of <paramref name="optionsToClone"/>
        /// </summary>
        /// <param name="optionsToClone">options to clone</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="optionsToClone"/> is <c>null</c></exception>
        public CimSubscriptionDeliveryOptions(CimSubscriptionDeliveryOptions optionsToClone)
        {
            if (optionsToClone == null)
            {
                throw new ArgumentNullException("optionsToClone");
            }
            MI_SubscriptionDeliveryOptions tmp;
            MI_Result result = optionsToClone.SubscriptionDeliveryOptionsHandle.Clone(out tmp);
            CimException.ThrowIfMiResultFailure(result);
            this._subscriptionDeliveryOptionsHandle = tmp;
        }

        #endregion Constructors

        #region Options

        /// <summary>
        /// Sets a string
        /// </summary>
        /// <param name="optionName"></param>
        /// <param name="optionValue"></param>
        /// <param name="flags"></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="optionName"/> is <c>null</c></exception>
        public void SetString(string optionName, string optionValue, UInt32 flags)
        {
            if (string.IsNullOrWhiteSpace(optionName))
            {
                throw new ArgumentNullException("optionName");
            }
            this.AssertNotDisposed();

            MI_Result result = this._subscriptionDeliveryOptionsHandle.SetString(optionName, optionValue, flags);
            CimException.ThrowIfMiResultFailure(result);
        }

        /// <summary>
        /// Sets a custom option
        /// </summary>
        /// <param name="optionName"></param>
        /// <param name="optionValue"></param>
        /// <param name="flags"></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="optionName"/> is <c>null</c></exception>
        public void SetNumber(string optionName, UInt32 optionValue, UInt32 flags)
        {
            if (string.IsNullOrWhiteSpace(optionName))
            {
                throw new ArgumentNullException("optionName");
            }
            this.AssertNotDisposed();

            MI_Result result = this._subscriptionDeliveryOptionsHandle.SetNumber(optionName, optionValue, flags);
            CimException.ThrowIfMiResultFailure(result);
        }

        /// <summary>
        /// Sets a custom option
        /// </summary>
        /// <param name="optionName"></param>
        /// <param name="optionValue"></param>
        /// <param name="flags"></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="optionName"/> is <c>null</c></exception>
        public void SetDateTime(string optionName, DateTime optionValue, UInt32 flags)
        {
            if (string.IsNullOrWhiteSpace(optionName))
            {
                throw new ArgumentNullException("optionName");
            }
            this.AssertNotDisposed();

            MI_Datetime dt = new MI_Datetime(optionValue);
            MI_Result result = this._subscriptionDeliveryOptionsHandle.SetDateTime(optionName, dt, flags);
            CimException.ThrowIfMiResultFailure(result);
        }

        /// <summary>
        /// Sets a custom option
        /// </summary>
        /// <param name="optionName"></param>
        /// <param name="optionValue"></param>
        /// <param name="flags"></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="optionName"/> is <c>null</c></exception>
        public void SetDateTime(string optionName, TimeSpan optionValue, UInt32 flags)
        {
            if (string.IsNullOrWhiteSpace(optionName))
            {
                throw new ArgumentNullException("optionName");
            }
            this.AssertNotDisposed();

            MI_Datetime dt = new MI_Datetime(optionValue);
            MI_Result result = this._subscriptionDeliveryOptionsHandle.SetDateTime(optionName, dt, flags);
            CimException.ThrowIfMiResultFailure(result);
        }

        /// <summary>
        /// Sets a custom option
        /// </summary>
        /// <param name="optionName"></param>
        /// <param name="optionValue"></param>
        /// <param name="flags"></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="optionName"/> is <c>null</c></exception>
        public void SetInterval(string optionName, TimeSpan optionValue, UInt32 flags)
        {
            if (string.IsNullOrWhiteSpace(optionName))
            {
                throw new ArgumentNullException("optionName");
            }
            this.AssertNotDisposed();

            // TODO: convert optionValue to MI_Interval
            MI_Interval interval;
            interval.days = interval.hours = interval.minutes = interval.seconds = interval.microseconds = interval.__padding1 = interval.__padding2 = interval.__padding3 = 0;
            MI_Result result = this._subscriptionDeliveryOptionsHandle.SetInterval(optionName, interval, flags);
            CimException.ThrowIfMiResultFailure(result);
        }

        /// <summary>
        /// AddCredentials
        /// </summary>
        /// <param name="optionName"></param>
        /// <param name="optionValue"></param>
        /// <param name="flags"></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="optionName"/> is <c>null</c></exception>
        public void AddCredentials(string optionName, CimCredential optionValue, UInt32 flags)
        {
            if (string.IsNullOrWhiteSpace(optionName) || optionValue == null)
            {
                throw new ArgumentNullException("optionName");
            }
            if (optionValue == null)
            {
                throw new ArgumentNullException("optionValue");
            }
            this.AssertNotDisposed();

            // TODO: Implement this
            //MI_Result result = this._subscriptionDeliveryOptionsHandle.SubscriptionDeliveryOptionsMethods.AddCredentials(optionName, optionValue.GetCredential(), flags);
            //CimException.ThrowIfMiResultFailure(result);
        }

        #endregion Options

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
                this._subscriptionDeliveryOptionsHandle.Delete();
                this._subscriptionDeliveryOptionsHandle = null;
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

        #region ICloneable Members

#if(!_CORECLR)

        object ICloneable.Clone()
        {
            return new CimSubscriptionDeliveryOptions(this);
        }

#endif // !_CORECLR

        #endregion ICloneable Members
    }
}

namespace Microsoft.Management.Infrastructure.Options.Internal
{
    internal static class CimSubscriptionDeliveryOptionssExtensionMethods
    {
        static internal MI_SubscriptionDeliveryOptions GetSubscriptionDeliveryOptionsHandle(this CimSubscriptionDeliveryOptions deliveryOptions)
        {
            return deliveryOptions != null ? deliveryOptions.SubscriptionDeliveryOptionsHandle : null;
        }
    }
}
