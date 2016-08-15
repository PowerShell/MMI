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

namespace Microsoft.Management.Infrastructure
{
    /// <summary>
    /// Cim flags
    /// </summary>
    [Flags]
    public enum CimFlags : long
    {
        None = (long)0,

        Class = (long)MI_Flags.MI_FLAG_CLASS,
        Method = (long)MI_Flags.MI_FLAG_METHOD,
        Property = (long)MI_Flags.MI_FLAG_PROPERTY,
        Parameter = (long)MI_Flags.MI_FLAG_PARAMETER,
        Association = (long)MI_Flags.MI_FLAG_ASSOCIATION,
        Indication = (long)MI_Flags.MI_FLAG_INDICATION,
        Reference = (long)MI_Flags.MI_FLAG_REFERENCE,
        Any = (long)MI_Flags.MI_FLAG_ANY,

        /* Qualifier flavors */
        EnableOverride = (long)MI_Flags.MI_FLAG_ENABLEOVERRIDE,
        DisableOverride = (long)MI_Flags.MI_FLAG_DISABLEOVERRIDE,
        Restricted = (long)MI_Flags.MI_FLAG_RESTRICTED,
        ToSubclass = (long)MI_Flags.MI_FLAG_TOSUBCLASS,
        Translatable = (long)MI_Flags.MI_FLAG_TRANSLATABLE,

        /* Select boolean qualifier */
        Key = (long)MI_Flags.MI_FLAG_KEY,
        In = (long)MI_Flags.MI_FLAG_IN,
        Out = (long)MI_Flags.MI_FLAG_OUT,
        Required = (long)MI_Flags.MI_FLAG_REQUIRED,
        Static = (long)MI_Flags.MI_FLAG_STATIC,
        Abstract = (long)MI_Flags.MI_FLAG_ABSTRACT,
        Terminal = (long)MI_Flags.MI_FLAG_TERMINAL,
        Expensive = (long)MI_Flags.MI_FLAG_EXPENSIVE,
        Stream = (long)MI_Flags.MI_FLAG_STREAM,
        ReadOnly = (long)MI_Flags.MI_FLAG_READONLY,

        /* Special flags */
        NotModified = (long)MI_Flags.MI_FLAG_NOT_MODIFIED,
        NullValue = (long)MI_Flags.MI_FLAG_NULL,
        Borrow = (long)MI_Flags.MI_FLAG_BORROW,
        Adopt = (long)MI_Flags.MI_FLAG_ADOPT,
    };

    /// <summary>
    /// CimSubscriptionDeliveryType
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "This is a direct copy of the native flags enum")]
    public enum CimSubscriptionDeliveryType : int
    {
        None = (int)MI_SubscriptionDeliveryType.MI_SubscriptionDeliveryType_Push,
        Push = (int)MI_SubscriptionDeliveryType.MI_SubscriptionDeliveryType_Push,
        Pull = (int)MI_SubscriptionDeliveryType.MI_SubscriptionDeliveryType_Pull,
    }
}
