/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/
using Microsoft.Management.Infrastructure.Options;
using System.Security;

namespace Microsoft.Management.Infrastructure.Native
{
    public class NativeCimCredential
    {
        private SecureString passwordSecureStr;
        private bool credentialIsCertificate;

        internal NativeCimCredential(bool bIsCertificate, SecureString secureStr)
        {
            passwordSecureStr = null;
            credentialIsCertificate = bIsCertificate;
            if (secureStr != null && secureStr.Length > 0)
            {
                passwordSecureStr = secureStr.Copy();
            }
        }

        internal SecureString GetSecureString()
        {
            return passwordSecureStr;
        }

        internal void AssertValidInternalState()
        {
            return;
        }

        internal static void CreateCimCredential(string authenticationMechanism, string certificateThumbprint, out NativeCimCredential credential)
        {
            // TODO: Implement
            credential = new NativeCimCredential(true, new SecureString());
        }

        internal static void CreateCimCredential(string authenticationMechanism, string domain, string userName, SecureString password, out NativeCimCredential credential)
        {
            // TODO: Implement
            credential = new NativeCimCredential(true, new SecureString());
        }

        internal static void CreateCimCredential(string authenticationMechanism, out NativeCimCredential credential)
        {
            // TODO: Implement
            credential = new NativeCimCredential(true, new SecureString());
        }

        internal static void CreateCimCredential(CertificateAuthenticationMechanism authenticationMechanism, string certificateThumbprint, out NativeCimCredential credential)
        {
            // TODO: Implement
            credential = new NativeCimCredential(true, new SecureString());
        }

        internal static void CreateCimCredential(PasswordAuthenticationMechanism authenticationMechanism, string domain, string userName, SecureString password, out NativeCimCredential credential)
        {
            // TODO: Implement
            credential = new NativeCimCredential(true, new SecureString());
        }

        internal static void CreateCimCredential(ImpersonatedAuthenticationMechanism authenticationMechanism, out NativeCimCredential credential)
        {
            // TODO: Implement
            credential = new NativeCimCredential(true, new SecureString());
        }
    }
}
