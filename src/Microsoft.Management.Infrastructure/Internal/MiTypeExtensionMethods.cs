/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


using System;
using System.Diagnostics;
using Microsoft.Management.Infrastructure.Native;

namespace Microsoft.Management.Infrastructure.Internal
{
    internal static class MiTypeExtensionMethods
    {
        public static CimType ToCimType(this MI_Type miType)
        {
            switch (miType)
            {
                case MI_Type.MI_BOOLEAN: return CimType.Boolean;
                case MI_Type.MI_UINT8: return CimType.UInt8;
                case MI_Type.MI_SINT8: return CimType.SInt8;
                case MI_Type.MI_UINT16: return CimType.UInt16;
                case MI_Type.MI_SINT16: return CimType.SInt16;
                case MI_Type.MI_UINT32: return CimType.UInt32;
                case MI_Type.MI_SINT32: return CimType.SInt32;
                case MI_Type.MI_UINT64: return CimType.UInt64;
                case MI_Type.MI_SINT64: return CimType.SInt64;
                case MI_Type.MI_REAL32: return CimType.Real32;
                case MI_Type.MI_REAL64: return CimType.Real64;
                case MI_Type.MI_CHAR16: return CimType.Char16;
                case MI_Type.MI_DATETIME: return CimType.DateTime;
                case MI_Type.MI_STRING: return CimType.String;
                case MI_Type.MI_REFERENCE: return CimType.Reference;
                case MI_Type.MI_INSTANCE: return CimType.Instance;

                case MI_Type.MI_BOOLEANA: return CimType.BooleanArray;
                case MI_Type.MI_UINT8A: return CimType.UInt8Array;
                case MI_Type.MI_SINT8A: return CimType.SInt8Array;
                case MI_Type.MI_UINT16A: return CimType.UInt16Array;
                case MI_Type.MI_SINT16A: return CimType.SInt16Array;
                case MI_Type.MI_UINT32A: return CimType.UInt32Array;
                case MI_Type.MI_SINT32A: return CimType.SInt32Array;
                case MI_Type.MI_UINT64A: return CimType.UInt64Array;
                case MI_Type.MI_SINT64A: return CimType.SInt64Array;
                case MI_Type.MI_REAL32A: return CimType.Real32Array;
                case MI_Type.MI_REAL64A: return CimType.Real64Array;
                case MI_Type.MI_CHAR16A: return CimType.Char16Array;
                case MI_Type.MI_DATETIMEA: return CimType.DateTimeArray;
                case MI_Type.MI_STRINGA: return CimType.StringArray;
                case MI_Type.MI_REFERENCEA: return CimType.ReferenceArray;
                case MI_Type.MI_INSTANCEA: return CimType.InstanceArray;

                default:
                    Debug.Assert(false, "Unrecognized or unsupported value of MI_Type");
                    throw new ArgumentOutOfRangeException("miType");
            }
        }

        public static MI_Type FromCimType(this CimType cimType)
        {
            switch (cimType)
            {
                case CimType.Boolean: return MI_Type.MI_BOOLEAN;
                case CimType.UInt8: return MI_Type.MI_UINT8;
                case CimType.SInt8: return MI_Type.MI_SINT8;
                case CimType.UInt16: return MI_Type.MI_UINT16;
                case CimType.SInt16: return MI_Type.MI_SINT16;
                case CimType.UInt32: return MI_Type.MI_UINT32;
                case CimType.SInt32: return MI_Type.MI_SINT32;
                case CimType.UInt64: return MI_Type.MI_UINT64;
                case CimType.SInt64: return MI_Type.MI_SINT64;
                case CimType.Real32: return MI_Type.MI_REAL32;
                case CimType.Real64: return MI_Type.MI_REAL64;
                case CimType.Char16: return MI_Type.MI_CHAR16;
                case CimType.DateTime: return MI_Type.MI_DATETIME;
                case CimType.String: return MI_Type.MI_STRING;
                case CimType.Reference: return MI_Type.MI_REFERENCE;
                case CimType.Instance: return MI_Type.MI_INSTANCE;

                case CimType.BooleanArray: return MI_Type.MI_BOOLEANA;
                case CimType.UInt8Array: return MI_Type.MI_UINT8A;
                case CimType.SInt8Array: return MI_Type.MI_SINT8A;
                case CimType.UInt16Array: return MI_Type.MI_UINT16A;
                case CimType.SInt16Array: return MI_Type.MI_SINT16A;
                case CimType.UInt32Array: return MI_Type.MI_UINT32A;
                case CimType.SInt32Array: return MI_Type.MI_SINT32A;
                case CimType.UInt64Array: return MI_Type.MI_UINT64A;
                case CimType.SInt64Array: return MI_Type.MI_SINT64A;
                case CimType.Real32Array: return MI_Type.MI_REAL32A;
                case CimType.Real64Array: return MI_Type.MI_REAL64A;
                case CimType.Char16Array: return MI_Type.MI_CHAR16A;
                case CimType.DateTimeArray: return MI_Type.MI_DATETIMEA;
                case CimType.StringArray: return MI_Type.MI_STRINGA;
                case CimType.ReferenceArray: return MI_Type.MI_REFERENCEA;
                case CimType.InstanceArray: return MI_Type.MI_INSTANCEA;

                case CimType.Unknown:
                default:
                    Debug.Assert(false, "Unrecognized or unsupported value of CimType");
                    throw new ArgumentOutOfRangeException("cimType");
            }
        }
    }
}
