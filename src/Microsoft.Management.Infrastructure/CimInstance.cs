/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Management.Infrastructure.Generic;
using Microsoft.Management.Infrastructure.Internal;
using Microsoft.Management.Infrastructure.Internal.Data;
using Microsoft.Management.Infrastructure.Serialization;
using System.IO;

#if(!_CORECLR)

using Microsoft.Win32;

#endif

using Microsoft.Management.Infrastructure.Native;

namespace Microsoft.Management.Infrastructure
{
    /// <summary>
    /// Represents an CIM instance.
    /// </summary>
#if (!_CORECLR)

    [Serializable]
#endif
    public sealed class CimInstance : IDisposable
#if (!_CORECLR)
        //
        // Only implement these interfaces on FULL CLR and not Core CLR
        //
        , ICloneable, ISerializable
#endif
    {
        private readonly MI_Instance nativeInstance;
        private CimSystemProperties _systemProperties = null;

        internal MI_Instance InstanceHandle
        {
            get
            {
                this.AssertNotDisposed();
                return this.nativeInstance;
            }
        }

        #region Constructors

        ~CimInstance()
        {
            this.Dispose(false);
        }
        
        internal CimInstance(MI_Instance handle)
        {
            if (handle == null || handle.IsNull)
            {
                throw new ArgumentNullException();
            }

            this.nativeInstance = handle;
        }

        /// <summary>
        /// Instantiates a deep copy of <paramref name="cimInstanceToClone"/>
        /// </summary>
        /// <param name="cimInstanceToClone">Instance to clone</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="cimInstanceToClone"/> is <c>null</c></exception>
        public CimInstance(CimInstance cimInstanceToClone)
        {
            if (cimInstanceToClone == null)
            {
                throw new ArgumentNullException("cimInstanceToClone");
            }

            MI_Instance clonedHandle = cimInstanceToClone.InstanceHandle.Clone();
            this.nativeInstance = clonedHandle;
        }

        /// <summary>
        /// Instantiates an empty <see cref="CimInstance"/>.
        /// </summary>
        /// <remarks>
        /// This constructor provides a way to create CIM instances, without communicating with a CIM server.
        /// This constructor is typically used when the client knows all the key properties (<see cref="CimFlags.Key"/>)
        /// of the instance and wants to pass the instance as an argument of a CimSession method
        /// (for example as a "sourceInstance" parameter of <see cref="CimSession.EnumerateAssociatedInstances(string, CimInstance, string, string, string, string)"/>).
        /// <see cref="CimSession.EnumerateInstances(string,string)"/> or <see cref="CimSession.GetInstance(string, CimInstance)"/>.
        /// </remarks>
        /// <param name="className"></param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="className"/> is null or when it doesn't follow the format specified by DSP0004</exception>
        public CimInstance(string className)
        : this(className, null)
        {
        }

        /// <summary>
        /// Instantiates an empty <see cref="CimInstance"/>.
        /// </summary>
        /// <remarks>
        /// This constructor provides a way to create CIM instances, without communicating with a CIM server.
        /// This constructor is typically used when the client knows all the key properties (<see cref="CimFlags.Key"/>)
        /// of the instance and wants to pass the instance as an argument of a CimSession method
        /// (for example as a "sourceInstance" parameter of <see cref="CimSession.EnumerateAssociatedInstances(string, CimInstance, string, string, string, string)"/>).
        /// <see cref="CimSession.EnumerateInstances(string,string)"/> or <see cref="CimSession.GetInstance(string, CimInstance)"/>.
        /// </remarks>
        /// <param name="className"></param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="className"/> is null or when it doesn't follow the format specified by DSP0004</exception>
        public CimInstance(string className, string namespaceName)
        {
            if (className == null)
            {
                throw new ArgumentNullException("className");
            }

            MI_Instance tmpHandle;
            MI_Result result = CimApplication.Handle.NewInstance(className, null, out tmpHandle);
            if (result == MI_Result.MI_RESULT_INVALID_PARAMETER)
            {
                throw new ArgumentOutOfRangeException("className");
            }
            
            CimException.ThrowIfMiResultFailure(result);

            if (namespaceName != null)
            {
                result = tmpHandle.SetNameSpace(namespaceName);
                CimException.ThrowIfMiResultFailure(result);
            }

            this.nativeInstance = tmpHandle;
        }

        /// <summary>
        /// Instantiates an empty <see cref="CimInstance"/>.
        /// </summary>
        /// <param name="cimClass"></param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="cimClass"/> is null or when it doesn't follow the format specified by DSP0004</exception>
        public CimInstance(CimClass cimClass)
        {
            if (cimClass == null)
            {
                throw new ArgumentNullException("cimClass");
            }

            MI_Instance tmpHandle;
            MI_Result result = CimApplication.Handle.NewInstanceFromClass(cimClass.CimSystemProperties.ClassName, cimClass.ClassHandle, out tmpHandle);
            if (result == MI_Result.MI_RESULT_INVALID_PARAMETER)
            {
                throw new ArgumentOutOfRangeException("cimClass");
            }
            CimException.ThrowIfMiResultFailure(result);

            result = tmpHandle.SetNameSpace(cimClass.CimSystemProperties.Namespace);
            CimException.ThrowIfMiResultFailure(result);
            result = tmpHandle.SetServerName(cimClass.CimSystemProperties.ServerName);
            CimException.ThrowIfMiResultFailure(result);

            this.nativeInstance = tmpHandle;
        }

        #endregion Constructors

        #region Properties

        public CimClass CimClass
        {
            get
            {
                this.AssertNotDisposed();

                MI_Class classHandle;
                MI_Result result = this.InstanceHandle.GetClass(out classHandle);
                return ((classHandle == null) || (result != MI_Result.MI_RESULT_OK))
                           ? null
                           : new CimClass(classHandle);
            }
        }

        /// <summary>
        /// Properties of this CimInstance
        /// </summary>
        public CimKeyedCollection<CimProperty> CimInstanceProperties
        {
            get
            {
                this.AssertNotDisposed();
                return new CimPropertiesCollection(this);
            }
        }

        /// <summary>
        /// System Properties of this CimInstance
        /// </summary>
        public CimSystemProperties CimSystemProperties
        {
            get
            {
                this.AssertNotDisposed();
                if (_systemProperties == null)
                {
                    CimSystemProperties tmpSystemProperties = new CimSystemProperties();

                    // ComputerName
                    string tmpComputerName;
                    MI_Result result = this.InstanceHandle.GetServerName(out tmpComputerName);
                    CimException.ThrowIfMiResultFailure(result);

                    //ClassName
                    string tmpClassName;
                    result = this.InstanceHandle.GetClassName(out tmpClassName);
                    CimException.ThrowIfMiResultFailure(result);

                    //Namespace
                    string tmpNamespace;
                    result = this.InstanceHandle.GetNameSpace(out tmpNamespace);
                    CimException.ThrowIfMiResultFailure(result);
                    tmpSystemProperties.UpdateCimSystemProperties(tmpNamespace, tmpComputerName, tmpClassName);

                    //Path
                    tmpSystemProperties.UpdateSystemPath(CimInstance.GetCimSystemPath(tmpSystemProperties, null));
                    _systemProperties = tmpSystemProperties;
                }
                return _systemProperties;
            }
        }

        #endregion Properties

        #region Helpers

        /// <summary>
        /// Constructs the object path from the CimInstance.
        /// </summary>

        internal static string GetCimSystemPath(CimSystemProperties sysProperties, IEnumerator cimPropertiesEnumerator)
        {
            //Path should be supported by MI APIs, it is not currently supported by APIs
            // until that decision is taken we are reporting null for path.
            return null;
            /*
            string objectNamespace = sysProperties.Namespace;
            string objectComputerName = sysProperties.ServerName;
            string objectClassName = sysProperties.ClassName;
            StringBuilder strPath = new StringBuilder();
            if( objectComputerName != null )
            {
                strPath.Append("//");
                strPath.Append(objectComputerName);
                strPath.Append("/");
            }
            if( objectNamespace != null)
            {
                strPath.Append(objectNamespace);
                strPath.Append(":");
            }
            strPath.Append(objectClassName);
            //Now find the key properties
            IEnumerator cimProperties = cimPropertiesEnumerator;
            bool bFirst = true;
            while(cimProperties.MoveNext())
            {
                //Handle for both instance and class
                CimProperty instProp = cimProperties.Current as CimProperty;
                if( instProp != null)
                {
                    if(instProp.Value != null)
                    {
                        GetPathForProperty((long)instProp.Flags, instProp.Name, instProp.Value, ref bFirst, ref strPath);
                    }
                }
                else
                {
                    CimPropertyDeclaration classProp = cimProperties.Current as CimPropertyDeclaration;
                    if( classProp == null)
                    {
                        // this is not expected, should we throw from here?
                        return null;
                    }
                    if( classProp.Value != null )
                    {
                        GetPathForProperty((long)classProp.Flags, classProp.Name, classProp.Value, ref bFirst, ref strPath);
                    }
                }
            }
            return strPath.ToString();
            */
        }

        /*
                private static void  GetPathForProperty(long cimflags, string propName, object propValue, ref bool bFirst, ref StringBuilder strPath)
                {
                    long r = cimflags;
                    long l = (long) CimFlags.Key;

                    if( (r & l) != 0)
                    {
                        if(bFirst)
                        {
                            bFirst = false;
                            strPath.Append(".");
                        }
                        else
                        {
                            strPath.Append(",");
                        }
                        strPath.Append(propName);
                        strPath.Append("=");
                        strPath.Append("\"");
                        CimInstance innerInst = propValue as CimInstance;
                        string propValueStr;
                        if(innerInst == null )
                        {
                            propValueStr = (propValue).ToString();
                        }
                        else
                        {
                            propValueStr = GetCimSystemPath(innerInst.SystemProperties, innerInst.Properties.GetEnumerator());
                        }
                        strPath.Append(PutEscapeCharacterBack(propValueStr));
                        strPath.Append("\"");
                    }
                }

                private static string PutEscapeCharacterBack(string propValue)
                {
                    StringBuilder strPath = new StringBuilder();
                    for(int i = 0 ;i < propValue.Length;i++)
                    {
                        if( propValue[i] == '\"' || propValue[i] == '\\')
                        {
                            strPath.Append("\\");
                        }
                        strPath.Append(propValue[i]);
                    }
                    return strPath.ToString();
                }
                */

        #endregion Helpers

        #region IDisposable Members

        /// <summary>
        /// Releases resources associated with this object
        /// </summary>
        public void Dispose()
        {
            // Do not supress finalization of the object
            // since we don't do anything meaningful in the Dispose
            this.Dispose(true);
        }

        /// <summary>
        /// Releases resources associated with this object
        /// </summary>
        private void Dispose(bool disposing)
        {
            // This doesn't actually do anything right now
            // The original implementation relied on explicit disposes
            // because it was trying to internally ref-count a handle
            // We now always clone the MI_Instance and rely
            // on the Finalizer to clean up the actual memory
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Still prevent the caller from double-Disposing the object
                _disposed = true;
            }
            else
            {
                if (this.InstanceHandle != null && !this.InstanceHandle.IsNull)
                {
                    using (this.InstanceHandle)
                    {
                        this.InstanceHandle.Delete();
                    }
                }
            }
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

        #region .NET serialization

        private const string serializationId_MiXml = "MI_XML";
        private const string serializationId_CimSessionComputerName = "CSCN";

#if(!_CORECLR)

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            using (CimSerializer cimSerializer = CimSerializer.Create())
            {
                byte[] serializedBytes = cimSerializer.Serialize(this, InstanceSerializationOptions.IncludeClasses);
                string serializedString = Encoding.Unicode.GetString(serializedBytes);
                info.AddValue(serializationId_MiXml, serializedString);
            }
            info.AddValue(serializationId_CimSessionComputerName, this.GetCimSessionComputerName());
        }

