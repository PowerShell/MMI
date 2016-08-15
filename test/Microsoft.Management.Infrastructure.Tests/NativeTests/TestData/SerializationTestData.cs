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
    internal static class SerializationTestData
    {
#if !_LINUX
        internal const string SingletonClassNamespace = "root/cimv2";
        internal const string SingletonClassClassname = "Win32_ComputerSystem";
        internal const string SerializableClassNamespace = "root/cimv2";
        internal const string SerializableClassClassname = "CIM_Error";
        internal const string SingletonClassSerializableClassHeuristicString = "CIMStatusCodeDescription";
        internal const string SerializableClassStringProperty = "OtherErrorSourceFormat";
#else
        internal const string SingletonClassNamespace = "root/test";
        internal const string SingletonClassClassname = "TestClass_AllDMTFTypes";
        internal const string SingletonClassSerializationHeuristicString = "TestClass_PropertyValues";
        internal const string SerializableClassNamespace = "root/cimv2";
        internal const string SerializableClassClassname = "CIM_Error";
        internal const string SingletonClassSerializableClassHeuristicString = "CIMStatusCodeDescription";
        internal const string SerializableClassStringProperty = "OtherErrorSourceFormat";
#endif

        internal static MI_Instance CreateBasicSerializableTestInstance()
        {
            MI_Instance toSerialize;
            MI_Result res = StaticFixtures.Application.NewInstance("TestInstance", null, out toSerialize);
            MIAssert.Succeeded(res);
            MI_Value valueToSerialize = MI_Value.NewDirectPtr();
            valueToSerialize.String = "Test string";
            res = toSerialize.AddElement("string", valueToSerialize, MI_Type.MI_STRING, MI_Flags.None);
            MIAssert.Succeeded(res);

            return toSerialize;
        }

        internal static MI_Class GetSerializableTestClass()
        {
            MI_Operation queryOperation;
            StaticFixtures.Session.GetClass(MI_OperationFlags.Default, null, SerializableClassNamespace, SerializableClassClassname, null, out queryOperation);

            MI_Class classResult;
            MI_Result operationResult;
            bool moreResults;
            string errorMessage;
            MI_Instance completionDetails;
            MI_Result res = queryOperation.GetClass(out classResult, out moreResults, out operationResult, out errorMessage, out completionDetails);
            MIAssert.Succeeded(res);
            MIAssert.Succeeded(operationResult);
            Assert.False(moreResults);

            MI_Class clonedClass;
            res = classResult.Clone(out clonedClass);
            MIAssert.Succeeded(res);

            queryOperation.Close();

            return clonedClass;
        }

        public const string BasicSerializableTestInstanceXMLRepresentation = "<INSTANCE CLASSNAME=\"TestInstance\"><PROPERTY NAME=\"string\" TYPE=\"string\" MODIFIED=\"TRUE\"><VALUE>Test string</VALUE></PROPERTY></INSTANCE>";
        public const string BasicSerializableTestInstanceMOFRepresentation = "instance of TestInstance\n{\n    string = \"Test string\";\n};\n\n";
    }
}
