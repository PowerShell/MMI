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
using System.Globalization;

namespace Microsoft.Management.Infrastructure.Options
{
    public enum ProxyType
    {
        None,
        WinHttp,
        Auto,
        InternetExplorer,
    };
}

namespace Microsoft.Management.Infrastructure.Options.Internal
{
    internal static class ProxyTypeExtensionMethods
    {
        public static string ToNativeType(this ProxyType proxyType)
        {
            switch (proxyType)
            {
                case ProxyType.None:
                    return MI_ProxyType.None;

                case ProxyType.WinHttp:
                    return MI_ProxyType.WinHTTP;

                case ProxyType.Auto:
                    return MI_ProxyType.Auto;

                case ProxyType.InternetExplorer:
                    return MI_ProxyType.IE;

                default:
                    throw new ArgumentOutOfRangeException("proxyType");
            }
        }

        public static ProxyType FromNativeType(string proxyType)
        {
            if (String.Compare(proxyType, MI_ProxyType.None, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase) == 0)
            {
                return ProxyType.None;
            }
            else if (String.Compare(proxyType, MI_ProxyType.WinHTTP, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase) == 0)
            {
                return ProxyType.WinHttp;
            }
            else if (String.Compare(proxyType, MI_ProxyType.Auto, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase) == 0)
            {
                return ProxyType.Auto;
            }
            else if (String.Compare(proxyType, MI_ProxyType.IE, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase) == 0)
            {
                return ProxyType.InternetExplorer;
            }
            else
            {
                throw new ArgumentOutOfRangeException("proxyType");
            }
        }
    }
}
