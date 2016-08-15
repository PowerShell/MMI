/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


using Microsoft.Management.Infrastructure.Internal;
using Microsoft.Management.Infrastructure.Native;
using System;
using System.Diagnostics;

namespace Microsoft.Management.Infrastructure.Serialization
{
    /// <summary>
    ///  Class use to craete a mof serializer
    /// </summary>
    internal static class CimMofSerializer
    {
        #region Constructors

        private static CimSerializer CreateCimMofSerializer(string format, uint flags)
        {
            Debug.Assert(!string.IsNullOrEmpty(format), "Caller should verify that format != null");

            MI_Serializer tmpHandle;
            // TODO: Fix MI_SerializerFlags in next line to come from "flags"
            MI_Result result = CimApplication.Handle.NewSerializer(MI_SerializerFlags.None,
                                   format,
                                   out tmpHandle);
            if (result == MI_Result.MI_RESULT_INVALID_PARAMETER)
            {
                throw new ArgumentOutOfRangeException("format");
            }
            return new CimSerializer(tmpHandle);
        }

        /// <summary>
        /// Instantiates a default serializer
        /// </summary>
        public static CimSerializer Create()
        {
            return CreateCimMofSerializer(format: "MI_MOF_CIMV2_EXTV1", flags: 0);
        }

        #endregion Constructors
    }
}
