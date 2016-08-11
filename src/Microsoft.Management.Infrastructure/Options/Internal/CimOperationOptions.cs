/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


using Microsoft.Management.Infrastructure.Internal;
using Microsoft.Management.Infrastructure.Internal.Operations;
using Microsoft.Management.Infrastructure.Native;
using Microsoft.Management.Infrastructure.Options.Internal;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Microsoft.Management.Infrastructure.Options
{
    /// <summary>
    /// Represents options of a CIM operation.
    /// </summary>
    public class CimOperationOptions : IDisposable
#if (!_CORECLR)
        //
        // Only implement these interfaces on FULL CLR and not Core CLR
        //
        , ICloneable
#endif
    {
        private readonly Lazy<MI_OperationOptions> _operationOptionsHandle;

        private MI_OperationOptions OperationOptionsHandleOnDemand
        {
            get
            {
                this.AssertNotDisposed();
                return this._operationOptionsHandle.Value;
            }
        }

        internal MI_OperationOptions OperationOptionsHandle
        {
            get
            {
                if (this._operationOptionsHandle.IsValueCreated)
                {
                    return this._operationOptionsHandle.Value;
                }
                return null;
            }
        }

        private readonly MI_OperationCallbacks _operationCallback;

        internal MI_OperationCallbacks OperationCallback
        {
            get
            {
                this.AssertNotDisposed();
                return this._operationCallback;
            }
        }

        /// <summary>
        /// Creates a new <see cref="CimOperationOptions"/> instance (where the server has to understand all the options).
        /// </summary>
        public CimOperationOptions()
            : this(mustUnderstand: false)
        {
        }

        /// <summary>
        /// Creates a new <see cref="CimOperationOptions"/> instance.
        /// </summary>
        /// <param name="mustUnderstand">Indicates whether the server has to understand all the options.</param>
        public CimOperationOptions(bool mustUnderstand)
        {
            var operationCallbacks = new MI_OperationCallbacks();
            this._operationCallback = operationCallbacks;
            _writeMessageCallback = null;
            _writeProgressCallback = null;
            _writeErrorCallback = null;
            _promptUserCallback = null;
            this._operationOptionsHandle = new Lazy<MI_OperationOptions>(
                    delegate
                    {
                        MI_OperationOptions operationOptionsHandle;
                        MI_Result result = CimApplication.Handle.NewOperationOptions(
                            mustUnderstand, out operationOptionsHandle);
                        CimException.ThrowIfMiResultFailure(result);
                        return operationOptionsHandle;
                    });
        }

        /// <summary>
        /// Instantiates a deep copy of <paramref name="optionsToClone"/>
        /// </summary>
        /// <param name="optionsToClone">options to clone</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="optionsToClone"/> is <c>null</c></exception>
        public CimOperationOptions(CimOperationOptions optionsToClone)
        {
            if (optionsToClone == null)
            {
                throw new ArgumentNullException("optionsToClone");
            }

            this._operationCallback = optionsToClone.GetOperationCallbacks();
            _writeMessageCallback = optionsToClone._writeMessageCallback;
            _writeProgressCallback = optionsToClone._writeProgressCallback;
            _writeErrorCallback = optionsToClone._writeErrorCallback;
            _promptUserCallback = optionsToClone._promptUserCallback;
            this._operationOptionsHandle = new Lazy<MI_OperationOptions>(
                    delegate
                    {
                        MI_OperationOptions tmp;
                        MI_Result result = optionsToClone.OperationOptionsHandle.Clone(out tmp);
                        CimException.ThrowIfMiResultFailure(result);
                        return tmp;
                    });
        }

        /// <summary>
        /// Sets operation timeout
        /// </summary>
        /// <value></value>
        public TimeSpan Timeout
        {
            set
            {
                this.AssertNotDisposed();
                MI_Interval interval = value;

                MI_Result result = this.OperationOptionsHandleOnDemand.SetInterval("__MI_OPERATIONOPTIONS_TIMEOUT",
                                           interval,
                                           MI_OperationOptionsFlags.Unused);
                CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();
                MI_Interval value;
                UInt32 index;
                MI_OperationOptionsFlags flags;
                MI_Result result = this.OperationOptionsHandleOnDemand.GetInterval("__MI_OPERATIONOPTIONS_TIMEOUT",
                                           out value,
                                           out index,
                                           out flags);
                CimException.ThrowIfMiResultFailure(result);
                TimeSpan tempTimeout = value;
                return tempTimeout;
            }
        }

        /// <summary>
        /// Sets resource URI prefix
        /// </summary>
        /// <value></value>
        public Uri ResourceUriPrefix
        {
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.AssertNotDisposed();

                MI_Result result = this.OperationOptionsHandleOnDemand.SetString("__MI_OPERATIONOPTIONS_RESOURCE_URI_PREFIX",
                                         value.ToString(),
                                         MI_OperationOptionsFlags.Unused);
                CimException.ThrowIfMiResultFailure(result);
            }

            get
            {
                this.AssertNotDisposed();
                string value;
                UInt32 index;
                MI_OperationOptionsFlags flags;
                MI_Result result = this.OperationOptionsHandleOnDemand.GetString("__MI_OPERATIONOPTIONS_RESOURCE_URI_PREFIX",
                                         out value,
                                         out index,
                                         out flags);
                CimException.ThrowIfMiResultFailure(result);
                return new Uri(value);
            }
        }

        /// <summary>
        /// Sets resource URI
        /// </summary>
        /// <value></value>
        public Uri ResourceUri
        {
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.AssertNotDisposed();

                MI_Result result = this.OperationOptionsHandleOnDemand.SetString("__MI_OPERATIONOPTIONS_RESOURCE_URI",
                                         value.ToString(),
                                         MI_OperationOptionsFlags.Unused);
                CimException.ThrowIfMiResultFailure(result);
            }

            get
            {
                this.AssertNotDisposed();

                string value;
                UInt32 index;
                MI_OperationOptionsFlags flags;
                MI_Result result = this.OperationOptionsHandleOnDemand.GetString("__MI_OPERATIONOPTIONS_RESOURCE_URI",
                                         out value,
                                         out index,
                                         out flags);
                CimException.ThrowIfMiResultFailure(result);
                return new Uri(value);
            }
        }

        /// <summary>
        /// Sets whether to use machine ID
        /// </summary>
        /// <value></value>
        public bool UseMachineId
        {
            set
            {
                this.AssertNotDisposed();
                UInt32 number = value ? (uint)1 : (uint)0;
                MI_Result result = this.OperationOptionsHandleOnDemand.SetNumber("__MI_OPERATIONOPTIONS_USE_MACHINE_ID",
                                         number,
                                         MI_OperationOptionsFlags.Unused);
                CimException.ThrowIfMiResultFailure(result);
            }

            get
            {
                this.AssertNotDisposed();
                bool tmp;
                UInt32 value;
                UInt32 index;
                MI_OperationOptionsFlags flags;
                MI_Result result = this.OperationOptionsHandleOnDemand.GetNumber("__MI_OPERATIONOPTIONS_USE_MACHINE_ID",
                                         out value,
                                         out index,
                                         out flags);
                CimException.ThrowIfMiResultFailure(result);
                tmp = value == 1 ? true : false;
                return tmp;
            }
        }

        /// <summary>
        /// Sets a custom transport option
        /// </summary>
        /// <param name="optionName"></param>
        /// <param name="optionValue"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="optionName"/> is <c>null</c></exception>
        public void SetOption(string optionName, string optionValue)
        {
            if (string.IsNullOrWhiteSpace(optionName))
            {
                throw new ArgumentNullException("optionName");
            }
            this.AssertNotDisposed();

            MI_Result result = this.OperationOptionsHandleOnDemand.SetString(optionName,
                                         optionValue,
                                         MI_OperationOptionsFlags.Unused);
            CimException.ThrowIfMiResultFailure(result);
        }

        /// <summary>
        /// Sets regular prompt user
        /// </summary>
        /// <param name="callbackMode"></param>
        /// <param name="automaticConfirmation"></param>
        public void SetPromptUserRegularMode(CimCallbackMode callbackMode, bool automaticConfirmation)
        {
            this.AssertNotDisposed();

            MI_Result result = this.OperationOptionsHandleOnDemand.SetNumber("__MI_OPERATIONOPTIONS_PROMPTUSERMODE",
                                             (UInt32)callbackMode,
                                             MI_OperationOptionsFlags.Unused);
            CimException.ThrowIfMiResultFailure(result);
        }

        /// <summary>
        /// Sets a custom transport option
        /// </summary>
        /// <param name="optionName"></param>
        /// <param name="optionValue"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="optionName"/> is <c>null</c></exception>
        public void SetOption(string optionName, UInt32 optionValue)
        {
            if (string.IsNullOrWhiteSpace(optionName))
            {
                throw new ArgumentNullException("optionName");
            }
            this.AssertNotDisposed();

            MI_Result result = this.OperationOptionsHandleOnDemand.SetNumber(optionName,
                                         optionValue,
                                         MI_OperationOptionsFlags.Unused);
            CimException.ThrowIfMiResultFailure(result);
        }

        #region PSSEMANTICS

        internal void WriteMessageCallbackInternal(
            CimOperationCallbackProcessingContext callbackProcessingContext,
            MI_Operation operationHandle,
            UInt32 channel,
            string message)
        {
            if (_writeMessageCallback != null)
            {
                var callbacksReceiverBase = (CimAsyncCallbacksReceiverBase)callbackProcessingContext.ManagedOperationContext;
                callbacksReceiverBase.CallIntoUserCallback(
                    callbackProcessingContext,
                    () => _writeMessageCallback(channel, message));
            }
        }

        private WriteMessageCallback _writeMessageCallback;

        private void WriteProgressCallbackInternal(
            CimOperationCallbackProcessingContext callbackProcessingContext,
            MI_Operation operationHandle,
            string activity,
            string currentOperation,
            string statusDescription,
            UInt32 percentageCompleted,
            UInt32 secondsRemaining)
        {
            if (_writeProgressCallback != null)
            {
                var callbacksReceiverBase = (CimAsyncCallbacksReceiverBase)callbackProcessingContext.ManagedOperationContext;
                callbacksReceiverBase.CallIntoUserCallback(
                    callbackProcessingContext,
                    () => _writeProgressCallback(activity, currentOperation, statusDescription, percentageCompleted, secondsRemaining));
            }
        }

        private WriteProgressCallback _writeProgressCallback;

        internal void WriteErrorCallbackInternal(
            CimOperationCallbackProcessingContext callbackProcessingContext,
            MI_Operation operationHandle,
            MI_Instance instanceHandle,
            out MI_OperationCallback_ResponseType response)
        {
            response = MI_OperationCallback_ResponseType.Yes;
            if (_writeErrorCallback != null)
            {
                Debug.Assert(instanceHandle != null, "Caller should verify instance != null");
                CimInstance cimInstance = null;
                try
                {
                    if (!instanceHandle.IsNull)
                    {
                        cimInstance = new CimInstance(instanceHandle.Clone());
                        var callbacksReceiverBase = (CimAsyncCallbacksReceiverBase)callbackProcessingContext.ManagedOperationContext;
                        CimResponseType userResponse = CimResponseType.None;
                        callbacksReceiverBase.CallIntoUserCallback(
                            callbackProcessingContext,
                            delegate { userResponse = _writeErrorCallback(cimInstance); });
                        response = (MI_OperationCallback_ResponseType)userResponse;
                    }
                }
                finally
                {
                    if (cimInstance != null)
                    {
                        cimInstance.Dispose();
                    }
                }
            }
        }

        private WriteErrorCallback _writeErrorCallback;

        internal void PromptUserCallbackInternal(
            CimOperationCallbackProcessingContext callbackProcessingContext,
            MI_Operation operationHandle,
            string message,
            MI_PromptType promptType,
            out MI_OperationCallback_ResponseType response)
        {
            response = MI_OperationCallback_ResponseType.Yes;
            if (_promptUserCallback != null)
            {
                var callbacksReceiverBase = (CimAsyncCallbacksReceiverBase)callbackProcessingContext.ManagedOperationContext;
                CimResponseType userResponse = CimResponseType.None;
                callbacksReceiverBase.CallIntoUserCallback(
                    callbackProcessingContext,
                    delegate { userResponse = _promptUserCallback(message, (CimPromptType)promptType); });
                response = (MI_OperationCallback_ResponseType)userResponse;
            }
        }

        private PromptUserCallback _promptUserCallback;

        /// <summary>
        /// Set Write Error Mode
        /// </summary>
        /// <value></value>
        public CimCallbackMode WriteErrorMode
        {
            set
            {
                this.AssertNotDisposed();

                MI_Result result = this._operationOptionsHandle.Value.SetNumber("__MI_OPERATIONOPTIONS_WRITEERRORMODE",
                                        (uint)value,
                                        MI_OperationOptionsFlags.Unused);
                CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();

                UInt32 value;
                UInt32 index;
                MI_OperationOptionsFlags flags;
                MI_Result result = this._operationOptionsHandle.Value.GetNumber("__MI_OPERATIONOPTIONS_WRITEERRORMODE",
                                         out value,
                                         out index,
                                         out flags);
                CimException.ThrowIfMiResultFailure(result);
                return (CimCallbackMode)value;
            }
        }

        /// <summary>
        /// Set Prompt User Mode
        /// </summary>
        /// <value></value>
        public CimCallbackMode PromptUserMode
        {
            set
            {
                MI_Result result = this._operationOptionsHandle.Value.SetNumber("__MI_OPERATIONOPTIONS_PROMPTUSERMODE",
                                        (uint)value,
                                        MI_OperationOptionsFlags.Unused);
                CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();

                UInt32 value;
                UInt32 index;
                MI_OperationOptionsFlags flags;
                MI_Result result = this._operationOptionsHandle.Value.GetNumber("__MI_OPERATIONOPTIONS_PROMPTUSERMODE",
                                         out value,
                                         out index,
                                         out flags);
                CimException.ThrowIfMiResultFailure(result);
                return (CimCallbackMode)value;
            }
        }

        /// <summary>
        /// Sets WriteMessageCallback.
        /// </summary>
        /// <value></value>
        [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "See the justification on CimSessionOptions.Timeout")]
        public WriteMessageCallback WriteMessage
        {
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.AssertNotDisposed();
                _writeMessageCallback = value;
                // TODO: Get callbacks working
                //OperationCallback.writeMessage = this.WriteMessageCallbackInternal;
            }
        }

        /// <summary>
        /// Sets WriteProgressCallback.
        /// </summary>
        /// <value></value>
        [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "See the justification on CimSessionOptions.Timeout")]
        public WriteProgressCallback WriteProgress
        {
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.AssertNotDisposed();
                _writeProgressCallback = value;
                // TODO: Get callbacks working
                //OperationCallback.WriteProgressCallback = this.WriteProgressCallbackInternal;
            }
        }

        /// <summary>
        /// Sets WriteErrorCallback.
        /// </summary>
        /// <value></value>
        [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "See the justification on CimSessionOptions.Timeout")]
        public WriteErrorCallback WriteError
        {
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.AssertNotDisposed();
                _writeErrorCallback = value;
                // TODO: Get callbacks working
                //OperationCallback.WriteErrorCallback = this.WriteErrorCallbackInternal;
            }
        }

        /// <summary>
        /// Sets PromptUserCallback.
        /// </summary>
        /// <value></value>
        [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "See the justification on CimSessionOptions.Timeout")]
        public PromptUserCallback PromptUser
        {
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.AssertNotDisposed();
                _promptUserCallback = value;
                // TODO: Get callbacks working
                //OperationCallback.PromptUserCallback = this.PromptUserCallbackInternal;
            }
        }

        /// <summary>
        /// Enable the Channel for this particualr value.
        /// </summary>
        /// <param name="channelNumber"></param>
        public void EnableChannel(UInt32 channelNumber)
        {
            this.AssertNotDisposed();

            MI_Result result = this._operationOptionsHandle.Value.SetNumber("__MI_OPERATIONOPTIONS_CHANNEL",
                                        channelNumber,
                                        (MI_OperationOptionsFlags)0);
            CimException.ThrowIfMiResultFailure(result);
        }

        /// <summary>
        /// Disable the Channel for this particualr value.
        /// </summary>
        /// <param name="channelNumber"></param>
        public void DisableChannel(UInt32 channelNumber)
        {
            this.AssertNotDisposed();

            MI_Result result = this._operationOptionsHandle.Value.SetNumber("__MI_OPERATIONOPTIONS_CHANNEL",
                                        channelNumber,
                                        (MI_OperationOptionsFlags)1);
            CimException.ThrowIfMiResultFailure(result);
        }

        #endregion PSSEMANTICS

        /// <summary>
        /// Sets a custom server or CIM provider option
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="optionName"/> is <c>null</c></exception>
        public void SetCustomOption(string optionName, bool optionValue, bool mustComply)
        {
            this.SetCustomOption(optionName, optionValue, CimType.Boolean, mustComply);
        }

        /// <summary>
        /// Sets a custom server or CIM provider option
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="optionName"/> is <c>null</c></exception>
        public void SetCustomOption(string optionName, Byte optionValue, bool mustComply)
        {
            this.SetCustomOption(optionName, optionValue, CimType.UInt8, mustComply);
        }

        /// <summary>
        /// Sets a custom server or CIM provider option
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="optionName"/> is <c>null</c></exception>
        public void SetCustomOption(string optionName, SByte optionValue, bool mustComply)
        {
            this.SetCustomOption(optionName, optionValue, CimType.SInt8, mustComply);
        }

        /// <summary>
        /// Sets a custom server or CIM provider option
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="optionName"/> is <c>null</c></exception>
        public void SetCustomOption(string optionName, UInt16 optionValue, bool mustComply)
        {
            this.SetCustomOption(optionName, optionValue, CimType.UInt16, mustComply);
        }

        /// <summary>
        /// Sets a custom server or CIM provider option
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="optionName"/> is <c>null</c></exception>
        public void SetCustomOption(string optionName, Int16 optionValue, bool mustComply)
        {
            this.SetCustomOption(optionName, optionValue, CimType.SInt16, mustComply);
        }

        /// <summary>
        /// Sets a custom server or CIM provider option
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="optionName"/> is <c>null</c></exception>
        public void SetCustomOption(string optionName, UInt32 optionValue, bool mustComply)
        {
            this.SetCustomOption(optionName, optionValue, CimType.UInt32, mustComply);
        }

        /// <summary>
        /// Sets a custom server or CIM provider option
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="optionName"/> is <c>null</c></exception>
        public void SetCustomOption(string optionName, Int32 optionValue, bool mustComply)
        {
            this.SetCustomOption(optionName, optionValue, CimType.SInt32, mustComply);
        }

        /// <summary>
        /// Sets a custom server or CIM provider option
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="optionName"/> is <c>null</c></exception>
        public void SetCustomOption(string optionName, UInt64 optionValue, bool mustComply)
        {
            this.SetCustomOption(optionName, optionValue, CimType.UInt32, mustComply);
        }

        /// <summary>
        /// Sets a custom server or CIM provider option
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="optionName"/> is <c>null</c></exception>
        public void SetCustomOption(string optionName, Int64 optionValue, bool mustComply)
        {
            this.SetCustomOption(optionName, optionValue, CimType.SInt32, mustComply);
        }

        /// <summary>
        /// Sets a custom server or CIM provider option
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="optionName"/> is <c>null</c></exception>
        public void SetCustomOption(string optionName, Single optionValue, bool mustComply)
        {
            this.SetCustomOption(optionName, optionValue, CimType.Real32, mustComply);
        }

        /// <summary>
        /// Sets a custom server or CIM provider option
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="optionName"/> is <c>null</c></exception>
        public void SetCustomOption(string optionName, Double optionValue, bool mustComply)
        {
            this.SetCustomOption(optionName, optionValue, CimType.Real64, mustComply);
        }

        /// <summary>
        /// Sets a custom server or CIM provider option
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="optionName"/> is <c>null</c></exception>
        public void SetCustomOption(string optionName, char optionValue, bool mustComply)
        {
            this.SetCustomOption(optionName, optionValue, CimType.Char16, mustComply);
        }

        /// <summary>
        /// Sets a custom server or CIM provider option
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="optionName"/> or <paramref name="optionValue"/> is <c>null</c></exception>
        public void SetCustomOption(string optionName, string optionValue, bool mustComply)
        {
            this.SetCustomOption(optionName, optionValue, CimType.String, mustComply);
        }

        /// <summary>
        /// Sets a custom server or CIM provider option
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="optionName"/> is <c>null</c></exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="optionName"/> or <paramref name="optionValue"/> is <c>null</c></exception>
        public void SetCustomOption(string optionName, object optionValue, CimType cimType, bool mustComply)
        {
            if (string.IsNullOrWhiteSpace(optionName))
            {
                throw new ArgumentNullException("optionName");
            }
            if (optionValue == null)
            {
                throw new ArgumentNullException("optionValue");
            }
            this.AssertNotDisposed();

            MI_Value nativeLayerValue;
            try
            {
                nativeLayerValue = ValueHelpers.ConvertToNativeLayer(optionValue, cimType);
                ValueHelpers.ThrowIfMismatchedType(cimType.FromCimType(), nativeLayerValue);
            }
            catch (InvalidCastException e)
            {
                throw new ArgumentException(e.Message, "optionValue", e);
            }
            catch (FormatException e)
            {
                throw new ArgumentException(e.Message, "optionValue", e);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException(e.Message, "optionValue", e);
            }

            MI_OperationOptionsFlags flags = MI_OperationOptionsFlags.Unused;
            MI_Result result = this.OperationOptionsHandleOnDemand.SetCustomOption(
                optionName,
                cimType.FromCimType(),
        nativeLayerValue,
                mustComply,
        flags);
            CimException.ThrowIfMiResultFailure(result);
        }

        /// <summary>
        /// CancellationToken that can be used to cancel the operation.
        /// </summary>
        public CancellationToken? CancellationToken { get; set; }

        /// <summary>
        /// Asks the operation to only return key properties (see <see cref="CimFlags.Key"/>) of resulting CIM instances.
        /// Example of a CIM operation that supports this option: <see cref="CimSession.EnumerateInstances(string, string)"/>.
        /// </summary>
        public bool KeysOnly { get; set; }

        /// <summary>
        /// Asks the operation to only return class name (see <see cref="CimFlags.Key"/>) of resulting CIM class.
        /// Example of a CIM operation that supports this option: <see cref="CimSession.EnumerateInstances(string, string)"/>.
        /// </summary>
        public bool ClassNamesOnly { get; set; }

        /// <summary>
        /// Operation flags.
        /// </summary>
        public CimOperationFlags Flags { get; set; }

        /// <summary>
        /// report operation started flag
        /// </summary>
        public bool ReportOperationStarted { get { return (Flags & CimOperationFlags.ReportOperationStarted) == CimOperationFlags.ReportOperationStarted; } }

        /// <summary>
        /// Enables streaming of method results.
        /// See
        /// <see cref="CimSession.InvokeMethodAsync(string, string, string, CimMethodParametersCollection)"/>,
        /// <see cref="CimSession.InvokeMethodAsync(string, CimInstance, string, CimMethodParametersCollection)"/>,
        /// <see cref="CimMethodStreamedResult" />,
        /// <see cref="CimMethodResult" />,
        /// <see cref="CimMethodResultBase" />.
        /// </summary>
        public bool EnableMethodResultStreaming { get; set; }

        /// <summary>
        /// When <see cref="ShortenLifetimeOfResults"/> is set to <c>true</c>, then
        /// returned results (for example <see cref="CimInstance"/> objects) are
        /// valid only for a short duration.  This can improve performance by allowing
        /// to avoid copying of data from the transport layer.
        ///
        /// Shorter lifetime means that:
        /// - argument of IObserver.OnNext is disposed when OnNext returns
        /// - previous value of IEnumerator.Current is disposed after calling IEnumerator.MoveNext
        /// - value of IEnumerator.Current is disposed after calling IEnumerator.Dispose
        /// </summary>
        public bool ShortenLifetimeOfResults { get; set; }

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
            if (Interlocked.CompareExchange(ref this._disposed, 1, 0) == 0)
            {
                if (disposing)
                {
                    MI_OperationOptions tmpHandle = this.OperationOptionsHandle;
                    if (tmpHandle != null)
                    {
                        tmpHandle.Delete();
                    }
                }
            }
        }

        public bool IsDisposed
        {
            get
            {
                return this._disposed == 1;
            }
        }

        internal void AssertNotDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
        }

        private int _disposed;

        #endregion IDisposable Members

        #region ICloneable Members

