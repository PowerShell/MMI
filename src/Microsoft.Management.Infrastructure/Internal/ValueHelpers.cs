/*============================================================================
 * Copyright (C) Microsoft Corporation, All rights reserved.
 *============================================================================
 */

using Microsoft.Management.Infrastructure.Native;

namespace Microsoft.Management.Infrastructure.Internal
{
    internal static class ValueHelpers
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