/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


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