        private CimInstance(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            string serializedString = info.GetString(serializationId_MiXml);
            byte[] serializedBytes = Encoding.Unicode.GetBytes(serializedString);
            using (CimDeserializer cimDeserializer = CimDeserializer.Create())
            {
                uint offset = 0;
                MI_Instance deserializedInstanceHandle = cimDeserializer.DeserializeInstanceHandle(
                    serializedBytes,
                    ref offset,
                    cimClasses: null);
                this.nativeInstance = deserializedInstanceHandle;
            }
            this.SetCimSessionComputerName(info.GetString(serializationId_CimSessionComputerName));
        }

#endif // !_CORECLR

        #endregion .NET serialization

        #region ICloneable Members

#if(!_CORECLR)

        object ICloneable.Clone()
        {
            return new CimInstance(this);
        }

#endif // !_CORECLR

        #endregion ICloneable Members

        #region Utility functions

        /// <summary>
        /// get cimsession instance id
        /// </summary>
        /// <returns></returns>
        public Guid GetCimSessionInstanceId()
        {
            return this._CimSessionInstanceID;
        }

        /// <summary>
        /// set cimsession instance id
        /// </summary>
        /// <param name="instanceID"></param>
        internal void SetCimSessionInstanceId(Guid instanceID)
        {
            this._CimSessionInstanceID = instanceID;
        }

