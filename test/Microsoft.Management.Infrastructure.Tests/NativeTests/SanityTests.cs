using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Xunit;
using Microsoft.Management.Infrastructure.Native;
using MMI.Tests;

namespace MMI.Tests.Native
{
    public class SanityTests : IDisposable
    {
        private readonly string ApplicationName = "MMINativeTests";

        private MI_Application application = null;
        
        public SanityTests()
        {
            MI_Instance extendedError = null;
            MI_Result res = MI_Application.Initialize(ApplicationName, out extendedError, out this.application);
            MIAssert.Succeeded(res);
        }
        
        public void Dispose()
        {
            if (this.application != null)
            {
                var shutdownTask = Task.Factory.StartNew(() => this.application.Close() );
                bool completed = shutdownTask.Wait(TimeSpan.FromSeconds(5));
                Assert.True(completed, "MI_Application did not complete shutdown in the expected time - did you leave an object open?");
                MIAssert.Succeeded(shutdownTask.Result);
            }
        }

        [Fact]
        public void CanCreateSession()
        {
            MI_Session newSession = null;
            MI_Instance extendedError = null;
            MI_Result res = this.application.NewSession("WMIDCOM",
                    null,
                    MI_DestinationOptions.Null,
                    MI_SessionCallbacks.Null,
                    out extendedError,
                    out newSession);
            MIAssert.Succeeded(res);

            res = newSession.Close(IntPtr.Zero, null);
            MIAssert.Succeeded(res);
        }

        [Fact]
        public void CanTestSessionPositive()
        {
            MI_Session session = null;
            MI_Instance extendedError = null;
            MI_Result res = this.application.NewSession("WMIDCOM",
                    null,
                    MI_DestinationOptions.Null,
                    MI_SessionCallbacks.Null,
                    out extendedError,
                    out session);
            MIAssert.Succeeded(res);

            MI_Operation operation = null;
            session.TestConnection(MI_OperationFlags.Default, null, out operation);

            bool moreResults;
            MI_Result result;
            string errorMessage = null;
            MI_Instance instance = null;
            MI_Instance errorDetails = null;
            res = operation.GetInstance(out instance, out moreResults, out result, out errorMessage, out errorDetails);
            MIAssert.Succeeded(res);
            MIAssert.Succeeded(result);

            res = operation.Close();
            MIAssert.Succeeded(res);
            res = session.Close(IntPtr.Zero, null);
            MIAssert.Succeeded(res);
        }

        [Fact]
        public void CanTestSessionNegative()
        {
            MI_Session session = null;
            MI_Instance extendedError = null;
            MI_Result res = this.application.NewSession("WMIDCOM",
                    "badhost",
                    MI_DestinationOptions.Null,
                    MI_SessionCallbacks.Null,
                    out extendedError,
                    out session);
            MIAssert.Succeeded(res);

            MI_Operation operation = null;
            session.TestConnection(0, null, out operation);

            bool moreResults;
            MI_Result result;
            string errorMessage = null;
            MI_Instance instance = null;
            MI_Instance errorDetails = null;
            res = operation.GetInstance(out instance, out moreResults, out result, out errorMessage, out errorDetails);
            MIAssert.Succeeded(res, "Expect the GetInstance operation to succeed");
            MIAssert.Failed(result, "Expect the actual retrieval to fail");
            Assert.True(!String.IsNullOrEmpty(errorMessage), "Expect error message to be available");

            res = operation.Close();
            MIAssert.Succeeded(res);
            res = session.Close(IntPtr.Zero, null);
            MIAssert.Succeeded(res);
        }

        [Fact]
        public void CanDoSimpleSessionEnumerate()
        {
            MI_Session session = null;
            MI_Instance extendedError = null;
            MI_Result res = this.application.NewSession("WMIDCOM",
                    null,
                    MI_DestinationOptions.Null,
                    MI_SessionCallbacks.Null,
                    out extendedError,
                    out session);
            MIAssert.Succeeded(res);

            MI_Operation operation = null;
            session.EnumerateInstances(MI_OperationFlags.MI_OPERATIONFLAGS_DEFAULT_RTTI,
                MI_OperationOptions.Null,
                "root/cimv2",
                "Win32_ComputerSystem",
                false,
                null,
                out operation);

            bool moreResults = true;
            MI_Instance clonedInstance = null;

            MI_Result result;
            string errorMessage = null;
            MI_Instance instanceOut = null;
            MI_Instance errorDetails = null;
            res = operation.GetInstance(out instanceOut, out moreResults, out result, out errorMessage, out errorDetails);
            MIAssert.Succeeded(res, "Expect the first GetInstance call to succeed");

            if (!instanceOut.IsNull)
            {
                res = instanceOut.Clone(out clonedInstance);
                MIAssert.Succeeded(res, "Expect the clone to succeed");
            }

            while (moreResults)
            {
                MI_Result secondaryResult = MI_Result.MI_RESULT_OK;
                res = operation.GetInstance(out instanceOut, out moreResults, out secondaryResult, out errorMessage, out errorDetails);
                MIAssert.Succeeded(res, "Expect GetInstance to succeed even if we don't want the result");
            }

            res = operation.Close();
            MIAssert.Succeeded(res, "Expect operation to close successfully");

            string className = null;
            res = clonedInstance.GetClassName(out className);
            MIAssert.Succeeded(res, "Expect GetClassName to succeed");
            Assert.Equal("Win32_ComputerSystem", className); // Expect the class name to be the one we queried

            MI_Value elementValue = null;
            MI_Type elementType;
            MI_Flags elementFlags;
            UInt32 elementIndex;
            res = clonedInstance.GetElement("Name", out elementValue, out elementType, out elementFlags, out elementIndex);
            MIAssert.Succeeded(res, "Expect GetElement to succeed");

            Assert.Equal(MI_Type.MI_STRING, elementType); // Verify that the Name property is registered as a string
            Assert.Equal(MI_Flags.MI_FLAG_KEY | MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_NOT_MODIFIED | MI_Flags.MI_FLAG_READONLY,
                elementFlags); // Expect the element flags to also be properly available from the query
            Assert.Equal(Environment.MachineName, elementValue.String); // Expect the machine name to have survived the whole journey

            res = session.Close(IntPtr.Zero, null);
            MIAssert.Succeeded(res, "Expect to be able to close the session");
        }

