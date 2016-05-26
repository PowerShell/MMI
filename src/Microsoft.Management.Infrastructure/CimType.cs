/*============================================================================
 * Copyright (C) Microsoft Corporation, All rights reserved.
 *============================================================================
 */

using Microsoft.Management.Infrastructure.Native;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Management.Infrastructure
{
    /// <summary>
    /// <para>
    /// CIM type of a value.
    /// </para>
    /// <para>
    /// This is a .NET representation of intrinsic CIM types (as defined by DSP0004).
    ///
    /// The mapping of scalar types is as follows:
    /// - CIM: uint8 -> .NET: System.Byte
    /// - CIM: sint8 -> .NET: System.SByte
    /// - CIM: uint16 -> .NET: System.UInt16
    /// - CIM: sint16 -> .NET: System.Int16
    /// - CIM: uint32 -> .NET: System.UInt32
    /// - CIM: sint32 -> .NET: System.Int32
    /// - CIM: uint64 -> .NET: System.UInt64
    /// - CIM: sint64 -> .NET: System.Int64
    /// - CIM: string -> .NET: System.String
    /// - CIM: boolean -> .NET: System.Boolean
    /// - CIM: real32 -> .NET: System.Single
    /// - CIM: real64 -> .NET: System.Double
    /// - CIM: datetime -> .NET: either System.DateTime or System.TimeSpan
    /// - CIM: class ref -> .NET: CimInstance
    /// - CIM: char16 -> .NET: System.Char
    ///
    /// The mapping of arrays uses a single-dimensional .NET array of an appropriate type.
    /// The only exception is the CIM: datetime[] -> .NET: System.Object[] mapping
    /// (which is necessary because the CIM array can contain a mixture of dates and intervals).
    /// </para>
    /// </summary>
    public enum CimType
    {
        Unknown = 0,

        Boolean,
        UInt8,
        SInt8,
        UInt16,
        SInt16,
        UInt32,
        SInt32,
        UInt64,
        SInt64,
        Real32,
        Real64,
        Char16,
        DateTime,
        String,
        Reference,
        Instance,

        BooleanArray,
        UInt8Array,
        SInt8Array,
        UInt16Array,
        SInt16Array,
        UInt32Array,
        SInt32Array,
        UInt64Array,
        SInt64Array,
        Real32Array,
        Real64Array,
        Char16Array,
        DateTimeArray,
        StringArray,
        ReferenceArray,
        InstanceArray,
    }
}

namespace Microsoft.Management.Infrastructure.Internal
{
    internal static class CimTypeExtensionMethods
    {
        public static MI_Type ToMiType(this CimType cimType)
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

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "false positive.  this code is invoked from cimasyncmethodresultobserverproxy.cs")]
        public static Type ToDotNetType(this CimType cimType)
        {
            switch (cimType)
            {
                case CimType.Boolean:
                    return typeof(System.Boolean);

                case CimType.UInt8:
                    return typeof(System.Byte);

                case CimType.SInt8:
                    return typeof(System.SByte);

                case CimType.UInt16:
                    return typeof(System.UInt16);

                case CimType.SInt16:
                    return typeof(System.Int16);

                case CimType.UInt32:
                    return typeof(System.UInt32);

                case CimType.SInt32:
                    return typeof(System.Int32);

                case CimType.UInt64:
                    return typeof(System.UInt64);

                case CimType.SInt64:
                    return typeof(System.Int64);

                case CimType.Real32:
                    return typeof(System.Single);

                case CimType.Real64:
                    return typeof(System.Double);

                case CimType.Char16:
                    return typeof(System.Char);

                case CimType.DateTime:
                    return typeof(System.Object);

                case CimType.String:
                    return typeof(System.String);

                case CimType.Reference:
                case CimType.Instance:
                    return typeof(CimInstance);

                case CimType.BooleanArray:
                    return typeof(Boolean[]);

                case CimType.UInt8Array:
                    return typeof(Byte[]);

                case CimType.SInt8Array:
                    return typeof(SByte[]);

                case CimType.UInt16Array:
                    return typeof(UInt16[]);

                case CimType.SInt16Array:
                    return typeof(Int16[]);

                case CimType.UInt32Array:
                    return typeof(UInt32[]);

                case CimType.SInt32Array:
                    return typeof(Int32[]);

                case CimType.UInt64Array:
                    return typeof(UInt64[]);

                case CimType.SInt64Array:
                    return typeof(Int64[]);

                case CimType.Real32Array:
                    return typeof(Single[]);

                case CimType.Real64Array:
                    return typeof(Double[]);

                case CimType.Char16Array:
                    return typeof(Char[]);

                case CimType.DateTimeArray:
                    return typeof(object[]);

                case CimType.StringArray:
                    return typeof(String[]);

                case CimType.ReferenceArray:
                case CimType.InstanceArray:
                    return typeof(CimInstance[]);

                case CimType.Unknown:
                default:
                    Debug.Assert(false, "Unrecognized or unsupported value of CimType");
                    throw new ArgumentOutOfRangeException("cimType");
            }
        }
    }

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
    }
}