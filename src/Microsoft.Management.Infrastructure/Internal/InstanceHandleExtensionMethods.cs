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
    internal static class InstanceHandleExtensionMethods
    {
        public static MI_Instance Clone(this MI_Instance handleToClone)
        {
            if (handleToClone == null || handleToClone.IsNull)
            {
                throw new ArgumentNullException();
            }

            MI_Instance clonedHandle;
            MI_Result result = handleToClone.Clone(out clonedHandle);
            CimException.ThrowIfMiResultFailure(result);
            return clonedHandle;
        }

        public static MI_Instance[] CloneMIArray(this MI_Instance[] arrayToClone)
        {
            if (arrayToClone == null)
            {
                throw new ArgumentNullException();
            }

            MI_Instance[] result = new MI_Instance[arrayToClone.Length];
            try
            {
                for (int i = 0; i < arrayToClone.Length; i++)
                {
                    MI_Instance origInstance = arrayToClone[i];
                    result[i] = origInstance == null ? null : origInstance.Clone();
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