        /// <summary>
        /// cimsession id that generated the instance,
        /// Guid.Empty means no session generated this instance
        /// </summary>
        private Guid _CimSessionInstanceID = Guid.Empty;

        /// <summary>
        /// get the computername of the session
        /// </summary>
        /// <returns></returns>
        public String GetCimSessionComputerName()
        {
            return this._CimSessionComputerName;
        }

        /// <summary>
        /// set the computername of the session
        /// </summary>
        /// <param name="computerName"></param>
        internal void SetCimSessionComputerName(String computerName)
        {
            this._CimSessionComputerName = computerName;
        }

        /// <summary>
        /// computername of a cimsession, which genereated the instance,
        /// null means no session generated this instance
        /// </summary>
        private string _CimSessionComputerName = null;

        #endregion Utility functions

        public override string ToString()
        {
            CimProperty captionProperty = this.CimInstanceProperties["Caption"];
            string captionValue = null;
            if (captionProperty != null)
            {
                captionValue = captionProperty.Value as string;
            }

            string keyValues = string.Join(", ", this.CimInstanceProperties.Where(p => CimFlags.Key == (p.Flags & CimFlags.Key)));

            string toStringValue;
            if (string.IsNullOrEmpty(keyValues) && (string.IsNullOrEmpty(captionValue)))
            {
                toStringValue = this.CimSystemProperties.ClassName;
            }
            else if (string.IsNullOrEmpty(captionValue))
            {
                toStringValue = string.Format(
                    CultureInfo.InvariantCulture,
                    Strings.CimInstanceToStringNoCaption,
                    this.CimSystemProperties.ClassName,
                    keyValues);
            }
            else if (string.IsNullOrEmpty(keyValues))
            {
                toStringValue = string.Format(
                    CultureInfo.InvariantCulture,
                    Strings.CimInstanceToStringNoKeys,
                    this.CimSystemProperties.ClassName,
                    captionValue);
            }
            else
            {
                toStringValue = string.Format(
                    CultureInfo.InvariantCulture,
                    Strings.CimInstanceToStringFullData,
                    this.CimSystemProperties.ClassName,
                    keyValues,
                    captionValue);
            }
            return toStringValue;
        }
    }
}
