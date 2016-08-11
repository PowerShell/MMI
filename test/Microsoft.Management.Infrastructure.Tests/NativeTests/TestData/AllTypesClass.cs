/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/
using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Management.Infrastructure.Native;
using MMI.Tests;

namespace MMI.Tests.Native
{
    internal class AllTypesClass
    {
        internal static class AllTypesClassQualifiers
        {
            static MI_QualifierDecl ClassVersion_qual_decl = MI_QualifierDecl.NewDirectPtr("ClassVersion",
                MI_Type.MI_STRING,
                MI_Flags.MI_FLAG_ASSOCIATION | MI_Flags.MI_FLAG_CLASS | MI_Flags.MI_FLAG_INDICATION,
                MI_Flags.MI_FLAG_ENABLEOVERRIDE | MI_Flags.MI_FLAG_RESTRICTED,
                0,
                IntPtr.Zero);

            static MI_QualifierDecl CmdletAliases_qual_decl = MI_QualifierDecl.NewDirectPtr("CmdletAliases",
                MI_Type.MI_STRINGA,
                MI_Flags.MI_FLAG_METHOD,
                MI_Flags.MI_FLAG_ENABLEOVERRIDE | MI_Flags.MI_FLAG_TOSUBCLASS,
                0,
                IntPtr.Zero);

            static MI_QualifierDecl Delegate_qual_decl = MI_QualifierDecl.NewDirectPtr("Delegate",
                MI_Type.MI_BOOLEAN,
                MI_Flags.MI_FLAG_METHOD | MI_Flags.MI_FLAG_PARAMETER,
                MI_Flags.MI_FLAG_ENABLEOVERRIDE | MI_Flags.MI_FLAG_TOSUBCLASS,
                0,
                false);

            static MI_QualifierDecl Name_qual_decl = MI_QualifierDecl.NewDirectPtr("Name",
                MI_Type.MI_STRING,
                MI_Flags.MI_FLAG_METHOD,
                MI_Flags.MI_FLAG_ENABLEOVERRIDE | MI_Flags.MI_FLAG_TOSUBCLASS,
                0,
                IntPtr.Zero);

            static MI_QualifierDecl Observable_qual_decl = MI_QualifierDecl.NewDirectPtr("Observable",
                MI_Type.MI_BOOLEAN,
                MI_Flags.MI_FLAG_METHOD | MI_Flags.MI_FLAG_PARAMETER,
                MI_Flags.MI_FLAG_ENABLEOVERRIDE | MI_Flags.MI_FLAG_TOSUBCLASS,
                0,
                false);

            static MI_QualifierDecl Stream_qual_decl = MI_QualifierDecl.NewDirectPtr("Stream",
                MI_Type.MI_BOOLEAN,
                MI_Flags.MI_FLAG_METHOD | MI_Flags.MI_FLAG_PARAMETER,
                MI_Flags.MI_FLAG_ENABLEOVERRIDE | MI_Flags.MI_FLAG_TOSUBCLASS,
                0,
                false);

            static MI_QualifierDecl Structure_qual_decl = MI_QualifierDecl.NewDirectPtr("Structure",
                MI_Type.MI_BOOLEAN,
                MI_Flags.MI_FLAG_CLASS | MI_Flags.MI_FLAG_INDICATION,
                MI_Flags.MI_FLAG_ENABLEOVERRIDE | MI_Flags.MI_FLAG_TOSUBCLASS,
                0,
                false);

            static MI_QualifierDecl Switch_qual_decl = MI_QualifierDecl.NewDirectPtr("Switch",
                MI_Type.MI_BOOLEAN,
                MI_Flags.MI_FLAG_PARAMETER,
                MI_Flags.MI_FLAG_ENABLEOVERRIDE | MI_Flags.MI_FLAG_TOSUBCLASS,
                0,
                false);

            static MI_QualifierDecl UpstreamPipe_qual_decl = MI_QualifierDecl.NewDirectPtr("UpstreamPipe",
                MI_Type.MI_BOOLEAN,
                MI_Flags.MI_FLAG_PARAMETER,
                MI_Flags.MI_FLAG_ENABLEOVERRIDE | MI_Flags.MI_FLAG_TOSUBCLASS,
                0,
                false);

            static MI_QualifierDecl ValueFromPipeline_qual_decl = MI_QualifierDecl.NewDirectPtr("ValueFromPipeline",
                MI_Type.MI_BOOLEAN,
                MI_Flags.MI_FLAG_PARAMETER,
                MI_Flags.MI_FLAG_ENABLEOVERRIDE | MI_Flags.MI_FLAG_TOSUBCLASS,
                0,
                false);

            internal static MI_QualifierDecl[] qualifierDecls = new MI_QualifierDecl[]
            {
                ClassVersion_qual_decl,
                CmdletAliases_qual_decl,
                Delegate_qual_decl,
                Name_qual_decl,
                Observable_qual_decl,
                Stream_qual_decl,
                Structure_qual_decl,
                Switch_qual_decl,
                UpstreamPipe_qual_decl,
                ValueFromPipeline_qual_decl,
            };
        }

        internal static class AllTypesProperties
        {
            static MI_PropertyDecl TestClass_AllTypes_BooleanValue_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0062650C,
                "BooleanValue",
                IntPtr.Zero,
                0,
                MI_Type.MI_BOOLEAN,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_Uint8Value_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0075650A,
                "Uint8Value",
                IntPtr.Zero,
                0,
                MI_Type.MI_UINT8,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_Sint8Value_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0073650A,
                "Sint8Value",
                IntPtr.Zero,
                0,
                MI_Type.MI_SINT8,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_Uint16Value_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0075650B,
                "Uint16Value",
                IntPtr.Zero,
                0,
                MI_Type.MI_UINT16,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_Sint16Value_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0073650B,
                "Sint16Value",
                IntPtr.Zero,
                0,
                MI_Type.MI_SINT16,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_Uint32Value_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0075650B,
                "Uint32Value",
                IntPtr.Zero,
                0,
                MI_Type.MI_UINT32,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);
            
