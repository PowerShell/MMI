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
using Xunit;

namespace MMI.Tests.Native
{
    public class MOFDeserializerTests : NativeDeserializerTestsBase
    {
        public MOFDeserializerTests() : base(MI_SerializationFormat.MOF)
        {
        }

/* @TODO Fix me later 
        [Fact]
        public void CanDeserializeInstance()
        {
            this.VerifyRoundTripInstance();
        }
*/

/*        [WindowsFact]
        public void CanDeserializeClass()
        {
            this.VerifyRoundTripClass();
        }

        [WindowsFact]
        public void CanDeserializeInstanceWithCallback()
        {
            this.VerifyRoundtripWithCallback();
        }

        [WindowsFact]
        public void CanDeserializeInstanceWithInternalMOFMethod()
        {
            var cimClass = this.GetClassDefinition();
            var serializedInstance = this.GetSerializedSingleton();

            MI_ExtendedArray deserializedArray;
            MI_Instance[] instances;
            uint bufferRead;
            MI_Instance cimErrorDetails;

            MI_DeserializerCallbacks callbacks = new MI_DeserializerCallbacks();

            var res = this.Deserializer.DeserializeInstanceArray(MI_SerializerFlags.None,
                MI_OperationOptions.Null,
                callbacks,
                serializedInstance,
                new MI_Class[] { cimClass },
                out bufferRead,
                out deserializedArray,
                out cimErrorDetails);
            MIAssert.Succeeded(res, "Expect to be able to deserialize instance");

            instances = deserializedArray.ReadAsManagedPointerArray(MI_Instance.NewFromDirectPtr);
            var expectedInstance = this.GetSerializableInstance();
            Assert.Equal(1, instances.Length, "Expect only the instance we serialized");
            MIAssert.InstancesEqual(expectedInstance, instances[0], "SerializedInstance");

            cimClass.Delete();
            expectedInstance.Delete();
            deserializedArray.Delete();
        }

        [WindowsFact]
        public void CanDeserializeClassWithInternalMOFMethod()
        {
            var serializedClass = this.GetSerializedClass();

            MI_ExtendedArray deserializedArray;
            MI_Class[] classes;
            uint bufferRead;
            MI_Instance cimErrorDetails;

            MI_DeserializerCallbacks callbacks = new MI_DeserializerCallbacks();

            var res = this.Deserializer.DeserializeClassArray(MI_SerializerFlags.None,
                MI_OperationOptions.Null,
                callbacks,
                serializedClass,
                new MI_Class[] {},
                null,
                null,
                out bufferRead,
                out deserializedArray,
                out cimErrorDetails);
            MIAssert.Succeeded(res, "Expect to be able to deserialize instance");

            classes = deserializedArray.ReadAsManagedPointerArray(MI_Class.NewFromDirectPtr);
            var expectedInstance = this.GetSerializableInstance();
            Assert.Equal(1, classes.Length, "Expect only the class we serialized");
            MI_Value elementValue;
            UInt32 elementIndex;
            MI_Type elementType;
            MI_Flags elementFlags;
            bool valueExists;
            string referenceClass;
            MI_QualifierSet qualifierSet;
            res = classes[0].GetElement(SerializationTestData.SerializableClassStringProperty, out elementValue, out valueExists, out elementType, out referenceClass, out qualifierSet, out elementFlags, out elementIndex);
            MIAssert.Succeeded(res, "Expect to be able to get property from deserialized class");
            Assert.Equal(MI_Type.MI_STRING, elementType, "Expect the type to be correct");
            
            expectedInstance.Delete();
            deserializedArray.Delete();
        }
*/
    }
}
