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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.Management.Infrastructure.Options
{
    /// <summary>
    /// Represents options of <see cref="CimSession"/>
    /// </summary>
    public class CimSessionOptions : IDisposable
#if (!_CORECLR)
        //
        // Only implement these interfaces on FULL CLR and not Core CLR
        //
        , ICloneable
#endif
    {
        private readonly Lazy<MI_DestinationOptions> _destinationOptionsHandle;

        internal MI_DestinationOptions DestinationOptionsHandleOnDemand
        {
            get
            {
                this.AssertNotDisposed();
                return this._destinationOptionsHandle.Value;
            }
        }

        internal MI_DestinationOptions DestinationOptionsHandle
        {
            get
            {
                this.AssertNotDisposed();
                if (this._destinationOptionsHandle.IsValueCreated)
                {
                    return this._destinationOptionsHandle.Value;
                }
                return null;
            }
        }

        internal string Protocol { get; private set; }

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="CimSessionOptions"/> object that uses the default transport protocol
        /// </summary>
        public CimSessionOptions()
            : this(protocol: null, validateProtocol: false)
        {
        }

        /// <summary>
        /// Creates a new <see cref="CimSessionOptions"/> object that uses the specified transport protocol
        /// </summary>
        /// <param name="protocol">Protocol to use.  This string corresponds to a registry key at TODO/FIXME.</param>
        protected CimSessionOptions(string protocol)
            : this(protocol, validateProtocol: true)
        {
        }

        private CimSessionOptions(string protocol, bool validateProtocol)
        {
            if (validateProtocol)
            {
                if (string.IsNullOrWhiteSpace(protocol))
                {
                    throw new ArgumentNullException("protocol");
                }
            }

            this.Protocol = protocol;

            this._destinationOptionsHandle = new Lazy<MI_DestinationOptions>(
                    delegate
                    {
                        MI_DestinationOptions tmp;
                        MI_Result result = CimApplication.Handle.NewDestinationOptions(out tmp);
                        CimException.ThrowIfMiResultFailure(result);
                        return tmp;
                    });
        }

        /// <summary>
        /// Instantiates a deep copy of <paramref name="optionsToClone"/>
        /// </summary>
        /// <param name="optionsToClone">options to clone</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="optionsToClone"/> is <c>null</c></exception>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Old Code")]
        internal CimSessionOptions(CimSessionOptions optionsToClone)
        {
            if (optionsToClone == null)
            {
                throw new ArgumentNullException("optionsToClone");
            }

            this.Protocol = optionsToClone.Protocol;
            if (optionsToClone.DestinationOptionsHandle == null)
            {
                // underline DestinationOptions is not created yet, then create a new one
                this._destinationOptionsHandle = new Lazy<MI_DestinationOptions>(
                    delegate
                    {
                        MI_DestinationOptions tmp;
                        MI_Result result = CimApplication.Handle.NewDestinationOptions(out tmp);
                        CimException.ThrowIfMiResultFailure(result);
                        return tmp;
                    });
            }
            else
            {
                MI_DestinationOptions tmp;
                MI_Result result = optionsToClone.DestinationOptionsHandle.Clone(out tmp);
                CimException.ThrowIfMiResultFailure(result);
                this._destinationOptionsHandle = new Lazy<MI_DestinationOptions>(() => tmp);
            }
            // Ensure the destinationOptions is created
            if (this.DestinationOptionsHandleOnDemand == null)
            {
                CimException.ThrowIfMiResultFailure(MI_Result.MI_RESULT_FAILED);
            }
        }

        #endregion Constructors

        #region Options

        /// <summary>
        /// Sets a custom option
        /// </summary>
        /// <param name="optionName"></param>
        /// <param name="optionValue"></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="optionName"/> is <c>null</c></exception>
        public void SetCustomOption(string optionName, string optionValue)
        {
            if (string.IsNullOrWhiteSpace(optionName))
            {
                throw new ArgumentNullException("optionName");
            }
            this.AssertNotDisposed();

            MI_Result result = this.DestinationOptionsHandleOnDemand.SetString(optionName,
                                               optionValue,
                                               MI_DestinationOptionsFlags.Unused);
            CimException.ThrowIfMiResultFailure(result);
        }

        /// <summary>
        /// Sets a custom option
        /// </summary>
        /// <param name="optionName"></param>
        /// <param name="optionValue"></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="optionName"/> is <c>null</c></exception>
        public void SetCustomOption(string optionName, UInt32 optionValue)
        {
            if (string.IsNullOrWhiteSpace(optionName))
            {
                throw new ArgumentNullException("optionName");
            }
            this.AssertNotDisposed();

            MI_Result result = this.DestinationOptionsHandleOnDemand.SetNumber(optionName,
                                               optionValue,
                                               MI_DestinationOptionsFlags.Unused);
            CimException.ThrowIfMiResultFailure(result);
        }

        /// <summary>
        /// Sets a Destination Credential
        /// </summary>
        /// <param name="credential"></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="credential"/> is <c>null</c></exception>
        public void AddDestinationCredentials(CimCredential credential)
        {
            // TODO: Once credentials are working, uncomment and fix
            /*
                if (credential == null)
                {
                    throw new ArgumentNullException("credential");
                }
                this.AssertNotDisposed();

            MI_UserCredentials nativeCredential;
            SecureString securePassword = credential.GetSecureString();
            IntPtr passwordPtr = IntPtr.Zero;
            if( securePassword != null && securePassword.Length > 0)
            {
    #if(!_CORECLR)
            passwordPtr = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
    #else
            passwordPtr = SecureStringMarshal.SecureStringToCoTaskMemUnicode(securePassword);
    #endif
            nativeCredential.usernamePassword.password = passwordPtr;
            }
            else
            {
            nativeCredential.usernamePassword.password = null;
            }

            MI_Result result = this.DestinationOptionsHandleOnDemand.AddCredentials("__MI_DESTINATIONOPTIONS_DESTINATION_CREDENTIALS",
                nativeCredential,
                MI_DestinationOptionFlags.Unused);

            if ( passwordPtr != IntPtr.Zero )
            {
    #if(!_CORECLR)
            Marshal.FreeHGlobal(passwordPtr);
    #else
            SecureStringMarshal.ZeroFreeCoTaskMemUnicode(passwordPtr);
    #endif
            }
                CimException.ThrowIfMiResultFailure(result);
            */
        }

        /// <summary>
        /// Sets timeout
        /// </summary>
        /// <value></value>
        public TimeSpan Timeout
        {
            set
            {
                this.AssertNotDisposed();
                MI_Interval interval = value;
                MI_Result result = this.DestinationOptionsHandleOnDemand.SetInterval("__MI_DESTINATIONOPTIONS_TIMEOUT",
                                             interval,
                                             MI_DestinationOptionsFlags.Unused);
                CimException.ThrowIfMiResultFailure(result);
            }

            get
            {
                this.AssertNotDisposed();

                TimeSpan timeout;
                MI_Interval value;
                UInt32 index;
                MI_DestinationOptionsFlags flags;
                MI_Result result = this.DestinationOptionsHandleOnDemand.GetInterval("__MI_DESTINATIONOPTIONS_TIMEOUT",
                                             out value,
                                             out index,
                                             out flags);

                timeout = value;

                return (result == MI_Result.MI_RESULT_OK) ? timeout : TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Sets data culture.
        /// </summary>
        /// <value>Culture to use.  &lt;c&gt;null&lt;/c&gt; indicates the current thread&apos;s culture</value>
        public CultureInfo Culture
        {
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.AssertNotDisposed();

                MI_Result result = this.DestinationOptionsHandleOnDemand.SetString("__MI_DESTINATIONOPTIONS_DATA_LOCALE",
                                           value.Name,
                                           MI_DestinationOptionsFlags.Unused);
                CimException.ThrowIfMiResultFailure(result);
            }

            get
            {
                this.AssertNotDisposed();

                string locale;
                UInt32 index;
                MI_DestinationOptionsFlags flags;
                MI_Result result = this.DestinationOptionsHandleOnDemand.GetString("__MI_DESTINATIONOPTIONS_DATA_LOCALE",
                                                   out locale,
                                                   out index,
                                                   out flags);

                return (result == MI_Result.MI_RESULT_OK) ? new CultureInfo(locale) : null;
            }
        }

        /// <summary>
        /// Sets UI culture.
        /// </summary>
        /// <value>Culture to use.  &lt;c&gt;null&lt;/c&gt; indicates the current thread&apos;s UI culture</value>
        public CultureInfo UICulture
        {
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.AssertNotDisposed();

                MI_Result result = this.DestinationOptionsHandleOnDemand.SetString("__MI_DESTINATIONOPTIONS_UI_LOCALE",
                                           value.Name,
                                           MI_DestinationOptionsFlags.Unused);
                CimException.ThrowIfMiResultFailure(result);
            }

            get
            {
                this.AssertNotDisposed();

                string locale;
                UInt32 index;
                MI_DestinationOptionsFlags flags;
                MI_Result result = this.DestinationOptionsHandleOnDemand.GetString("__MI_DESTINATIONOPTIONS_UI_LOCALE",
                                                   out locale,
                                                   out index,
                                                   out flags);

                return (result == MI_Result.MI_RESULT_OK) ? new CultureInfo(locale) : null;
            }
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
                MI_DestinationOptions tmpHandle = this.DestinationOptionsHandle;
                if (tmpHandle != null)
                {
                    tmpHandle.Delete();
                }
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
            return new CimSessionOptions(this);
        }

#endif // !_CORECLR

        #endregion ICloneable Members
    }
}
