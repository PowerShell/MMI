using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Management.Infrastructure.Native;
using MMI.Tests;

namespace MMI.Tests.Native
{
    public class SessionTests : NativeTestsBase
    {
        internal MI_Session session = null;

        public SessionTests() : base()
        {
            MI_Instance extendedError = null;
            MI_Result res = this.application.NewSession(null,
                    null,
                    MI_DestinationOptions.Null,
                    MI_SessionCallbacks.Null,
                    out extendedError,
                    out this.session);
            MIAssert.Succeeded(res, "Expect simple NewSession to succeed");
        }

        public override void Dispose()
        {
            var res = this.session.Close(IntPtr.Zero, null);
            MIAssert.Succeeded(res, "Expect simple session close to succceed");
            base.Dispose();
        }

        [WindowsFact]
        public void TestSessionPositive()
        {
            MI_Session session = null;
            MI_Instance extendedError = null;
            MI_Result res = this.application.NewSession(MI_Protocol.WSMan,
                    null,
                    MI_DestinationOptions.Null,
                    MI_SessionCallbacks.Null,
                    out extendedError,
                    out session);
            MIAssert.Succeeded(res, "Expect simple NewSession to succeed");

            MI_Operation operation = null;
            session.TestConnection(MI_OperationFlags.Default, null, out operation);

            bool moreResults;
            MI_Result result;
            string errorMessage = null;
            MI_Instance instance = null;
            MI_Instance errorDetails = null;
            res = operation.GetInstance(out instance, out moreResults, out result, out errorMessage, out errorDetails);
            MIAssert.Succeeded(res, "Expect GetInstance result to succeed");
            MIAssert.Succeeded(result, "Expect actual operation result to be success");

            res = operation.Close();
            MIAssert.Succeeded(res, "Expect to be able to close completed operation");
            res = session.Close(IntPtr.Zero, null);
            MIAssert.Succeeded(res, "Expect to be able to close session");
        }

        [WindowsFact]
        public void TestSessionNegative()
        {
            MI_Session session = null;
            MI_Instance extendedError = null;
            MI_Result res = this.application.NewSession(MI_Protocol.WSMan,
                    "badhost",
                    MI_DestinationOptions.Null,
                    MI_SessionCallbacks.Null,
                    out extendedError,
                    out session);
            MIAssert.Succeeded(res, "Expect simple NewSession to succeed");

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
            MIAssert.Succeeded(res, "Expect to be able to close operation now");
            res = session.Close(IntPtr.Zero, null);
            MIAssert.Succeeded(res, "Expect to be able to close session");
        }

        [WindowsFact]
        public void SimpleEnumerateInstance()
        {
            MI_Session session = null;
            MI_Instance extendedError = null;
            MI_Result res = this.application.NewSession(MI_Protocol.WSMan,
                    null,
                    MI_DestinationOptions.Null,
                    MI_SessionCallbacks.Null,
                    out extendedError,
                    out session);
            MIAssert.Succeeded(res, "Expect simple NewSession to succeed");

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
            Assert.Equal("Win32_ComputerSystem", className, "Expect the class name to be the one we queried");

            MI_Value elementValue = null;
            MI_Type elementType;
            MI_Flags elementFlags;
            UInt32 elementIndex;
            res = clonedInstance.GetElement("Name", out elementValue, out elementType, out elementFlags, out elementIndex);
            MIAssert.Succeeded(res, "Expect GetElement to succeed");

            Assert.Equal(MI_Type.MI_STRING, elementType, "Expect that the Name property is registered as a string");
            Assert.Equal(MI_Flags.MI_FLAG_KEY | MI_Flags.MI_FLAG_PROPERTY | MI_Flags.MI_FLAG_NOT_MODIFIED | MI_Flags.MI_FLAG_READONLY,
                elementFlags, "Expect the element flags to also be properly available from the query");
            Assert.Equal(Environment.MachineName, elementValue.String, "Expect the machine name to have survived the whole journey");

            res = session.Close(IntPtr.Zero, null);
            MIAssert.Succeeded(res, "Expect to be able to close the session");
        }

        [WindowsFact]
        public void SimpleGetClass()
        {
            MI_Session session = null;
            MI_Instance extendedError = null;
            MI_Result res = this.application.NewSession(MI_Protocol.WSMan,
                    null,
                    MI_DestinationOptions.Null,
                    MI_SessionCallbacks.Null,
                    out extendedError,
                    out session);
            MIAssert.Succeeded(res, "Expect simple NewSession to succeed");

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
            Assert.Equal(MI_Type.MI_UINT64, elementType, "Expect the CIM property to have the right width");
            Assert.Equal(MI_Flags.MI_FLAG_READONLY | MI_Flags.MI_FLAG_NULL | MI_Flags.MI_FLAG_PROPERTY, elementFlags, "Expect the CIM property to have the normal flags");

            MI_Type miClassQualifierType;
            MI_Value miClassQualifierValue;
            MI_Flags miClassQualifierFlags;
            MI_QualifierSet classQualifierSet;
            res = clonedClass.GetClassQualifierSet(out classQualifierSet);
            MIAssert.Succeeded(res, "Expect to be able to get class qualifiers set");
            UInt32 qualifierIndex;
            res = classQualifierSet.GetQualifier("UUID", out miClassQualifierType, out miClassQualifierFlags, out miClassQualifierValue, out qualifierIndex);
            MIAssert.Succeeded(res, "Expect to be able to get qualifier information from class");

            Assert.Equal(MI_Type.MI_STRING, miClassQualifierType, "Expect qualifier type to be a string");
            Assert.True((miClassQualifierFlags & MI_Flags.MI_FLAG_ENABLEOVERRIDE) != 0, "Expect flags to be standard flags");
            Assert.Equal("{8502C4B0-5FBB-11D2-AAC1-006008C78BC7}", miClassQualifierValue.String, "Expect UUID of class to be the known UUID");

            MI_ParameterSet parameters;
            MI_QualifierSet methodQualifierSet;
            UInt32 methodIndex;
            res = clonedClass.GetMethod("SetPowerState", out methodQualifierSet, out parameters, out methodIndex);
            MIAssert.Succeeded(res, "Expect to be able to get method from class");

            UInt32 parameterCount;
            res = parameters.GetParameterCount(out parameterCount);
            MIAssert.Succeeded(res, "Expect to be able to get count from parameter set");
            Assert.Equal(2u, parameterCount, "Expect there to be the documented number of parameters");

            MI_Type parameterType;
            string parameterReferenceClass;
            MI_QualifierSet parameterQualifierSet;
            UInt32 parameterIndex;
            res = parameters.GetParameter("PowerState", out parameterType, out parameterReferenceClass, out parameterQualifierSet, out parameterIndex);
            MIAssert.Succeeded(res, "Expect to be able to get parameter from parameter set");

            Assert.Equal(MI_Type.MI_UINT16, parameterType, "Expect parameter type to be the documented type");
            Assert.Equal(0u, parameterIndex, "Expect the power state to be the first parameter");

            res = session.Close(IntPtr.Zero, null);
            MIAssert.Succeeded(res, "Expect to be able to close the session");
        }
    }
}
