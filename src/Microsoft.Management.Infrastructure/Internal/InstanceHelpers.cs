/*============================================================================
 * Copyright (C) Microsoft Corporation, All rights reserved.
 *============================================================================
 */

using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Management.Infrastructure.Generic;
using Microsoft.Management.Infrastructure.Internal;
using Microsoft.Management.Infrastructure.Internal.Data;
using Microsoft.Management.Infrastructure.Native;
using Microsoft.Management.Infrastructure.Serialization;
using System.IO;

namespace Microsoft.Management.Infrastructure.Internal
{
    internal static class NativeErrorCodeExtensionMethods
    {
        public static NativeErrorCode ToNativeErrorCode(this MI_Result miResult)
        {
            return (NativeErrorCode)miResult;
        }
    }
}