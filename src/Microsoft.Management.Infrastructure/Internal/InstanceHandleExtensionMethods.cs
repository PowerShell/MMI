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
    internal static class InstanceHandleExtensionMethods
    {
        public static MI_Instance Clone(this MI_Instance handleToClone)
        {
            if (handleToClone == null)
            {
                return null;
            }
            handleToClone.AssertValidInternalState();

            MI_Instance clonedHandle;
            MI_Result result = handleToClone.Clone(out clonedHandle);
            CimException.ThrowIfMiResultFailure(result);
            return clonedHandle;
        }
    }
}