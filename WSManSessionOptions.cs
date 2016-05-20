/*============================================================================
 * Copyright (C) Microsoft Corporation, All rights reserved. 
 *============================================================================
 */

using System;
using Microsoft.Management.Infrastructure.Options.Internal;
using System.Globalization;
using NativeObject;

namespace Microsoft.Management.Infrastructure.Options
{
    /// <summary>
    /// Options of <see cref="CimSession"/> that uses WSMan as the transport protocol
    /// </summary>
    public class WSManSessionOptions : CimSessionOptions
    {
        /// <summary>
        /// Creates a new <see cref="WSManSessionOptions"/> instance
        /// </summary>
        public WSManSessionOptions()
            : base(MI_Protocol.WSMan)
        {
        }

        /// <summary>
        /// Instantiates a deep copy of <paramref name="optionsToClone"/>
        /// </summary>
        /// <param name="optionsToClone">options to clone</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="optionsToClone"/> is <c>null</c></exception>
        public WSManSessionOptions(WSManSessionOptions optionsToClone)
            : base(optionsToClone)
        {
        }

        // REVIEW PLEASE: native API uses MI_uint32 for the port number.  should we limit that to UInt16 in the managed layer?

        /// <summary>
        /// Sets destination port
        /// </summary>
        /// <value></value>
        public uint DestinationPort
        {
            set
            {
                this.AssertNotDisposed();
		// TODO: MI API is missing function below
                //MI_Result result = this.DestinationOptionsHandleOnDemand.SetDestinationPort(value);
                //CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();
                uint port = 0;
		// TODO: MI API is missing function below
                //MI_Result result = this.DestinationOptionsHandleOnDemand.GetDestinationPort(out port);
                //CimException.ThrowIfMiResultFailure(result);
                return port;
            }
        }

        /// <summary>
        /// Sets maximum size of SOAP envelope
        /// </summary>
        /// <value></value>
        public uint MaxEnvelopeSize
        {
            set
            {
                this.AssertNotDisposed();
		// TODO: MI API is missing function below
                //MI_Result result = this.DestinationOptionsHandleOnDemand.SetMaxEnvelopeSize(value);
                //CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();
                uint size = 0;
		// TODO: MI API is missing function below
                //MI_Result result = this.DestinationOptionsHandleOnDemand.GetMaxEnvelopeSize(out size);
                //CimException.ThrowIfMiResultFailure(result);
                return size;
            }
        }

        /// <summary>
        /// Sets whether the client should validate that the server certificate is signed by a trusted certificate authority (CA).
        /// </summary>
        /// <value></value>
        public bool CertCACheck
        {
            set
            {
                this.AssertNotDisposed();

		// TODO: MI API is missing function below
                //MI_Result result = this.DestinationOptionsHandleOnDemand.SetCertCACheck(value);
                //CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();
                bool check = false;
		// TODO: MI API is missing function below
		//MI_Result result = this.DestinationOptionsHandleOnDemand.GetCertCACheck(out check);
                //CimException.ThrowIfMiResultFailure(result);
                return check;
            }
        }

        /// <summary>
        /// Sets whether the client should validate that the certificate common name (CN) of the server matches the hostname of the server.
        /// </summary>
        /// <value></value>
        public bool CertCNCheck
        {
            set
            {
                this.AssertNotDisposed();
		// TODO: MI API is missing function below
                //MI_Result result = this.DestinationOptionsHandleOnDemand.SetCertCNCheck(value);
                //CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();
                bool check = false;
		// TODO: MI API is missing function below
		//MI_Result result = this.DestinationOptionsHandleOnDemand.GetCertCNCheck(out check);
                //CimException.ThrowIfMiResultFailure(result);
                return check;
            }
        }

        /// <summary>
        /// Sets whether the client should validate the revocation status of the server certificate.
        /// </summary>
        /// <value></value>
        public bool CertRevocationCheck
        {
            set
            {
                this.AssertNotDisposed();
		// TODO: MI API is missing function below
                //MI_Result result = this.DestinationOptionsHandleOnDemand.SetCertRevocationCheck(value);
                //CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();
                bool check = false;
		// TODO: MI API is missing function below
                //MI_Result result = this.DestinationOptionsHandleOnDemand.GetCertRevocationCheck(out check);
                //CimException.ThrowIfMiResultFailure(result);
                return check;
            }
        }

