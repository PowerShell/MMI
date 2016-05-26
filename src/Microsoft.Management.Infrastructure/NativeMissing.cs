using Microsoft.Management.Infrastructure.Options;
using System.Security;

namespace NativeObject
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

    public enum MI_CallbackMode : uint
    {
        Report = 0,
        Inquire = 1,
        Ignore = 2,
    }

    public enum MI_ImpersonationType : uint
    {
        Default = 0,
        None = 1,
        Identify = 2,
        Impersonate = 3,
        Delegate = 4,
    }

    public class MI_ProxyType
    {
        public static string None = "None";
        public static string WinHTTP = "WinHTTP";
        public static string Auto = "Auto";
        public static string IE = "IE";
    }

    public class MI_PacketEncoding
    {
        public static string Default = "default";
        public static string UTF8 = "UTF8";
        public static string UTF16 = "UTF16";
    }

    public class MI_Protocol
    {
        public static string WSMan = "WinRM";
    }

    public class MI_Transport
    {
        public static string HTTPS = "HTTPS";
        public static string HTTP = "HTTP";
    }

    internal class OperationCallbackProcessingContext
    {
        private bool inUserCode;
        private object managedOperationContext;

        internal OperationCallbackProcessingContext(object managedOperationContext)
        {
            this.inUserCode = false;
            this.managedOperationContext = managedOperationContext;
        }

        internal bool InUserCode
        {
            get
            {
                return this.inUserCode;
            }
            set
            {
                this.inUserCode = value;
            }
        }

        internal object ManagedOperationContext
        {
            get
            {
                return this.managedOperationContext;
            }
        }
    }

    internal class InstanceMethods
    {
        internal static void ThrowIfMismatchedType(MI_Type type, object managedValue)
        {
            // TODO: Implement this
            /*
              MI_Value throwAway;
              memset(&throwAway, 0, sizeof(MI_Value));
              IEnumerable<DangerousHandleAccessor^>^ dangerousHandleAccesorsFromConversion = nullptr;
              try
              {
              dangerousHandleAccesorsFromConversion = ConvertToMiValue(type, managedValue, &throwAway);
              }
              finally
              {
              ReleaseMiValue(type, &throwAway, dangerousHandleAccesorsFromConversion);
              }
            */
        }
    }
}