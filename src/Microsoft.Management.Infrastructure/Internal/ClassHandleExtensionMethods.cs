/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/

using System;
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

        public static MI_Class[] CloneMIArray(this MI_Class[] arrayToClone)
        {
            if (arrayToClone == null)
            {
                throw new ArgumentNullException();
            }

            MI_Class[] result = new MI_Class[arrayToClone.Length];
            try
            {
                for (int i = 0; i < arrayToClone.Length; i++)
                {
                    MI_Class origClass = arrayToClone[i];
                    result[i] = origClass == null ? null : origClass.Clone();
                }
            }
            catch
            {
                // If we encounter an exception halfway through we need to rollback
                for (int i = 0; i < arrayToClone.Length; i++)
                {
                    if (arrayToClone[i] == null)
                    {
                        break;
                    }

                    arrayToClone[i].Delete();
                }

                throw;
            }

            return result;
        }
    }
}
