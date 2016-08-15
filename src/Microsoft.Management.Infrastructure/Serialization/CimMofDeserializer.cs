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
using Microsoft.Management.Infrastructure.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.Management.Infrastructure.Serialization
{
    /// <summary>
    /// Represents an CIM deserializer.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Deserializer", Justification = "Deserializer is a valid word?")]
    internal class CimMofDeserializer : IDisposable
    {
        private readonly MI_Deserializer _myHandle;

        #region Constructors

        private CimMofDeserializer()
        {
            MI_Deserializer tmpHandle;
            MI_Result result = CimApplication.Handle.NewDeserializer(MI_SerializerFlags.None, MI_SerializationFormat.MOF, out tmpHandle);
            CimException.ThrowIfMiResultFailure(result);
            this._myHandle = tmpHandle;
            this.SchemaValidationOption = MofDeserializerSchemaValidationOption.Default;
        }

        /// <summary>
        /// delegate to set the GetClass Callback.
        /// </summary>
        /// <value></value>
        /// <returns>null if failed to find the class</returns>
        public delegate CimClass OnClassNeeded(
            string serverName,
            string namespaceName,
            string className);

        /// <summary>
        /// Create an internal delegate on demand
        /// </summary>
        /// <param name="wrappedcallback"></param>
        /// <returns></returns>
        internal static MI_Deserializer.MI_Deserializer_ClassObjectNeeded CreateClassObjectNeededCallbackDelegate(OnClassNeeded wrappedcallback)
        {
            return delegate(string serverName,
                string namespaceName,
                string className,
                out MI_Class classHandle)
            {
                CimClass cimClass = null;
                classHandle = null;
                cimClass = wrappedcallback(serverName, namespaceName, className);
                if (cimClass != null)
                {
                    classHandle = cimClass.ClassHandle;
                }
                return (classHandle != null) ? MI_Result.MI_RESULT_OK : MI_Result.MI_RESULT_FAILED;
            };
        }

        /// <summary>
        /// delegate to get and free included file content
        /// </summary>
        /// <value></value>
        /// <returns>true means sucess; false means failed to load included file</returns>
        public delegate byte[] GetIncludedFileContent(string fileName);

        // TODO: Missing MI_Deserializer_ClassObjectNeeded from C# MI API. Commented out section below (and references)
        /// <summary>
        /// Create an internal delegate on demand
        /// </summary>
        /// <param name="wrappedcallback"></param>
        /// <returns></returns>
        /*
            internal static MI_DeserializerCallbacks.GetIncludedFileBufferCallbackDelegate CreateGetIncludedFileBufferCallback(
                GetIncludedFileContent wrappedcallback)
            {
                return delegate(string fileName,
                    out byte[] fileBuffer)
                {
                    fileBuffer = wrappedcallback(fileName);
                    return (fileBuffer != null);
                };
            }
        */

        /// <summary>
        /// Schema validation option for deserializing instance(s)
        /// </summary>
        public MofDeserializerSchemaValidationOption SchemaValidationOption { get; set; }

        private CimOperationOptions GetOperationOptions()
        {
            CimOperationOptions options = new CimOperationOptions();
            switch (SchemaValidationOption)
            {
                case MofDeserializerSchemaValidationOption.Default:
                    options.SetOption("SchemaValidation", "Default");
                    break;

                case MofDeserializerSchemaValidationOption.Ignore:
                    options.SetOption("SchemaValidation", "IgnoreSchema");
                    break;

                case MofDeserializerSchemaValidationOption.IgnorePropertyType:
                    options.SetOption("SchemaValidation", "IgnorePropertyType");
                    break;

                case MofDeserializerSchemaValidationOption.Loose:
                    options.SetOption("SchemaValidation", "Loose");
                    break;

                case MofDeserializerSchemaValidationOption.Strict:
                    options.SetOption("SchemaValidation", "Strict");
                    break;
            }
            return options;
        }

        /// <summary>
        /// Instantiates a default deserializer
        /// </summary>
        public static CimMofDeserializer Create()
        {
            return new CimMofDeserializer();
        }

        #endregion Constructors

        #region Methods

        internal MI_Instance[] DeserializeInstanceHandle(
            byte[] serializedData,
            ref uint offset,
            IEnumerable<CimClass> cimClasses,
            OnClassNeeded onClassNeededCallback,
            GetIncludedFileContent getIncludedFileCallback)
        {
            if (serializedData == null)
            {
                throw new ArgumentNullException("serializedData");
            }
            else if (offset >= serializedData.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            else if (offset != 0)
            {
                // Need to implement this in our layer as it is not
                // handled by the underlying API
                throw new NotImplementedException();
            }
            else if (getIncludedFileCallback != null)
            {
                // Still need to add the native definitions of these callbacks
                throw new NotImplementedException();
            }
            this.AssertNotDisposed();

            MI_Class[] nativeClassHandles = null;
            if (cimClasses != null)
            {
                nativeClassHandles = cimClasses.Select(cimClass => cimClass.ClassHandle).ToArray();
            }
            
            UInt32 inputBufferUsed;
            MI_Instance cimError;
            MI_ExtendedArray instanceArray;
            MI_OperationOptions nativeOption = GetOperationOptions().OperationOptionsHandle;

            // TODO: Add definitions for these callbacks
            //if (getIncludedFileCallback != null) callbacks.GetIncludedFileBufferCallback = CreateGetIncludedFileBufferCallback(getIncludedFileCallback);
            MI_DeserializerCallbacks callbacks = new MI_DeserializerCallbacks();

            if (onClassNeededCallback != null)
            {
                callbacks.classObjectNeeded = CreateClassObjectNeededCallbackDelegate(onClassNeededCallback);
            }

            MI_Result result = this._myHandle.DeserializeInstanceArray(
                MI_SerializerFlags.None,
                nativeOption,
                callbacks,
                serializedData,
                nativeClassHandles,
                out inputBufferUsed,
                out instanceArray,
                out cimError);

            CimException.ThrowIfMiResultFailure(result, cimError);

            MI_Instance[] deserializedInstances = instanceArray.ReadAsManagedPointerArray(MI_Instance.NewFromDirectPtr);
            MI_Instance[] resultInstances = deserializedInstances.CloneMIArray();
            instanceArray.Delete();

            offset += inputBufferUsed;
            return resultInstances;
        }

        /// <summary>
        /// Deserializes a CIM instance
        /// </summary>
        /// <param name="serializedData">buffer with the serialized data</param>
        /// <param name="offset">
        /// Offset where to start reading the data.
        /// When the method returns, the offset will be pointing to the next byte after the deserialied instance.
        /// </param>
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Have to return 2 things.  Wrapping those 2 things in a class will result in a more, not less complexity")]
        public IEnumerable<CimInstance> DeserializeInstances(byte[] serializedData, ref uint offset)
        {
            return this.DeserializeInstances(serializedData, ref offset, null, null, null);
        }

        /// <summary>
        /// Deserializes a CIM instance with callbacks
        /// </summary>
        /// <param name="serializedData">buffer with the serialized data</param>
        /// <param name="offset">
        /// Offset where to start reading the data.
        /// When the method returns, the offset will be pointing to the next byte after the deserialied instance.
        /// </param>
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Have to return 2 things.  Wrapping those 2 things in a class will result in a more, not less complexity")]
        public IEnumerable<CimInstance> DeserializeInstances(byte[] serializedData, ref uint offset, OnClassNeeded onClassNeededCallback,
            GetIncludedFileContent getIncludedFileCallback)
        {
            return this.DeserializeInstances(serializedData, ref offset, null, onClassNeededCallback, getIncludedFileCallback);
        }

        /// <summary>
        /// Deserializes a CIM instance
        /// </summary>
        /// <param name="serializedData">buffer with the serialized data</param>
        /// <param name="offset">
        /// Offset where to start reading the data.
        /// When the method returns, the offset will be pointing to the next byte after the deserialied instance.
        /// </param>
        /// <param name="cimClasses">Optional cache of classes... (I can't concisely explain what this does... maybe Paul can...)</param>
        /// <returns>Deserialized CIM instance</returns>
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Have to return 2 things.  Wrapping those 2 things in a class will result in a more, not less complexity")]
        public IEnumerable<CimInstance> DeserializeInstances(
            byte[] serializedData,
            ref uint offset,
            IEnumerable<CimClass> cimClasses,
            OnClassNeeded onClassNeededCallback,
            GetIncludedFileContent getIncludedFileCallback)
        {
            List<CimInstance> instancelist = new List<CimInstance>();
            MI_Instance[] instanceHandles = DeserializeInstanceHandle(serializedData, ref offset, cimClasses, onClassNeededCallback, getIncludedFileCallback);
            foreach (MI_Instance handle in instanceHandles)
            {
                instancelist.Add(new CimInstance(handle));
            }
            return instancelist;
        }

        internal MI_Class[] DeserializeClassHandle(
            byte[] serializedData,
            ref uint offset,
            IEnumerable<CimClass> cimClasses,
            string computerName,
            string namespaceName,
            OnClassNeeded onClassNeededCallback,
            GetIncludedFileContent getIncludedFileCallback)
        {
            if (serializedData == null)
            {
                throw new ArgumentNullException("serializedData");
            }
            else if (offset >= serializedData.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            else if (getIncludedFileCallback != null)
            {
                // Need the definition for the callbacks
                throw new NotImplementedException();
            }
            else if (offset != 0)
            {
                // Need to internally handle the offset as the native calls
                // have no knowledge of this right now
                throw new NotImplementedException();
            }

            this.AssertNotDisposed();

            MI_Class[] nativeClassHandles = null;
            if (cimClasses != null)
            {
                nativeClassHandles = cimClasses.Select(cimClass => cimClass.ClassHandle).ToArray();
            }
            
            UInt32 inputBufferUsed;
            MI_Instance cimError;
            MI_ExtendedArray classArray;
            MI_OperationOptions nativeOption = GetOperationOptions().OperationOptionsHandle;

            //MI_DeserializerCallbacks callbacks = new MI_DeserializerCallbacks();
            //if (onClassNeededCallback != null) callbacks.ClassObjectNeededCallback = CreateClassObjectNeededCallbackDelegate(onClassNeededCallback);
            //if (getIncludedFileCallback != null) callbacks.GetIncludedFileBufferCallback = CreateGetIncludedFileBufferCallback(getIncludedFileCallback);
            MI_DeserializerCallbacks callbacks = new MI_DeserializerCallbacks();

            if (onClassNeededCallback != null)
            {
                callbacks.classObjectNeeded = CreateClassObjectNeededCallbackDelegate(onClassNeededCallback);
            }

            MI_Result result = this._myHandle.DeserializeClassArray(
                MI_SerializerFlags.None,
                nativeOption,
                callbacks,
                serializedData,
                nativeClassHandles,
                computerName,
                namespaceName,
                out inputBufferUsed,
                out classArray,
                out cimError);
            CimException.ThrowIfMiResultFailure(result, cimError);
            
            MI_Class[] deserializedClasses = classArray.ReadAsManagedPointerArray(MI_Class.NewFromDirectPtr);
            MI_Class[] resultClasses = deserializedClasses.CloneMIArray();
            classArray.Delete();

            offset += inputBufferUsed;
            return resultClasses;
        }

        /// <summary>
        /// Deserializes a CIM class
        /// </summary>
        /// <param name="serializedData">buffer with the serialized data</param>
        /// <param name="offset">
        /// Offset where to start reading the data.
        /// When the method returns, the offset will be pointing to the next byte after the deserialied class.
        /// </param>
        /// <returns>Deserialized CIM class</returns>
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Have to return 2 things.  Wrapping those 2 things in a class will result in a more, not less complexity")]
        public IEnumerable<CimClass> DeserializeClasses(byte[] serializedData, ref uint offset)
        {
            return this.DeserializeClasses(serializedData, ref offset, null);
        }

        /// <summary>
        /// Deserializes a CIM class
        /// </summary>
        /// <param name="serializedData">buffer with the serialized data</param>
        /// <param name="offset">
        /// Offset where to start reading the data.
        /// When the method returns, the offset will be pointing to the next byte after the deserialied class.
        /// </param>
        /// <returns>Deserialized CIM class</returns>
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Have to return 2 things.  Wrapping those 2 things in a class will result in a more, not less complexity")]
        public IEnumerable<CimClass> DeserializeClasses(byte[] serializedData, ref uint offset, IEnumerable<CimClass> classes)
        {
            return DeserializeClasses(serializedData, ref offset, classes, null, null, null, null);
        }

        /// <summary>
        /// Deserializes a CIM class
        /// </summary>
        /// <param name="serializedData">buffer with the serialized data</param>
        /// <param name="offset">
        /// Offset where to start reading the data.
        /// When the method returns, the offset will be pointing to the next byte after the deserialied class.
        /// </param>
        /// <returns>Deserialized CIM class</returns>
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Have to return 2 things.  Wrapping those 2 things in a class will result in a more, not less complexity")]
        public IEnumerable<CimClass> DeserializeClasses(byte[] serializedData,
            ref uint offset,
            IEnumerable<CimClass> classes,
            OnClassNeeded onClassNeededCallback,
            GetIncludedFileContent getIncludedFileCallback)
        {
            return DeserializeClasses(serializedData, ref offset, classes, null, null, onClassNeededCallback, getIncludedFileCallback);
        }

        /// <summary>
        /// Deserializes a CIM class
        /// </summary>
        /// <param name="serializedData">buffer with the serialized data</param>
        /// <param name="offset">
        /// Offset where to start reading the data.
        /// When the method returns, the offset will be pointing to the next byte after the deserialied class.
        /// </param>
        /// <param name="parentClass">Optional parent class... (I can't concisely explain what this does and why this is needed... maybe Paul can...)</param>
        /// <returns>Deserialized CIM class</returns>
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Have to return 2 things.  Wrapping those 2 things in a class will result in a more, not less complexity")]
        public IEnumerable<CimClass> DeserializeClasses(byte[] serializedData,
            ref uint offset,
            IEnumerable<CimClass> classes,
            string computerName,
            string namespaceName,
            OnClassNeeded onClassNeededCallback,
            GetIncludedFileContent getIncludedFileCallback)
        {
            MI_Class[] classHandles = DeserializeClassHandle(
                serializedData,
                ref offset,
                classes,
                computerName,
                namespaceName,
                onClassNeededCallback,
                getIncludedFileCallback);
            List<CimClass> classlist = new List<CimClass>();
            foreach (MI_Class handle in classHandles)
            {
                classlist.Add(new CimClass(handle));
            }
            return classlist;
        }

        #endregion Methods

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
        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // TODO: When MI API supports Delete/Dispose, uncomment
                //this._myHandle.Dispose();
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
    }
}