#if (!_CORECLR)

        object ICloneable.Clone()
        {
            return new CimOperationOptions(this);
        }

#endif

        #endregion ICloneable Members
    }
}

namespace Microsoft.Management.Infrastructure.Options.Internal
{
    internal static class OperationOptionsExtensionMethods
    {
        static internal MI_OperationCallbacks GetOperationCallbacks(this CimOperationOptions operationOptions)
        {
            var operationCallbacks = new MI_OperationCallbacks();
            if (operationOptions != null)
            {
                // TODO: Uncomment these
                //operationCallbacks.writeError = operationOptions.OperationCallback.WriteErrorCallback;
                //operationCallbacks.writeMessage = operationOptions.OperationCallback.WriteMessageCallback;
                //operationCallbacks.writeProgress = operationOptions.OperationCallback.WriteProgressCallback;
                //operationCallbacks.promptUser = operationOptions.OperationCallback.PromptUserCallback;
            }
            return operationCallbacks;
        }

        static internal MI_OperationCallbacks GetOperationCallbacks(
            this CimOperationOptions operationOptions,
            CimAsyncCallbacksReceiverBase acceptCallbacksReceiver)
        {
            MI_OperationCallbacks operationCallbacks = operationOptions.GetOperationCallbacks();

            if (acceptCallbacksReceiver != null)
            {
                acceptCallbacksReceiver.RegisterAcceptedAsyncCallbacks(operationCallbacks, operationOptions);
            }

            return operationCallbacks;
        }

