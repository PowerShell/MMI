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
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Management.Infrastructure.Options
{
    /// <summary>
    /// Flags of CIM operations.
    /// </summary>
    /// <seealso cref="CimOperationOptions"/>
    [Flags]
    [SuppressMessage("Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags", Justification = "This is a direct copy of the native flags enum (which indeed doesn't cover 0x4, 0x1, 0x100")]
    public enum CimOperationFlags : long
    {
        None = 0,

        // Nothing for Native.MiOperationFlags.ManualAckResults - this is covered by the infrastructure

        NoTypeInformation = MI_OperationFlags.MI_OPERATIONFLAGS_NO_RTTI,
        BasicTypeInformation = MI_OperationFlags.MI_OPERATIONFLAGS_BASIC_RTTI,
        StandardTypeInformation = MI_OperationFlags.MI_OPERATIONFLAGS_STANDARD_RTTI,
        FullTypeInformation = MI_OperationFlags.MI_OPERATIONFLAGS_FULL_RTTI,

        LocalizedQualifiers = MI_OperationFlags.MI_OPERATIONFLAGS_LOCALIZED_QUALIFIERS,

        ExpensiveProperties = MI_OperationFlags.MI_OPERATIONFLAGS_EXPENSIVE_PROPERTIES,

        PolymorphismShallow = MI_OperationFlags.MI_OPERATIONFLAGS_POLYMORPHISM_SHALLOW,
        PolymorphismDeepBasePropsOnly = MI_OperationFlags.MI_OPERATIONFLAGS_POLYMORPHISM_DEEP_BASE_PROPS_ONLY,

        ReportOperationStarted = MI_OperationFlags.MI_OPERATIONFLAGS_REPORT_OPERATION_STARTED,
    };

    /// <summary>
    /// Password Authentication mechanisms.
    /// </summary>
    /// <seealso cref="CimOperationOptions"/>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "This is a direct representation of the native flags (which doesn't have None)")]
    public enum PasswordAuthenticationMechanism : int
    {
        Default = 0,
        Digest = 1,
        Negotiate = 2,
        Basic = 3,
        Kerberos = 4,
        NtlmDomain = 5,

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "CredSsp is direct representation of the native flag")]
        CredSsp = 6,
    };

    /// <summary>
    /// Certificate Authentication mechanisms.
    /// </summary>
    /// <seealso cref="CimOperationOptions"/>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "This is a direct representation of the native flags (which doesn't have None)")]
    public enum CertificateAuthenticationMechanism : int
    {
        Default = 0,
        ClientCertificate = 1,
        IssuerCertificate = 2,
    };

    /// <summary>
    /// Impersonated Authentication mechanisms of CIM session.
    /// </summary>
    /// <seealso cref="CimOperationOptions"/>
    public enum ImpersonatedAuthenticationMechanism : int
    {
        None = 0,
        Negotiate = 1,
        Kerberos = 2,
        NtlmDomain = 3,
    };
}

namespace Microsoft.Management.Infrastructure.Options.Internal
{
    internal static class OperationFlagsExtensionMethods
    {
        public static MI_OperationFlags ToNative(this CimOperationFlags operationFlags)
        {
            return (MI_OperationFlags)operationFlags;
        }
    }
}
