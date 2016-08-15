/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/
namespace Microsoft.Management.Infrastructure.Native
{
    internal class MI_TypeFlags
    {
        /* MI_ARRAY is not an actual type, rather this is the bit that signifies
         * the type is an array */
        internal const MI_Type MI_ARRAY = (MI_Type)16;
    }

    internal enum MI_Type : uint
    {
        MI_BOOLEAN = 0,
        MI_UINT8 = 1,
        MI_SINT8 = 2,
        MI_UINT16 = 3,
        MI_SINT16 = 4,
        MI_UINT32 = 5,
        MI_SINT32 = 6,
        MI_UINT64 = 7,
        MI_SINT64 = 8,
        MI_REAL32 = 9,
        MI_REAL64 = 10,
        MI_CHAR16 = 11,
        MI_DATETIME = 12,
        MI_STRING = 13,
        MI_REFERENCE = 14,
        MI_INSTANCE = 15,
        MI_BOOLEANA = 16,
        MI_UINT8A = 17,
        MI_SINT8A = 18,
        MI_UINT16A = 19,
        MI_SINT16A = 20,
        MI_UINT32A = 21,
        MI_SINT32A = 22,
        MI_UINT64A = 23,
        MI_SINT64A = 24,
        MI_REAL32A = 25,
        MI_REAL64A = 26,
        MI_CHAR16A = 27,
        MI_DATETIMEA = 28,
        MI_STRINGA = 29,
        MI_REFERENCEA = 30,
        MI_INSTANCEA = 31
    }
}