        /// <summary>
        /// Sets whether the client should use SSL.
        /// </summary>
        /// <value></value>
        public bool UseSsl
        {
            set
            {
                this.AssertNotDisposed();

                if (value)
                {
		    // TODO: MI API is missing function below
                    //MI_Result result = this.DestinationOptionsHandleOnDemand.SetTransport(MI_Transport.HTTPS);
                    //CimException.ThrowIfMiResultFailure(result);
                }
                else
                {
		    // TODO: MI API is missing function below
                    //MI_Result result = this.DestinationOptionsHandleOnDemand.SetTransport(MI_Transport.HTTP);
                    //CimException.ThrowIfMiResultFailure(result);
                }
            }
            get
            {
                this.AssertNotDisposed();
                string transport = "";
		// TODO: MI API is missing function below
                //MI_Result result = this.DestinationOptionsHandleOnDemand.GetTransport(out transport);
                //CimException.ThrowIfMiResultFailure(result);
#if(!_CORECLR)
                if ( string.Compare( transport, MI_Transport.HTTPS, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase ) == 0 )
#else
                if ( string.Compare( transport, MI_Transport.HTTPS, StringComparison.CurrentCultureIgnoreCase ) == 0 )
#endif
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Sets type of HTTP proxy.
        /// </summary>
        /// <value></value>
        public ProxyType ProxyType
        {
            set
            {
                this.AssertNotDisposed();

		// TODO: MI API is missing function below
                //string nativeProxyType = value.ToNativeType();
                //MI_Result result = this.DestinationOptionsHandleOnDemand.SetProxyType(nativeProxyType);
                //CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();
                string type = "";
		// TODO: MI API is missing function below
                //MI_Result result = this.DestinationOptionsHandleOnDemand.GetProxyType(out type);
                //CimException.ThrowIfMiResultFailure(result);
                return ProxyTypeExtensionMethods.FromNativeType(type);
            }
        }

        /// <summary>
        /// Sets packet encoding.
        /// </summary>
        /// <value></value>
        public PacketEncoding PacketEncoding
        {
            set
            {
                this.AssertNotDisposed();

                string nativePacketEncoding = value.ToNativeType();
		// TODO: MI API is missing function below
                //MI_Result result = this.DestinationOptionsHandleOnDemand.SetPacketEncoding(nativePacketEncoding);
                //CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();

                string nativePacketEncoding = "";
		// TODO: MI API is missing function below
                //MI_Result result = this.DestinationOptionsHandleOnDemand.GetPacketEncoding(out nativePacketEncoding);
                //CimException.ThrowIfMiResultFailure(result);
                return PacketEncodingExtensionMethods.FromNativeType(nativePacketEncoding);
            }
        }

        /// <summary>
        /// Sets packet privacy
        /// </summary>
        /// <value></value>
        public bool NoEncryption
        {
            set
            {
                this.AssertNotDisposed();

                bool noEncryption = value;
                bool packetPrivacy = !noEncryption;
		// TODO: MI API is missing function below
                //MI_Result result = this.DestinationOptionsHandleOnDemand.SetPacketPrivacy(packetPrivacy);
                //CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();
                bool packetPrivacy = false;
		// TODO: MI API is missing function below
                //MI_Result result = this.DestinationOptionsHandleOnDemand.GetPacketPrivacy(out packetPrivacy);
                //CimException.ThrowIfMiResultFailure(result);
                bool noEncryption = !packetPrivacy;
                return noEncryption;
            }
        }

        /// <summary>
        /// Sets whether to encode port in service principal name (SPN).
        /// </summary>
        /// <value></value>
        public bool EncodePortInServicePrincipalName
        {
            set
            {
                this.AssertNotDisposed();

		// TODO: MI API is missing function below
                //MI_Result result = this.DestinationOptionsHandleOnDemand.SetEncodePortInSPN(value);
                //CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();
                bool encodePortInServicePrincipalName = false;
		// TODO: MI API is missing function below
                //MI_Result result = this.DestinationOptionsHandleOnDemand.GetEncodePortInSPN(out encodePortInServicePrincipalName);
                //CimException.ThrowIfMiResultFailure(result);
                return encodePortInServicePrincipalName;
            }
        }

        /// <summary>
        /// Sets http url prefix.
        /// </summary>
        /// <value></value>
        public Uri HttpUrlPrefix
        {
            set
            {
                if (value == null) // empty string ok
                {
                    throw new ArgumentNullException("value");
                }
                this.AssertNotDisposed();

		// TODO: MI API is missing function below
                //MI_Result result = this.DestinationOptionsHandleOnDemand.SetHttpUrlPrefix(value.ToString());
                //CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();
                string httpUrlPrefix = "";
		// TODO: MI API is missing function below
                //MI_Result result = this.DestinationOptionsHandleOnDemand.GetHttpUrlPrefix(out httpUrlPrefix);
                //if (result != MI_Result.MI_RESULT_OK)
                //{
		//    return null;
	        //}
                try
                {
                    try
                    {
                        return new Uri(httpUrlPrefix, UriKind.Relative);
                    }
#if(!_CORECLR)
                    catch (UriFormatException)
#else
                    catch (FormatException)
#endif
                    {
                        return new Uri(httpUrlPrefix, UriKind.Absolute);
                    }
                }
#if(!_CORECLR)
                catch (UriFormatException)
#else
                catch (FormatException)
#endif
                {
                    return null;
                }
                catch (ArgumentException)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Sets a Proxy Credential
        /// </summary>
        /// <param name="credential"></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="credential"/> is <c>null</c></exception>
        public void AddProxyCredentials(CimCredential credential)
        {
            if (credential == null)
            {
                throw new ArgumentNullException("credential");
            }
            this.AssertNotDisposed();

	    // TODO: MI API is missing function below
	    //MI_Result result = this.DestinationOptionsHandleOnDemand.AddProxyCredentials(credential.GetCredential());
            //CimException.ThrowIfMiResultFailure(result);
        }
    }
}
