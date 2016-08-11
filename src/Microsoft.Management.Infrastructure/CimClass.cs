/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


using Microsoft.Management.Infrastructure.Generic;
using Microsoft.Management.Infrastructure.Internal.Data;
using Microsoft.Management.Infrastructure.Native;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.Management.Infrastructure
{
    /// <summary>
    /// Represents an CIM Class.
    /// </summary>
    public sealed class CimClass : IDisposable
    {
        private CimSystemProperties _systemProperties = null;
        private MI_Class _classHandle;

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal MI_Class ClassHandle
        {
            get
            {
                this.AssertNotDisposed();
                return this._classHandle;
            }
        }

        #region Constructors

        internal CimClass(MI_Class handle)
        {
            Debug.Assert(handle != null, "Caller should verify that instanceHandle != null");

            this._classHandle = handle;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Name of the Super CIM class
        /// </summary>
        public string CimSuperClassName
        {
            get
            {
                this.AssertNotDisposed();

                string tmp;
                MI_Result result = this._classHandle.GetParentClassName(out tmp);
                switch (result)
                {
                    case MI_Result.MI_RESULT_INVALID_SUPERCLASS:
                        return null;

                    default:
                        CimException.ThrowIfMiResultFailure(result);
                        return tmp;
                }
            }
        }

        /// <summary>
        /// Super class schema
        /// </summary>
        public CimClass CimSuperClass
        {
            get
            {
                this.AssertNotDisposed();

                MI_Class tmp;
                MI_Result result = this._classHandle.GetParentClass(out tmp);
                switch (result)
                {
                    case MI_Result.MI_RESULT_INVALID_SUPERCLASS:
                        return null;

                    default:
                        CimException.ThrowIfMiResultFailure(result);
                        return new CimClass(tmp);
                }
            }
        }

        /// <summary>
        /// Properties of this CimClass
        /// </summary>
        public CimReadOnlyKeyedCollection<CimPropertyDeclaration> CimClassProperties
        {
            get
            {
                this.AssertNotDisposed();
                return new CimClassPropertiesCollection(this._classHandle);
            }
        }

        /// <summary>
        /// Qualifiers of this CimClass
        /// </summary>
        public CimReadOnlyKeyedCollection<CimQualifier> CimClassQualifiers
        {
            get
            {
                this.AssertNotDisposed();
                return new CimClassQualifierCollection(this._classHandle);
            }
        }

        /// <summary>
        /// Qualifiers of this CimClass
        /// </summary>
        public CimReadOnlyKeyedCollection<CimMethodDeclaration> CimClassMethods
        {
            get
            {
                this.AssertNotDisposed();
                return new CimMethodDeclarationCollection(this._classHandle);
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
                    MI_Result result = this._classHandle.GetServerName(out tmpComputerName);
                    CimException.ThrowIfMiResultFailure(result);

                    //ClassName
                    string tmpClassName;
                    result = this._classHandle.GetClassName(out tmpClassName);
                    CimException.ThrowIfMiResultFailure(result);

                    //Namespace
                    string tmpNamespace;
                    result = this._classHandle.GetNameSpace(out tmpNamespace);
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
                this._classHandle.Delete();
                this._classHandle = null;
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

        public override int GetHashCode()
        {
            // TODO: implement this function?
            //return this.ClassHandle.GetClassHashCode();
            return 0;
        }

        public override bool Equals(object obj)
        {
            return object.ReferenceEquals(this, obj);
        }

        public override string ToString()
        {
            return string.Format(
                    CultureInfo.InvariantCulture,
                    Strings.CimClassToString,
                    this.CimSystemProperties.Namespace,
                    this.CimSystemProperties.ClassName);
        }
    }
}