            static MI_PropertyDecl TestClass_AllTypes_Sint32Value_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0073650B,
                "Sint32Value",
                IntPtr.Zero,
                0,
                MI_Type.MI_UINT32,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_Uint64Value_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0075650B,
                "Uint64Value",
                IntPtr.Zero,
                0,
                MI_Type.MI_UINT64,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_Sint64Value_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0073650B,
                "Sint64Value",
                IntPtr.Zero,
                0,
                MI_Type.MI_SINT64,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_Real32Value_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0072650B,
                "Real32Value",
                IntPtr.Zero,
                0,
                MI_Type.MI_REAL32,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_Real64Value_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0072650B,
                "Real64Value",
                IntPtr.Zero,
                0,
                MI_Type.MI_REAL64,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_Char16Value_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0063650B,
                "Char16Value",
                IntPtr.Zero,
                0,
                MI_Type.MI_CHAR16,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_TimestampValue_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0074650E,
                "TimestampValue",
                IntPtr.Zero,
                0,
                MI_Type.MI_DATETIME,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_IntervalValue_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0069650D,
                "IntervalValue",
                IntPtr.Zero,
                0,
                MI_Type.MI_DATETIME,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_StringValue_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0073650B,
                "StringValue",
                IntPtr.Zero,
                0,
                MI_Type.MI_STRING,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_BooleanArray_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0062790C,
                "BooleanArray",
                IntPtr.Zero,
                0,
                MI_Type.MI_BOOLEANA,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_Uint8Array_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0075790A,
                "Uint8Array",
                IntPtr.Zero,
                0,
                MI_Type.MI_UINT8A,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_Sint8Array_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0073790A,
                "Sint8Array",
                IntPtr.Zero,
                0,
                MI_Type.MI_SINT8A,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_Uint16Array_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0075790B,
                "Uint16Array",
                IntPtr.Zero,
                0,
                MI_Type.MI_UINT16A,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_Sint16Array_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0073790B,
                "Sint16Array",
                IntPtr.Zero,
                0,
                MI_Type.MI_SINT16A,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_Uint32Array_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0075790B,
                "Uint32Array",
                IntPtr.Zero,
                0,
                MI_Type.MI_UINT32A,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_Sint32Array_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0073790B,
                "Sint32Array",
                IntPtr.Zero,
                0,
                MI_Type.MI_SINT32A,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_Uint64Array_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0075790B,
                "Uint64Array",
                IntPtr.Zero,
                0,
                MI_Type.MI_UINT64A,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_Sint64Array_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0073790B,
                "Sint64Array",
                IntPtr.Zero,
                0,
                MI_Type.MI_SINT64A,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_Real32Array_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0072790B,
                "Real32Array",
                IntPtr.Zero,
                0,
                MI_Type.MI_REAL32A,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_Real64Array_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0072790B,
                "Real64Array",
                IntPtr.Zero,
                0,
                MI_Type.MI_REAL64A,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_Char16Array_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0063790B,
                "Char16Array",
                IntPtr.Zero,
                0,
                MI_Type.MI_CHAR16A,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_DatetimeArray_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0064790D,
                "DatetimeArray",
                IntPtr.Zero,
                0,
                MI_Type.MI_DATETIMEA,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);

            static MI_PropertyDecl TestClass_AllTypes_StringArray_prop = MI_PropertyDecl.NewDirectPtr(
                MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_READONLY,
                0x0073790B,
                "StringArray",
                IntPtr.Zero,
                0,
                MI_Type.MI_STRINGA,
                null,
                0,
                7,
                "TestClass_AllTypes",
                "TestClass_AllTypes",
                IntPtr.Zero);


            internal static MI_PropertyDecl[] TestClass_AllTypes_props = new MI_PropertyDecl[]
            {
                TestClass_AllTypes_BooleanValue_prop,
                TestClass_AllTypes_Uint8Value_prop,
                TestClass_AllTypes_Sint8Value_prop,
                TestClass_AllTypes_Uint16Value_prop,
                TestClass_AllTypes_Sint16Value_prop,
                TestClass_AllTypes_Uint32Value_prop,
                TestClass_AllTypes_Sint32Value_prop,
                TestClass_AllTypes_Uint64Value_prop,
                TestClass_AllTypes_Sint64Value_prop,
                TestClass_AllTypes_Real32Value_prop,
                TestClass_AllTypes_Real64Value_prop,
                TestClass_AllTypes_Char16Value_prop,
                TestClass_AllTypes_TimestampValue_prop,
                TestClass_AllTypes_IntervalValue_prop,
                TestClass_AllTypes_StringValue_prop,
                TestClass_AllTypes_BooleanArray_prop,
                TestClass_AllTypes_Uint8Array_prop,
                TestClass_AllTypes_Sint8Array_prop,
                TestClass_AllTypes_Uint16Array_prop,
                TestClass_AllTypes_Sint16Array_prop,
                TestClass_AllTypes_Uint32Array_prop,
                TestClass_AllTypes_Sint32Array_prop,
                TestClass_AllTypes_Uint64Array_prop,
                TestClass_AllTypes_Sint64Array_prop,
                TestClass_AllTypes_Real32Array_prop,
                TestClass_AllTypes_Real64Array_prop,
                TestClass_AllTypes_Char16Array_prop,
                TestClass_AllTypes_DatetimeArray_prop,
                TestClass_AllTypes_StringArray_prop,
            };
        }
    }
}
