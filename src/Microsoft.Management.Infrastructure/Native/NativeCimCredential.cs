using Microsoft.Management.Infrastructure.Options;
using System.Security;
using System;

namespace Microsoft.Management.Infrastructure.Native
{
    public class NativeCimCredential
    {
        private SecureString passwordSecureStr;
        private bool credentialIsCertificate;
        internal MI_UserCredentials cred;

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
            credential = new NativeCimCredential(true, null);
            
            credential.cred.domainString = null;
            credential.cred.usernameString = null;
            credential.cred.passwordString = null;

            credential.cred.authenticationTypeString = authenticationMechanism;
            credential.cred.certificateThumbprintString = certificateThumbprint;
        }

        internal static void CreateCimCredential(string authenticationMechanism, string domain, string userName, SecureString password, out NativeCimCredential credential)
        {
            credential = new NativeCimCredential(false, password);
            credential.cred.certificateThumbprintString = null;
            credential.cred.passwordString = null;

            credential.cred.authenticationTypeString = authenticationMechanism;
            credential.cred.domainString = domain;
            credential.cred.usernameString = userName;
        }

        internal static void CreateCimCredential(string authenticationMechanism, out NativeCimCredential credential)
        {
            credential = new NativeCimCredential(false, null);
            credential.cred.certificateThumbprintString = null;
            credential.cred.domainString = null;
            credential.cred.usernameString = null;
            credential.cred.passwordString = null;

            credential.cred.authenticationTypeString = authenticationMechanism;
        }
    }
}