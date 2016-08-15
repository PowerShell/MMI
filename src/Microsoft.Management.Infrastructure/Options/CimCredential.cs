/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/

using Microsoft.Management.Infrastructure.Native;
using System;
using System.Security;

namespace Microsoft.Management.Infrastructure.Options
{
    /// <summary>
    /// Represents CimCredential.
    /// </summary>
    public class CimCredential
    {
        private NativeCimCredential credential;

        /// <summary>
        /// Creates a new Credentials
        /// </summary>
        public CimCredential(string authenticationMechanism, string certificateThumbprint)
        {
            if (authenticationMechanism == null)
            {
                throw new ArgumentNullException("authenticationMechanism");
            }
            NativeCimCredential.CreateCimCredential(authenticationMechanism, certificateThumbprint, out credential);
        }

        /// <summary>
        /// Creates a new Credentials
        /// </summary>
        public CimCredential(string authenticationMechanism, string domain, string userName, SecureString password)
        {
            if (authenticationMechanism == null)
            {
                throw new ArgumentNullException("authenticationMechanism");
            }
            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }
            NativeCimCredential.CreateCimCredential(authenticationMechanism, domain, userName, password, out credential);
        }

        /// <summary>
        /// Creates a new Credentials
        /// </summary>
        public CimCredential(string authenticationMechanism)
        {
            if (authenticationMechanism == null)
            {
                throw new ArgumentNullException("authenticationMechanism");
            }
            NativeCimCredential.CreateCimCredential(authenticationMechanism, out credential);
        }

        /// <summary>
        /// Creates a new Credentials
        /// </summary>
        public CimCredential(CertificateAuthenticationMechanism authenticationMechanism, string certificateThumbprint)
        {
            string strAuthenticationMechanism = null;
            if (authenticationMechanism == CertificateAuthenticationMechanism.Default)
            {
                strAuthenticationMechanism = MI_AuthType.CLIENT_CERTS;
            }
            else if (authenticationMechanism == CertificateAuthenticationMechanism.ClientCertificate)
            {
                strAuthenticationMechanism = MI_AuthType.CLIENT_CERTS;
            }
            else if (authenticationMechanism == CertificateAuthenticationMechanism.IssuerCertificate)
            {
                strAuthenticationMechanism = MI_AuthType.ISSUER_CERT;
            }
            else
            {
                throw new ArgumentOutOfRangeException("authenticationMechanism");
            }
            NativeCimCredential.CreateCimCredential(strAuthenticationMechanism, certificateThumbprint, out credential);
        }

        /// <summary>
        /// Creates a new Credentials
        /// </summary>
        public CimCredential(PasswordAuthenticationMechanism authenticationMechanism, string domain, string userName, SecureString password)
        {
            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }
            string strAuthenticationMechanism = null;
            if (authenticationMechanism == PasswordAuthenticationMechanism.Default)
            {
                strAuthenticationMechanism = MI_AuthType.DEFAULT;
            }
            else if (authenticationMechanism == PasswordAuthenticationMechanism.Basic)
            {
                strAuthenticationMechanism = MI_AuthType.BASIC;
            }
            else if (authenticationMechanism == PasswordAuthenticationMechanism.Digest)
            {
                strAuthenticationMechanism = MI_AuthType.DIGEST;
            }
            else if (authenticationMechanism == PasswordAuthenticationMechanism.Negotiate)
            {
                strAuthenticationMechanism = MI_AuthType.NEGO_WITH_CREDS;
            }
            else if (authenticationMechanism == PasswordAuthenticationMechanism.Kerberos)
            {
                strAuthenticationMechanism = MI_AuthType.KERBEROS;
            }
            else if (authenticationMechanism == PasswordAuthenticationMechanism.NtlmDomain)
            {
                strAuthenticationMechanism = MI_AuthType.NTLM;
            }
            else if (authenticationMechanism == PasswordAuthenticationMechanism.CredSsp)
            {
                strAuthenticationMechanism = MI_AuthType.CREDSSP;
            }
            else
            {
                throw new ArgumentOutOfRangeException("authenticationMechanism");
            }
            NativeCimCredential.CreateCimCredential(strAuthenticationMechanism, domain, userName, password, out credential);
        }

        /// <summary>
        /// Creates a new Credentials
        /// </summary>
        public CimCredential(ImpersonatedAuthenticationMechanism authenticationMechanism)
        {
            string strAuthenticationMechanism = null;
            if (authenticationMechanism == ImpersonatedAuthenticationMechanism.None)
            {
                strAuthenticationMechanism = MI_AuthType.NONE;
            }
            else if (authenticationMechanism == ImpersonatedAuthenticationMechanism.Negotiate)
            {
                strAuthenticationMechanism = MI_AuthType.NEGO_NO_CREDS;
            }
            else if (authenticationMechanism == ImpersonatedAuthenticationMechanism.Kerberos)
            {
                strAuthenticationMechanism = MI_AuthType.KERBEROS;
            }
            else if (authenticationMechanism == ImpersonatedAuthenticationMechanism.NtlmDomain)
            {
                strAuthenticationMechanism = MI_AuthType.NTLM;
            }
            else
            {
                throw new ArgumentOutOfRangeException("authenticationMechanism");
            }
            NativeCimCredential.CreateCimCredential(strAuthenticationMechanism, out credential);
        }

        // TODO: return proper credential type here
        //internal NativeCimCredentialHandle GetCredential(){ return credential; }
    }
}