        [Fact]
        public void CanDoSimpleSessionGetClass()
        {
            MI_Session session = null;
            MI_Instance extendedError = null;
            MI_Result res = this.application.NewSession("WMIDCOM",
                    null,
                    MI_DestinationOptions.Null,
                    MI_SessionCallbacks.Null,
                    out extendedError,
                    out session);
            MIAssert.Succeeded(res);

            MI_Operation operation = null;
            session.GetClass(MI_OperationFlags.Default, MI_OperationOptions.Null, "root/cimv2", "Win32_ComputerSystem", null, out operation);

            MI_Class classOut;
            MI_Class clonedClass = null;
            MI_Result result;
            string errorMessage = null;
            MI_Instance errorDetails = null;
            bool moreResults = false;
            res = operation.GetClass(out classOut, out moreResults, out result, out errorMessage, out errorDetails);
            MIAssert.Succeeded(res, "Expect the first GetClass call to succeed");

            Assert.True(!classOut.IsNull, "Expect retrieved class instance to be non-null");
            res = classOut.Clone(out clonedClass);
            MIAssert.Succeeded(res, "Expect the clone to succeed");

            while (moreResults)
            {
                MI_Result secondaryResult = MI_Result.MI_RESULT_OK;
                res = operation.GetClass(out classOut, out moreResults, out secondaryResult, out errorMessage, out errorDetails);
                MIAssert.Succeeded(res, "Expect GetClass to succeed even if we don't want the result");
            }

            res = operation.Close();
            MIAssert.Succeeded(res, "Expect operation to close successfully");

            MI_Value elementValue;
            UInt32 elementIndex;
            MI_Flags elementFlags;
            MI_Type elementType;
            bool valueExists;
            string referenceClass;
            MI_QualifierSet propertyQualifierSet;
            res = clonedClass.GetElement("TotalPhysicalMemory", out elementValue, out valueExists, out elementType, out referenceClass, out propertyQualifierSet, out elementFlags, out elementIndex);
            MIAssert.Succeeded(res, "Expect to see the normal property on the class");
            Assert.Equal(MI_Type.MI_UINT64, elementType); // Expect the CIM property to have the right width
            Assert.Equal(MI_Flags.MI_FLAG_READONLY | MI_Flags.MI_FLAG_NULL | MI_Flags.MI_FLAG_PROPERTY, elementFlags); // Expect the CIM property to have the normal flags

            MI_Type miClassQualifierType;
            MI_Value miClassQualifierValue;
            MI_Flags miClassQualifierFlags;
            MI_QualifierSet classQualifierSet;
            res = clonedClass.GetClassQualifierSet(out classQualifierSet);
            MIAssert.Succeeded(res, "Expect to be able to get class qualifiers set");
            UInt32 qualifierIndex;
            res = classQualifierSet.GetQualifier("UUID", out miClassQualifierType, out miClassQualifierFlags, out miClassQualifierValue, out qualifierIndex);
            MIAssert.Succeeded(res, "Expect to be able to get qualifier information from class");

            Assert.Equal(MI_Type.MI_STRING, miClassQualifierType); // Expect qualifier type to be a string
            Assert.Equal(MI_Flags.MI_FLAG_ENABLEOVERRIDE | MI_Flags.MI_FLAG_TOSUBCLASS, miClassQualifierFlags); // Expect flags to be standard flags
            Assert.Equal("{8502C4B0-5FBB-11D2-AAC1-006008C78BC7}", miClassQualifierValue.String); // Expect UUID of class to be the known UUID

            MI_ParameterSet parameters;
            MI_QualifierSet methodQualifierSet;
            UInt32 methodIndex;
            res = clonedClass.GetMethod("SetPowerState", out methodQualifierSet, out parameters, out methodIndex);
            MIAssert.Succeeded(res, "Expect to be able to get method from class");

            UInt32 parameterCount;
            res = parameters.GetParameterCount(out parameterCount);
            MIAssert.Succeeded(res, "Expect to be able to get count from parameter set");
            Assert.Equal(2u, parameterCount); // Expect there to be the documented number of parameters

            MI_Type parameterType;
            string parameterReferenceClass;
            MI_QualifierSet parameterQualifierSet;
            UInt32 parameterIndex;
            res = parameters.GetParameter("PowerState", out parameterType, out parameterReferenceClass, out parameterQualifierSet, out parameterIndex);
            MIAssert.Succeeded(res, "Expect to be able to get parameter from parameter set");

            Assert.Equal(MI_Type.MI_UINT16, parameterType); // Expect parameter type to be the documented type
            Assert.Equal(0u, parameterIndex); // Expect the power state to be the first parameter

            res = session.Close(IntPtr.Zero, null);
            MIAssert.Succeeded(res, "Expect to be able to close the session");
        }