        static internal MI_OperationFlags GetOperationFlags(this CimOperationOptions operationOptions)
        {
            return operationOptions != null
                       ? operationOptions.Flags.ToNative()
                       : CimOperationFlags.None.ToNative();
        }

        static internal MI_OperationOptions GetOperationOptionsHandle(this CimOperationOptions operationOptions)
        {
            return operationOptions != null
                       ? operationOptions.OperationOptionsHandle
                       : null;
        }

        static internal bool GetKeysOnly(this CimOperationOptions operationOptions)
        {
            return operationOptions != null && operationOptions.KeysOnly;
        }

        static internal bool GetClassNamesOnly(this CimOperationOptions operationOptions)
        {
            return operationOptions != null && operationOptions.ClassNamesOnly;
        }

        static internal CancellationToken? GetCancellationToken(this CimOperationOptions operationOptions)
        {
            return operationOptions != null
                       ? operationOptions.CancellationToken
                       : null;
        }

        static internal bool GetShortenLifetimeOfResults(this CimOperationOptions operationOptions)
        {
            return operationOptions != null
                       ? operationOptions.ShortenLifetimeOfResults
                       : false;
        }

        static internal bool GetReportOperationStarted(this CimOperationOptions operationOptions)
        {
            return operationOptions != null
                       ? operationOptions.ReportOperationStarted
                       : false;
        }
    }
}
