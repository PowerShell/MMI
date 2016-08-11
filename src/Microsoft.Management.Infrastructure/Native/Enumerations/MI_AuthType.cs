/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/
namespace Microsoft.Management.Infrastructure.Native
{
    public class MI_AuthType
    {
        public static string DEFAULT = "Default";
        public static string NONE = "None";
        public static string DIGEST = "Digest";
        public static string NEGO_WITH_CREDS = "NegoWithCreds";
        public static string NEGO_NO_CREDS = "NegoNoCreds";
        public static string BASIC = "Basic";
        public static string KERBEROS = "Kerberos";
        public static string CLIENT_CERTS = "ClientCerts";
        public static string NTLM = "Ntlmdomain";
        public static string CREDSSP = "CredSSP";
        public static string ISSUER_CERT = "IssuerCert";
    }
}
