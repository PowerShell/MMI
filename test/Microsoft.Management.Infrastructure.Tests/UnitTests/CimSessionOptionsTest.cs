/*============================================================================
* Copyright (C) Microsoft Corporation, All rights reserved.
*=============================================================================
*/

using System;
using System.Globalization;
using System.Security;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Options;
using Xunit;

namespace MMI.Tests.UnitTests
{

    public class CimSessionOptionsTest
    {
        [Fact]
        public void BaseOptions_Empty()
        {
            var sessionOptions = new CimSessionOptions();
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [TDDFact]
        public void BaseOptions_Timeout()
        {
            var sessionOptions = new CimSessionOptions();
            sessionOptions.Timeout = TimeSpan.FromSeconds(123);
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [TDDFact]
        public void BaseOptions_AddDestinationCredential1()
        {
            var sessionOptions = new CimSessionOptions();
            CimCredential cred = new CimCredential(ImpersonatedAuthenticationMechanism.None);
            sessionOptions.AddDestinationCredentials(cred);
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [TDDFact]
        public void BaseOptions_AddDestinationCredential2()
        {
            var sessionOptions = new CimSessionOptions();
            SecureString str = new SecureString();
            str.AppendChar('d');
            str.AppendChar('d');
            str.AppendChar('d');
            str.AppendChar('d');
            str.AppendChar('d');
            str.AppendChar('d');
            CimCredential cred = new CimCredential(PasswordAuthenticationMechanism.Default, "ddddddd", "ddddddd", str);
            sessionOptions.AddDestinationCredentials(cred);
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [TDDFact]
        public void BaseOptions_AddDestinationCredential3()
        {
            string thumbprint = null;
            X509Store store = new X509Store("My", StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            foreach (var cert in store.Certificates)
            {
                thumbprint = cert.Thumbprint;
                break;
            }

            var sessionOptions = new WSManSessionOptions();
            CimCredential cred = new CimCredential(CertificateAuthenticationMechanism.Default, thumbprint);
            sessionOptions.AddDestinationCredentials(cred);
            sessionOptions.UseSsl = true;
            using (CimSession cimSession = CimSession.Create(Environment.MachineName, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [TDDFact]
        public void BaseOptions_AddDestinationCredential4_NullUserName()
        {
            var sessionOptions = new CimSessionOptions();
            SecureString str = new SecureString();
            str.AppendChar('d');
            str.AppendChar('d');
            str.AppendChar('d');
            str.AppendChar('d');
            str.AppendChar('d');
            str.AppendChar('d');
            CimCredential cred = new CimCredential(PasswordAuthenticationMechanism.Default, "ddddddd", null, str);
            sessionOptions.AddDestinationCredentials(cred);
            Assert.Throws<ArgumentNullException>(() => { return CimSession.Create(null, sessionOptions); });
        }

        [TDDFact]
        public void BaseOptions_AddDestinationCredential5_WrongMechamism()
        {
            var sessionOptions = new CimSessionOptions();
            SecureString str = new SecureString();
            str.AppendChar('d');
            str.AppendChar('d');
            str.AppendChar('d');
            str.AppendChar('d');
            str.AppendChar('d');
            str.AppendChar('d');
            CimCredential cred = new CimCredential((PasswordAuthenticationMechanism)100, "ddddddd", "ddddddd", str);
            sessionOptions.AddDestinationCredentials(cred);
            Assert.Throws<ArgumentOutOfRangeException>(() => { return CimSession.Create(null, sessionOptions); });
        }

        [TDDFact]
        public void BaseOptions_Timeout_Infinity()
        {
            var sessionOptions = new CimSessionOptions();
            sessionOptions.Timeout = TimeSpan.MaxValue;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }
        /*
                [Fact]
                public void BaseOptions_Culture()
                {
                    var sessionOptions = new CimSessionOptions();
                    sessionOptions.Culture = CultureInfo.GetCultures(CultureTypes.AllCultures).First();
                    using (CimSession cimSession = CimSession.Create(null, sessionOptions))
                    {
                        Assert.NotNull(cimSession, "cimSession should not be null");
                    }
                }
        */
        [Fact]
        public void BaseOptions_Culture_Null()
        {
            var sessionOptions = new CimSessionOptions();
            Assert.Throws<ArgumentNullException>(() => { return sessionOptions.Culture = null; });
        }
        /*
                [Fact]
                public void BaseOptions_UICulture()
                {
                    var sessionOptions = new CimSessionOptions();
                    sessionOptions.UICulture = CultureInfo.GetCultures(CultureTypes.AllCultures).First();
                    using (CimSession cimSession = CimSession.Create(null, sessionOptions))
                    {
                        Assert.NotNull(cimSession, "cimSession should not be null");
                    }
                }
        */
        [Fact]
        public void BaseOptions_UICulture_Null()
        {
            var sessionOptions = new CimSessionOptions();
            Assert.Throws<ArgumentNullException>(() => { return sessionOptions.UICulture = null; });
        }

        [Fact]
        public void DComOptions_Empty()
        {
            using (var sessionOptions = new DComSessionOptions())
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void DComOptions_PacketIntegrity_True()
        {
            var sessionOptions = new DComSessionOptions();
            sessionOptions.PacketIntegrity = true;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void DComOptions_PacketIntegrity_False()
        {
            var sessionOptions = new DComSessionOptions();
            sessionOptions.PacketIntegrity = false;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void DComOptions_PacketPrivacy_True()
        {
            var sessionOptions = new DComSessionOptions();
            sessionOptions.PacketPrivacy = true;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void DComOptions_PacketPrivacy_False()
        {
            var sessionOptions = new DComSessionOptions();
            sessionOptions.PacketPrivacy = false;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void DComOptions_Impersonation_None()
        {
            var sessionOptions = new DComSessionOptions();
            sessionOptions.Impersonation = ImpersonationType.None;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void DComOptions_Impersonation_Default()
        {
            var sessionOptions = new DComSessionOptions();
            sessionOptions.Impersonation = ImpersonationType.Default;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void DComOptions_Impersonation_Delegate()
        {
            var sessionOptions = new DComSessionOptions();
            sessionOptions.Impersonation = ImpersonationType.Delegate;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void DComOptions_Impersonation_Identify()
        {
            var sessionOptions = new DComSessionOptions();
            sessionOptions.Impersonation = ImpersonationType.Identify;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void DComOptions_Impersonation_Impersonate()
        {
            var sessionOptions = new DComSessionOptions();
            sessionOptions.Impersonation = ImpersonationType.Impersonate;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void WSManOptions_Empty()
        {
            using (var sessionOptions = new WSManSessionOptions())
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void WSManOptions_SetDestinationPort()
        {
            // TODO/FIXME - add unit test for corner cases (0, > 65535)

            var sessionOptions = new WSManSessionOptions();
            sessionOptions.DestinationPort = 8080;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void WSManOptions_SetProxyCredential()
        {
            var sessionOptions = new WSManSessionOptions();
            sessionOptions.DestinationPort = 8080;
            CimCredential cred = new CimCredential(ImpersonatedAuthenticationMechanism.None); //wsman accepts only username/password
            sessionOptions.AddProxyCredentials(cred);
            //Exception is thrown after creating the session as WSMAN doesn't allow proxy without username/password.
            Assert.Throws<CimException>(() => { return CimSession.Create(null, sessionOptions); });
        }

        [Fact]
        public void WSManOptions_SetMaxEnvelopeSize()
        {
            // TODO/FIXME - add unit test for corner cases (0, maxvalue)
            var sessionOptions = new WSManSessionOptions();
            sessionOptions.MaxEnvelopeSize = 8080;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void WSManOptions_SetCertCACheck_True()
        {
            var sessionOptions = new WSManSessionOptions();
            sessionOptions.CertCACheck = true;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void WSManOptions_SetCertCACheck_False()
        {
            var sessionOptions = new WSManSessionOptions();
            sessionOptions.CertCACheck = false;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void WSManOptions_SetCertCNCheck_True()
        {
            var sessionOptions = new WSManSessionOptions();
            sessionOptions.CertCNCheck = true;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void WSManOptions_SetCertCNCheck_False()
        {
            var sessionOptions = new WSManSessionOptions();
            sessionOptions.CertCNCheck = false;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void WSManOptions_SetCertRevocationCheck_True()
        {
            var sessionOptions = new WSManSessionOptions();
            sessionOptions.CertRevocationCheck = true;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void WSManOptions_SetCertRevocationCheck_False()
        {
            var sessionOptions = new WSManSessionOptions();
            sessionOptions.CertRevocationCheck = false;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void WSManOptions_SetUseSsl_True()
        {
            var sessionOptions = new WSManSessionOptions();
            sessionOptions.UseSsl = true;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void WSManOptions_SetUseSsl_False()
        {
            var sessionOptions = new WSManSessionOptions();
            sessionOptions.UseSsl = false;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void WSManOptions_ProxyType_None()
        {
            var sessionOptions = new WSManSessionOptions();
            sessionOptions.ProxyType = ProxyType.None;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void WSManOptions_ProxyType_Invalid()
        {
            var sessionOptions = new WSManSessionOptions();
            Assert.Throws<ArgumentOutOfRangeException>(() => { return sessionOptions.ProxyType = (ProxyType)123; });
        }

        [Fact]
        public void WSManOptions_PacketEncoding_Default()
        {
            var sessionOptions = new WSManSessionOptions();
            sessionOptions.PacketEncoding = PacketEncoding.Default;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void WSManOptions_PacketEncoding_UTF8()
        {
            var sessionOptions = new WSManSessionOptions();
            sessionOptions.PacketEncoding = PacketEncoding.Utf8;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void WSManOptions_PacketEncoding_UTF16()
        {
            var sessionOptions = new WSManSessionOptions();
            sessionOptions.PacketEncoding = PacketEncoding.Utf16;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void WSManOptions_PacketEncoding_Invalid()
        {
            var sessionOptions = new WSManSessionOptions();
            Assert.Throws<ArgumentOutOfRangeException>(() => { return sessionOptions.PacketEncoding = (PacketEncoding)123; });
        }

        [Fact]
        public void WSManOptions_SetEncodePortInSPN_True()
        {
            var sessionOptions = new WSManSessionOptions();
            sessionOptions.EncodePortInServicePrincipalName = true;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void WSManOptions_SetEncodePortInSPN_False()
        {
            var sessionOptions = new WSManSessionOptions();
            sessionOptions.EncodePortInServicePrincipalName = false;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        private void WSManOptions_SetHttpUrlPrefix_Core(Uri orig)
        {
            var sessionOptions = new WSManSessionOptions();
            sessionOptions.HttpUrlPrefix = orig;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
            Uri roundTrip = sessionOptions.HttpUrlPrefix;
            Assert.Equal(roundTrip.ToString(), orig.ToString(), "HttpUrlPrefix should stay the same on round-trip");
        }

        [Fact]
        public void WSManOptions_SetHttpUrlPrefix_Relative()
        {
            WSManOptions_SetHttpUrlPrefix_Core(new Uri("myPrefix", UriKind.Relative));
        }

        [Fact]
        public void WSManOptions_SetHttpUrlPrefix_Absolute()
        {
            WSManOptions_SetHttpUrlPrefix_Core(new Uri("http://www.microsoft.com", UriKind.Absolute));
        }

        [Fact]
        public void WSManOptions_SetHttpUrlPrefix_Null()
        {
            var sessionOptions = new WSManSessionOptions();
            Assert.Throws<ArgumentNullException>(() => { return sessionOptions.HttpUrlPrefix = null; });
        }

        private class TestCustomOptions : CimSessionOptions
        {
            internal TestCustomOptions(string protocolName)
                : base(protocolName)
            {
            }
        }

        [Fact]
        public void WSManOptions_NoEncryption_True()
        {
            var sessionOptions = new WSManSessionOptions();
            sessionOptions.NoEncryption = true;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void WSManOptions_NoEncryption_False()
        {
            var sessionOptions = new WSManSessionOptions();
            sessionOptions.NoEncryption = false;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [TDDFact]
        public void CustomOptions_UnrecognizedProtocol()
        {
            var sessionOptions = new TestCustomOptions("unrecognizedProtocolName");
            Assert.Throws<CimException>(() => { return CimSession.Create(null, sessionOptions); });
        }
    }
}
