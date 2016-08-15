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
    public class NativeDeserializerTestsBase : NativeTestsBase
    {
        internal MI_Deserializer Deserializer;
        
        private string format;

        public NativeDeserializerTestsBase(string format) : base()
        {
            this.format = format;
            var application = StaticFixtures.Application;

            MI_Deserializer newDeserializer = null;
            MI_Result res = application.NewDeserializer(MI_SerializerFlags.None,
                format,
                out newDeserializer);
            MIAssert.Succeeded(res);
            Assert.NotNull(newDeserializer, "Expect newly created deserializer to be non-null");
            this.Deserializer = newDeserializer;
        }

        public virtual void Dispose()
        {
            if (this.Deserializer != null)
            {
                this.Deserializer.Close();
            }
        }

        internal MI_Class GetClassDefinition()
        {
            MI_Operation cimClassOperation;
            this.Session.GetClass(MI_OperationFlags.Default, null, SerializationTestData.SingletonClassNamespace, SerializationTestData.SingletonClassClassname, null, out cimClassOperation);

            MI_Class cimClass;
            MI_Class cimClassOut;
            bool moreResults;
            MI_Result operationRes;
            MI_Instance completionDetails;
            string errorMessage;
            var res = cimClassOperation.GetClass(out cimClass, out moreResults, out operationRes, out errorMessage, out completionDetails);
            MIAssert.Succeeded(res);
            MIAssert.Succeeded(operationRes);
            Assert.False(moreResults, "Expect no more results after getting named class");

            res = cimClass.Clone(out cimClassOut);
            MIAssert.Succeeded(res, "Class Clone failed");

            cimClassOperation.Close();

            return cimClassOut;
        }

        internal MI_Instance GetSerializableInstance()
        {
            MI_Operation cimInstanceOperation;
            this.Session.EnumerateInstances(MI_OperationFlags.Default, null, SerializationTestData.SingletonClassNamespace, SerializationTestData.SingletonClassClassname, true, null, out cimInstanceOperation);

            MI_Instance instance;
            MI_Instance outInstance;
            bool moreResults;
            MI_Result operationRes;
            MI_Instance completionDetails;
            string errorMessage;
            var res = cimInstanceOperation.GetInstance(out instance, out moreResults, out operationRes, out errorMessage, out completionDetails);
            MIAssert.Succeeded(res);
            MIAssert.Succeeded(operationRes);

            res = instance.Clone(out outInstance);
            MIAssert.Succeeded(res);

            while (moreResults)
            {
                res = cimInstanceOperation.GetInstance(out instance, out moreResults, out operationRes, out errorMessage, out completionDetails);
                MIAssert.Succeeded(res);
            }

            cimInstanceOperation.Close();
            
            return outInstance;
        }

        internal void VerifyRoundTripInstance()
        {
            var cimClass = this.GetClassDefinition();
            var serializedInstance = this.GetSerializedSingleton();

            MI_Instance instance;
            uint bufferRead;
            MI_Instance cimErrorDetails;

            var res = this.Deserializer.DeserializeInstance(MI_SerializerFlags.None,
                serializedInstance,
                new MI_Class[] { cimClass },
                null,
                out bufferRead,
                out instance,
                out cimErrorDetails);
            MIAssert.Succeeded(res, "Expect to be able to deserialize instance");

            var expectedInstance = this.GetSerializableInstance();
            MIAssert.InstancesEqual(expectedInstance, instance, "SerializedInstance");

            cimClass.Delete();
            instance.Delete();
            expectedInstance.Delete();
        }

        internal void VerifyRoundtripWithCallback()
        {
            var cimClass = this.GetClassDefinition();
            var serializedInstance = this.GetSerializedSingleton();

            MI_Instance instance;
            uint bufferRead;
            MI_Instance cimErrorDetails;

            bool callbackCalled = false;
            Exception callbackException = null;
            MI_Deserializer.MI_Deserializer_ClassObjectNeeded callback = delegate (
                string serverName,
                string namespaceName,
                string className,
                out MI_Class requestedObject
                )
            {
                requestedObject = null;
                callbackCalled = true;
                try
                {
                    Assert.Null(serverName, "Expect server name to be null because we connected to local host");

                    // TODO: Verify that this being null is expected where the namespace is root/cimv2
                    //Assert.Equal(SerializationTestData.SingletonClassNamespace, namespaceName, "Expect namespace to be the namespace of the class");

                    Assert.Equal(SerializationTestData.SingletonClassClassname, className, "Expect namespace to be the namespace of the class");
                }
                catch (Exception ex)
                {
                    callbackException = ex;
                    return MI_Result.MI_RESULT_FAILED;
                }

                requestedObject = cimClass;
                return MI_Result.MI_RESULT_OK;
            };

            var res = this.Deserializer.DeserializeInstance(MI_SerializerFlags.None,
                serializedInstance,
                new MI_Class[0],
                callback,
                out bufferRead,
                out instance,
                out cimErrorDetails);
            Assert.True(callbackCalled, "Expect callback to be called");
            if (callbackException != null)
            {
                throw callbackException;
            }

            MIAssert.Succeeded(res, "Expect to be able to deserialize instance");

            var expectedInstance = this.GetSerializableInstance();
            MIAssert.InstancesEqual(expectedInstance, instance, "SerializedInstance");

            cimClass.Delete();
            instance.Delete();
            expectedInstance.Delete();
        }

        internal void VerifyRoundTripClass()
        {
            var cimClass = this.GetClassDefinition();
            var serializedClass = this.GetSerializedClass();

            MI_Class deserializedClass;
            uint bufferRead;
            MI_Instance cimErrorDetails;

            var res = this.Deserializer.DeserializeClass(MI_SerializerFlags.None,
                serializedClass,
                null,
                null,
                null,
                IntPtr.Zero,
                IntPtr.Zero,
                out bufferRead,
                out deserializedClass,
                out cimErrorDetails);
            MIAssert.Succeeded(res, "Expect to be able to deserialize instance");

            MI_Value elementValue;
            UInt32 elementIndex;
            MI_Type elementType;
            MI_Flags elementFlags;
            bool valueExists;
            string referenceClass;
            MI_QualifierSet qualifierSet;
            res = deserializedClass.GetElement(SerializationTestData.SerializableClassStringProperty, out elementValue, out valueExists, out elementType, out referenceClass, out qualifierSet, out elementFlags, out elementIndex);
            MIAssert.Succeeded(res, "Expect to be able to get property from deserialized class");
            Assert.Equal(MI_Type.MI_STRING, elementType, "Expect the type to be correct");

            cimClass.Delete();
            deserializedClass.Delete();
        }

        internal byte[] GetSerializedSingleton()
        {
            NativeSerializerTestsBase serializerHelper = new NativeSerializerTestsBase(this.format);
            return serializerHelper.GetSerializationFromInstanceThunk(this.GetSerializableInstance);
        }

        internal byte[] GetSerializedClass()
        {
            NativeSerializerTestsBase serializerHelper = new NativeSerializerTestsBase(this.format);
            return serializerHelper.GetSerializationFromClassThunk(SerializationTestData.GetSerializableTestClass);
        }
    }
}
