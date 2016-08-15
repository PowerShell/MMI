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
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Management.Infrastructure
{
#if(!_CORECLR)

    [Serializable]
#endif
    public class CimException : Exception, IDisposable
    {
        private CimInstance _errorData;

        /// <summary>
        /// Native error code returned from MI Client API
        /// </summary>
        public NativeErrorCode NativeErrorCode { get; private set; }

        #region Internal constructors - THESE ARE THE ONES TO USE

        internal CimException(MI_Result errorCode, string errorMessage, MI_Instance errorDetailsHandle)
            : this(errorCode, errorMessage, errorDetailsHandle, null)
        {
        }

        internal CimException(MI_Result errorCode, string errorMessage, MI_Instance errorDetailsHandle, string exceptionMessage)
            : base(exceptionMessage ?? CimException.GetExceptionMessage(errorCode, errorMessage, errorDetailsHandle))
        {
            this.NativeErrorCode = errorCode.ToNativeErrorCode();

            if (errorDetailsHandle != null)
            {
                this._errorData = new CimInstance(errorDetailsHandle.Clone());
            }
        }

        static private string GetExceptionMessage(CimInstance cimError)
        {
            if (cimError == null)
            {
                throw new ArgumentNullException("cimError");
            }

            return GetExceptionMessage(MI_Result.MI_RESULT_FAILED, null, cimError.InstanceHandle);
        }

        static private string GetExceptionMessage(MI_Instance errorDetailsHandle)
        {
            if (errorDetailsHandle != null)
            {
                var temporaryErrorData = new CimInstance(errorDetailsHandle);
                string cimErrorMessage;
                if (TryGetErrorDataProperty(temporaryErrorData, "Message", out cimErrorMessage))
                {
                    return cimErrorMessage;
                }
            }

            return null;
        }

        static private string GetExceptionMessage(MI_Result errorCode, string errorMessage, MI_Instance errorDetailsHandle)
        {
            string result;

            // first check if CIM_Error provided an error message
            result = GetExceptionMessage(errorDetailsHandle);
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            // if the above failed, then use MI_Utilities_CimErrorFromErrorCode to get a good CIM_Error

            // TODO: Add GetCimErrorFromMiResult to MI API
            //Native.ApplicationMethods.GetCimErrorFromMiResult(errorCode, errorMessage, out errorDetailsHandle);
            try
            {
                result = GetExceptionMessage(errorDetailsHandle);
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }
            }
            finally
            {
                if (errorDetailsHandle != null)
                {
                    errorDetailsHandle.Delete();
                }
            }

            // if everything fails, then return a non-localized errorCode
            return errorCode.ToString();
        }

        #endregion Internal constructors - THESE ARE THE ONES TO USE

        #region Public constructor taking CIM_Error instance

        public CimException(CimInstance cimError)
            : base(CimException.GetExceptionMessage(cimError))
        {
            if (cimError == null)
            {
                throw new ArgumentNullException("cimError");
            }

            this._errorData = new CimInstance(cimError);
        }

        #endregion Public constructor taking CIM_Error instance

        #region Standard constructors - do not use

        public CimException()
            : this(String.Empty, (Exception)null)
        {
        }

        public CimException(string message)
            : this(message, (Exception)null)
        {
        }

        public CimException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion Standard constructors - do not use

        #region Deserializing constructor + other plumbing for .NET serialization

        private const string serializationId_ErrorData = "ErrorData";
#if(!_CORECLR)

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            base.GetObjectData(info, context);
            info.AddValue(serializationId_ErrorData, this.ErrorData);
        }

        protected CimException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            this._errorData = (CimInstance)info.GetValue(serializationId_ErrorData, typeof(CimInstance));
        }

#endif

        #endregion Deserializing constructor + other plumbing for .NET serialization

        #region Properties

        /// <summary>
        /// Optional (can be null) error data.
        /// This is typically an instance of a standard CIM_Error class or of a class derived from it.
        /// </summary>
        public CimInstance ErrorData
        {
            get
            {
                this.AssertNotDisposed();
                return this._errorData;
            }
        }

        private bool TryGetErrorDataProperty<T>(string propertyName, out T propertyValue)
        {
            return TryGetErrorDataProperty(this.ErrorData, propertyName, out propertyValue);
        }

        private static bool TryGetErrorDataProperty<T>(CimInstance errorData, string propertyName, out T propertyValue)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(propertyName), "Caller should verify !string.IsNullOrWhiteSpace(propertyName)");
            propertyValue = default(T);

            if (errorData == null)
            {
                return false;
            }

            try
            {
                CimProperty property = errorData.CimInstanceProperties[propertyName];
                if (property == null)
                {
                    return false;
                }

                if (!(property.Value is T))
                {
                    return false;
                }

                propertyValue = (T)property.Value;
                return true;
            }
            catch (CimException)
            {
                return false;
            }
        }

        // corresponds to CIM_Error.MessageId, related to SMA.ErrorRecord.FullyQualifiedErrorId property (directly mapped to the errorId parameter of ErrorRecord's constructor)
        public string MessageId
        {
            get
            {
                this.AssertNotDisposed();

                string result;
                return this.TryGetErrorDataProperty("MessageId", out result) ? result : null;
            }
        }

        // corresponds to CIM_Error.ErrorSource and also to SMA.ErrorRecord.TargetObject
        public string ErrorSource
        {
            get
            {
                this.AssertNotDisposed();

                string result;
                return this.TryGetErrorDataProperty("ErrorSource", out result) ? result : null;
            }
        }

        // corresponds to CIM_Error.ErrorType and also to SMA.ErrorRecord.CategoryInfo.Category
        public UInt16 ErrorType
        {
            get
            {
                this.AssertNotDisposed();

                UInt16 result;
                return this.TryGetErrorDataProperty("ErrorType", out result) ? result : (UInt16)0;
            }
        }

        // corresponds to CIM_Error.CIMStatusCode
        public UInt32 StatusCode
        {
            get
            {
                this.AssertNotDisposed();

                UInt32 result;
                return this.TryGetErrorDataProperty("CIMStatusCode", out result) ? result : (UInt32)0;
            }
        }

        #endregion Properties

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
                if (this._errorData != null)
                {
                    this._errorData.Dispose();
                    this._errorData = null;
                }
            }

            _disposed = true;
        }

        internal void AssertNotDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        private bool _disposed;

        #endregion IDisposable Members

        #region Static helper methods for translating an MiResult into an exception

        internal static void ThrowIfMiResultFailure(MI_Result result)
        {
            ThrowIfMiResultFailure(result, null, null);
        }

        internal static void ThrowIfMiResultFailure(MI_Result result, MI_Instance errorData)
        {
            ThrowIfMiResultFailure(result, null, errorData);
        }

        internal static void ThrowIfMiResultFailure(MI_Result result, string errorMessage, MI_Instance errorData)
        {
            CimException exception = GetExceptionIfMiResultFailure(result, errorMessage, errorData);
            if (exception != null)
            {
                throw exception;
            }
        }

        internal static CimException GetExceptionIfMiResultFailure(MI_Result result, string errorMessage, MI_Instance errorData)
        {
            return result == MI_Result.MI_RESULT_OK ? null : new CimException(result, errorMessage, errorData);
        }

        #endregion Static helper methods for translating an MiResult into an exception
    }
}
