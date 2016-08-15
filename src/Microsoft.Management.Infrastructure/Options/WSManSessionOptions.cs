/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


using Microsoft.Management.Infrastructure.Native;
using Microsoft.Management.Infrastructure.Options.Internal;
using System;
using System.Globalization;

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
                MI_Result result = this.DestinationOptionsHandleOnDemand.SetNumber("__MI_DESTINATIONOPTIONS_DESTINATION_PORT",
                                                   value,
                                                   MI_DestinationOptionsFlags.Unused);
                CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();
                UInt32 port;
                UInt32 index;
                MI_DestinationOptionsFlags flags;
                MI_Result result = this.DestinationOptionsHandleOnDemand.GetNumber("__MI_DESTINATIONOPTIONS_DESTINATION_PORT",
                                                   out port,
                                                   out index,
                                                   out flags);

                CimException.ThrowIfMiResultFailure(result);
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

                MI_Result result = this.DestinationOptionsHandleOnDemand.SetNumber("__MI_DESTINATIONOPTIONS_MAX_ENVELOPE_SIZE",
                                                   value,
                                                   MI_DestinationOptionsFlags.Unused);
                CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();
                UInt32 size;
                UInt32 index;
                MI_DestinationOptionsFlags flags;
                MI_Result result = this.DestinationOptionsHandleOnDemand.GetNumber("__MI_DESTINATIONOPTIONS_MAX_ENVELOPE_SIZE",
                                                   out size,
                                                   out index,
                                                   out flags);
                CimException.ThrowIfMiResultFailure(result);
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
                UInt32 checkInt = value == true ? (uint)1 : (uint)0;
                MI_Result result = this.DestinationOptionsHandleOnDemand.SetNumber("__MI_DESTINATIONOPTIONS_CERT_CA_CHECK",
                                                   checkInt,
                                                   MI_DestinationOptionsFlags.Unused);
                CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();
                UInt32 checkInt;
                UInt32 index;
                MI_DestinationOptionsFlags flags;
                MI_Result result = this.DestinationOptionsHandleOnDemand.GetNumber("__MI_DESTINATIONOPTIONS_CERT_CA_CHECK",
                                                   out checkInt,
                                                   out index,
                                                   out flags);
                CimException.ThrowIfMiResultFailure(result);
                bool checkBool = checkInt == 1 ? true : false;
                return checkBool;
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
                UInt32 checkInt = value == true ? (uint)1 : (uint)0;
                MI_Result result = this.DestinationOptionsHandleOnDemand.SetNumber("__MI_DESTINATIONOPTIONS_CERT_CN_CHECK",
                                                   checkInt,
                                                   MI_DestinationOptionsFlags.Unused);
                CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();
                UInt32 checkInt;
                UInt32 index;
                MI_DestinationOptionsFlags flags;
                MI_Result result = this.DestinationOptionsHandleOnDemand.GetNumber("__MI_DESTINATIONOPTIONS_CERT_CN_CHECK",
                                                   out checkInt,
                                                   out index,
                                                   out flags);
                CimException.ThrowIfMiResultFailure(result);
                bool checkBool = checkInt == 1 ? true : false;
                return checkBool;
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
                UInt32 checkInt = value == true ? (uint)1 : (uint)0;
                MI_Result result = this.DestinationOptionsHandleOnDemand.SetNumber("__MI_DESTINATIONOPTIONS_CERT_REVOCATION_CHECK",
                                                   checkInt,
                                                   MI_DestinationOptionsFlags.Unused);
                CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();
                UInt32 checkInt;
                UInt32 index;
                MI_DestinationOptionsFlags flags;
                MI_Result result = this.DestinationOptionsHandleOnDemand.GetNumber("__MI_DESTINATIONOPTIONS_CERT_REVOCATION_CHECK",
                                                   out checkInt,
                                                   out index,
                                                   out flags);
                CimException.ThrowIfMiResultFailure(result);
                bool checkBool = checkInt == 1 ? true : false;
                return checkBool;
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
                    MI_Result result = this.DestinationOptionsHandleOnDemand.SetString("__MI_DESTINATIONOPTIONS_TRANSPORT",
                                                       MI_Transport.HTTPS,
                                                       MI_DestinationOptionsFlags.Unused);
                    CimException.ThrowIfMiResultFailure(result);
                }
                else
                {
                    MI_Result result = this.DestinationOptionsHandleOnDemand.SetString("__MI_DESTINATIONOPTIONS_TRANSPORT",
                                                       MI_Transport.HTTP,
                                                       MI_DestinationOptionsFlags.Unused);
                    CimException.ThrowIfMiResultFailure(result);
                }
            }
            get
            {
                this.AssertNotDisposed();

                string transport;
                UInt32 index;
                MI_DestinationOptionsFlags flags;
                MI_Result result = this.DestinationOptionsHandleOnDemand.GetString("__MI_DESTINATIONOPTIONS_TRANSPORT",
                                                   out transport,
                                                   out index,
                                                   out flags);
                CimException.ThrowIfMiResultFailure(result);
#if(!_CORECLR)
                if (string.Compare(transport, MI_Transport.HTTPS, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase) == 0)
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
                string nativeProxyType = value.ToNativeType();
                MI_Result result = this.DestinationOptionsHandleOnDemand.SetString("__MI_DESTINATIONOPTIONS_PROXY_TYPE",
                                                   nativeProxyType,
                                                   MI_DestinationOptionsFlags.Unused);
                CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();
                string type;
                UInt32 index;
                MI_DestinationOptionsFlags flags;
                MI_Result result = this.DestinationOptionsHandleOnDemand.GetString("__MI_DESTINATIONOPTIONS_PROXY_TYPE",
                                                   out type,
                                                   out index,
                                                   out flags);
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

                string nativeEncoding = value.ToNativeType();
                MI_Result result = this.DestinationOptionsHandleOnDemand.SetString("__MI_DESTINATIONOPTIONS_PACKET_ENCODING",
                                                   nativeEncoding,
                                                   MI_DestinationOptionsFlags.Unused);
                CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();

                string nativeEncoding;
                UInt32 index;
                MI_DestinationOptionsFlags flags;
                MI_Result result = this.DestinationOptionsHandleOnDemand.GetString("__MI_DESTINATIONOPTIONS_PACKET_ENCODING",
                                                   out nativeEncoding,
                                                   out index,
                                                   out flags);
                return PacketEncodingExtensionMethods.FromNativeType(nativeEncoding);
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

                UInt32 packetPrivacyInt = packetPrivacy == true ? (uint)1 : (uint)0;
                MI_Result result = this.DestinationOptionsHandleOnDemand.SetNumber("__MI_DESTINATIONOPTIONS_PACKET_PRIVACY",
                                                   packetPrivacyInt,
                                                   MI_DestinationOptionsFlags.Unused);
                CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();

                UInt32 packetPrivacyInt;
                UInt32 index;
                MI_DestinationOptionsFlags flags;
                MI_Result result = this.DestinationOptionsHandleOnDemand.GetNumber("__MI_DESTINATIONOPTIONS_PACKET_PRIVACY",
                                                   out packetPrivacyInt,
                                                   out index,
                                                   out flags);
                CimException.ThrowIfMiResultFailure(result);
                bool packetPrivacyBool = packetPrivacyInt == 1 ? true : false;
                bool noEncryption = !packetPrivacyBool;
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

                UInt32 encodeInt = value == true ? (uint)1 : (uint)0;
                MI_Result result = this.DestinationOptionsHandleOnDemand.SetNumber("__MI_DESTINATIONOPTIONS_ENCODE_PORT_IN_SPN",
                                                   encodeInt,
                                                   MI_DestinationOptionsFlags.Unused);
                CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();

                UInt32 encodeInt;
                UInt32 index;
                MI_DestinationOptionsFlags flags;
                MI_Result result = this.DestinationOptionsHandleOnDemand.GetNumber("__MI_DESTINATIONOPTIONS_CERT_REVOCATION_CHECK",
                                                   out encodeInt,
                                                   out index,
                                                   out flags);
                CimException.ThrowIfMiResultFailure(result);
                bool encodeBool = encodeInt == 1 ? true : false;
                return encodeBool;
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

                string prefix = value.ToString();
                MI_Result result = this.DestinationOptionsHandleOnDemand.SetString("__MI_DESTINATIONOPTIONS_HTTP_URL_PREFIX",
                                                   prefix,
                                                   MI_DestinationOptionsFlags.Unused);
                CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();

                string httpUrlPrefix;
                UInt32 index;
                MI_DestinationOptionsFlags flags;
                MI_Result result = this.DestinationOptionsHandleOnDemand.GetString("__MI_DESTINATIONOPTIONS_HTTP_URL_PREFIX",
                                                   out httpUrlPrefix,
                                                   out index,
                                                   out flags);

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

            // TODO: Not trivial to port AddProxyCredentials
            //MI_Result result = this.DestinationOptionsHandleOnDemand.AddProxyCredentials(credential.GetCredential());
            //CimException.ThrowIfMiResultFailure(result);
        }
    }
}