        [Fact]
        public void CanGetSetOperationOptionsInterval()
        {
            MI_OperationOptions options;
            this.application.NewOperationOptions(false, out options);

            MI_Interval myInterval = new MI_Interval()
            {
                days = 21,
                hours = 2,
                seconds = 1
            };

            var res = options.SetInterval("MyCustomOption", myInterval, MI_OperationOptionsFlags.Unused);
            MIAssert.Succeeded(res, "Expect to be able to set an interval");

            MI_Interval retrievedInterval;
            MI_OperationOptionsFlags retrievedFlags;
            UInt32 optionIndex;
            res = options.GetInterval("MyCustomOption", out retrievedInterval, out optionIndex, out retrievedFlags);
            MIAssert.Succeeded(res, "Expect to be able to get an interval");

            MIAssert.MIIntervalsEqual(myInterval, retrievedInterval);
        }

        [Fact]
        public void DirectInstanceTableAccessesThrowWhenNotInitialized()
        {
            Assert.Throws<InvalidOperationException>(() => MI_Instance.NewDirectPtr().Delete());
        }

        [Fact]
        public void IndirectInstanceTableAccessesThrowWhenNotInitialized()
        {
            Assert.Throws<InvalidOperationException>(() => MI_Instance.NewIndirectPtr().Delete());
        }

        [Fact]
        public void DirectApplicationTableAccessesThrowWhenNotInitialized()
        {
            Assert.Throws<InvalidOperationException>(() => MI_Application.NewDirectPtr().Close());
        }

        [Fact]
        public void IndirectApplicationTableAccessesThrowWhenNotInitialized()
        {
            Assert.Throws<InvalidOperationException>(() => MI_Application.NewIndirectPtr().Close());
        }

        [Fact]
        public void DirectSessionTableAccessesThrowWhenNotInitialized()
        {
            Assert.Throws<InvalidOperationException>(() => MI_Session.NewDirectPtr().Close(IntPtr.Zero, null));
        }

        [Fact]
        public void IndirectSessionTableAccessesThrowWhenNotInitialized()
        {
            Assert.Throws<InvalidOperationException>(() => MI_Session.NewIndirectPtr().Close(IntPtr.Zero, null));
        }

        [Fact]
        public void CanCreateSerializer()
        {
            MI_Serializer newSerializer = null;
            MI_Result res = this.application.NewSerializer(MI_SerializerFlags.None,
                MI_SerializationFormat.XML,
                out newSerializer);
            MIAssert.Succeeded(res);
            Assert.NotNull(newSerializer);

            res = newSerializer.Close();
            MIAssert.Succeeded(res);
        }

        [Fact]
        public void CanXMLSerializeInstance()
        {
            MI_Instance toSerialize;
            MI_Result res = this.application.NewInstance("TestInstance", null, out toSerialize);
            MIAssert.Succeeded(res);
            MI_Value valueToSerialize = MI_Value.NewDirectPtr();
            valueToSerialize.String = "Test string";
            res = toSerialize.AddElement("string", valueToSerialize, MI_Type.MI_STRING, MI_Flags.None);
            MIAssert.Succeeded(res);

            MI_Serializer newSerializer = null;
            res = this.application.NewSerializer(MI_SerializerFlags.None,
                MI_SerializationFormat.XML,
                out newSerializer);
            MIAssert.Succeeded(res);
            Assert.NotNull(newSerializer);

            byte[] serializedInstance;
            res = newSerializer.SerializeInstance(MI_SerializerFlags.None, toSerialize, out serializedInstance);
            MIAssert.Succeeded(res);

            string serializedString = Encoding.Unicode.GetString(serializedInstance);
            Assert.Equal("<INSTANCE CLASSNAME=\"TestInstance\"><PROPERTY NAME=\"string\" TYPE=\"string\" MODIFIED=\"TRUE\"><VALUE>Test string</VALUE></PROPERTY></INSTANCE>", serializedString);

            res = newSerializer.Close();
            MIAssert.Succeeded(res);
        }
    }
}
