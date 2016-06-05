/*============================================================================
 * Copyright (C) Microsoft Corporation, All rights reserved.
 *============================================================================
 */

using Microsoft.Management.Infrastructure.Native;

namespace Microsoft.Management.Infrastructure.Internal
{
    internal static class ClassHandleExtensionMethods
    {
        public static MI_Class Clone(this MI_Class handleToClone)
        {
            if (handleToClone == null)
            {
                return null;
            }
            // TODO: handleToClone.AssertValidInternalState();

            MI_Class clonedHandle;
            MI_Result result = handleToClone.Clone(out clonedHandle);
            CimException.ThrowIfMiResultFailure(result);
            return clonedHandle;
        }
    }
}