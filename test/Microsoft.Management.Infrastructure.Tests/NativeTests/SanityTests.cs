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

        [WindowsFact]
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

        [WindowsFact]
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

        [WindowsFact]
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

        [WindowsFact]
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

        [WindowsFact]
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

        [WindowsFact]
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

        [WindowsFact]
        public void DirectInstanceTableAccessesThrowWhenNotInitialized()
        {
            Assert.Throws<InvalidOperationException>(() => MI_Instance.NewDirectPtr().Delete());
        }

        [WindowsFact]
        public void IndirectInstanceTableAccessesThrowWhenNotInitialized()
        {
            Assert.Throws<InvalidOperationException>(() => MI_Instance.NewIndirectPtr().Delete());
        }

        [WindowsFact]
        public void DirectApplicationTableAccessesThrowWhenNotInitialized()
        {
            Assert.Throws<InvalidOperationException>(() => MI_Application.NewDirectPtr().Close());
        }

        [WindowsFact]
        public void IndirectApplicationTableAccessesThrowWhenNotInitialized()
        {
            Assert.Throws<InvalidOperationException>(() => MI_Application.NewIndirectPtr().Close());
        }

        [WindowsFact]
        public void DirectSessionTableAccessesThrowWhenNotInitialized()
        {
            Assert.Throws<InvalidOperationException>(() => MI_Session.NewDirectPtr().Close(IntPtr.Zero, null));
        }

        [WindowsFact]
        public void IndirectSessionTableAccessesThrowWhenNotInitialized()
        {
            Assert.Throws<InvalidOperationException>(() => MI_Session.NewIndirectPtr().Close(IntPtr.Zero, null));
        }

        [WindowsFact]
        public void CanCreateXMLSerializer()
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

        [WindowsFact]
        public void CanCreateMOFSerializer()
        {
            MI_Serializer newSerializer = null;
            MI_Result res = this.application.NewSerializer(MI_SerializerFlags.None,
                MI_SerializationFormat.MOF,
                out newSerializer);
            MIAssert.Succeeded(res);
            Assert.NotNull(newSerializer);

            res = newSerializer.Close();
            MIAssert.Succeeded(res);
        }

        [WindowsFact]
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

        [WindowsFact]
        public void CanCreateDeserializer()
        {
            MI_Deserializer newDeserializer = null;
            MI_Result res = this.application.NewDeserializer(MI_SerializerFlags.None,
                MI_SerializationFormat.XML,
                out newDeserializer);
            MIAssert.Succeeded(res);
            Assert.NotNull(newDeserializer);

            res = newDeserializer.Close();
            MIAssert.Succeeded(res);
        }

        [WindowsFact]
        public void CanXMLDeserializeInstance()
        {
            MI_Deserializer newDeserializer = null;
            MI_Result res = this.application.NewDeserializer(MI_SerializerFlags.None,
                MI_SerializationFormat.XML,
                out newDeserializer);
            MIAssert.Succeeded(res);
            Assert.NotNull(newDeserializer);

            MI_Session newSession = null;
            MI_Instance extendedError = null;
            res = this.application.NewSession("WMIDCOM",
                    null,
                    MI_DestinationOptions.Null,
                    MI_SessionCallbacks.Null,
                    out extendedError,
                    out newSession);
            MIAssert.Succeeded(res);

            MI_Operation cimClassOperation;
            newSession.GetClass(MI_OperationFlags.Default, null, "root/cimv2", "Win32_ComputerSystem", null, out cimClassOperation);

            MI_Class cimClass;
            bool moreResults;
            MI_Result operationRes;
            MI_Instance completionDetails;
            string errorMessage;
            res = cimClassOperation.GetClass(out cimClass, out moreResults, out operationRes, out errorMessage, out completionDetails);
            MIAssert.Succeeded(res);
            MIAssert.Succeeded(operationRes);
            Assert.False(moreResults);
            cimClassOperation.Close();

            string serializedString = "<INSTANCE CLASSNAME=\"Win32_ComputerSystem\"><PROPERTY NAME=\"Caption\" TYPE=\"string\"><VALUE>BECARR-LENOVO</VALUE></PROPERTY><PROPERTY NAME=\"Description\" TYPE=\"string\"><VALUE>AT/AT COMPATIBLE</VALUE></PROPERTY><PROPERTY NAME=\"InstallDate\" TYPE=\"datetime\"></PROPERTY><PROPERTY NAME=\"Name\" TYPE=\"string\"><VALUE>BECARR-LENOVO</VALUE></PROPERTY><PROPERTY NAME=\"Status\" TYPE=\"string\"><VALUE>OK</VALUE></PROPERTY><PROPERTY NAME=\"CreationClassName\" TYPE=\"string\"><VALUE>Win32_ComputerSystem</VALUE></PROPERTY><PROPERTY NAME=\"NameFormat\" TYPE=\"string\"></PROPERTY><PROPERTY NAME=\"PrimaryOwnerContact\" TYPE=\"string\"></PROPERTY><PROPERTY NAME=\"PrimaryOwnerName\" TYPE=\"string\"><VALUE>Windows User</VALUE></PROPERTY><PROPERTY.ARRAY NAME=\"Roles\" TYPE=\"string\"><VALUE.ARRAY><VALUE>LM_Workstation</VALUE><VALUE>LM_Server</VALUE><VALUE>NT</VALUE></VALUE.ARRAY></PROPERTY.ARRAY><PROPERTY.ARRAY NAME=\"InitialLoadInfo\" TYPE=\"string\"></PROPERTY.ARRAY><PROPERTY NAME=\"LastLoadInfo\" TYPE=\"string\"></PROPERTY><PROPERTY.ARRAY NAME=\"PowerManagementCapabilities\" TYPE=\"uint16\"></PROPERTY.ARRAY><PROPERTY NAME=\"PowerManagementSupported\" TYPE=\"boolean\"></PROPERTY><PROPERTY NAME=\"PowerState\" TYPE=\"uint16\"><VALUE>0</VALUE></PROPERTY><PROPERTY NAME=\"ResetCapability\" TYPE=\"uint16\"><VALUE>1</VALUE></PROPERTY><PROPERTY NAME=\"AdminPasswordStatus\" TYPE=\"uint16\"><VALUE>0</VALUE></PROPERTY><PROPERTY NAME=\"AutomaticManagedPagefile\" TYPE=\"boolean\"><VALUE>false</VALUE></PROPERTY><PROPERTY NAME=\"AutomaticResetBootOption\" TYPE=\"boolean\"><VALUE>true</VALUE></PROPERTY><PROPERTY NAME=\"AutomaticResetCapability\" TYPE=\"boolean\"><VALUE>true</VALUE></PROPERTY><PROPERTY NAME=\"BootOptionOnLimit\" TYPE=\"uint16\"></PROPERTY><PROPERTY NAME=\"BootOptionOnWatchDog\" TYPE=\"uint16\"></PROPERTY><PROPERTY NAME=\"BootROMSupported\" TYPE=\"boolean\"><VALUE>true</VALUE></PROPERTY><PROPERTY NAME=\"BootupState\" TYPE=\"string\"><VALUE>Normal boot</VALUE></PROPERTY><PROPERTY NAME=\"ChassisBootupState\" TYPE=\"uint16\"><VALUE>2</VALUE></PROPERTY><PROPERTY NAME=\"CurrentTimeZone\" TYPE=\"sint16\"><VALUE>-420</VALUE></PROPERTY><PROPERTY NAME=\"DaylightInEffect\" TYPE=\"boolean\"><VALUE>true</VALUE></PROPERTY><PROPERTY NAME=\"DNSHostName\" TYPE=\"string\"><VALUE>becarr-lenovo</VALUE></PROPERTY><PROPERTY NAME=\"Domain\" TYPE=\"string\"><VALUE>redmond.corp.microsoft.com</VALUE></PROPERTY><PROPERTY NAME=\"DomainRole\" TYPE=\"uint16\"><VALUE>1</VALUE></PROPERTY><PROPERTY NAME=\"EnableDaylightSavingsTime\" TYPE=\"boolean\"><VALUE>true</VALUE></PROPERTY><PROPERTY NAME=\"FrontPanelResetStatus\" TYPE=\"uint16\"><VALUE>2</VALUE></PROPERTY><PROPERTY NAME=\"HypervisorPresent\" TYPE=\"boolean\"><VALUE>true</VALUE></PROPERTY><PROPERTY NAME=\"InfraredSupported\" TYPE=\"boolean\"><VALUE>false</VALUE></PROPERTY><PROPERTY NAME=\"KeyboardPasswordStatus\" TYPE=\"uint16\"><VALUE>2</VALUE></PROPERTY><PROPERTY NAME=\"Manufacturer\" TYPE=\"string\"><VALUE>LENOVO</VALUE></PROPERTY><PROPERTY NAME=\"Model\" TYPE=\"string\"><VALUE>2447MD7</VALUE></PROPERTY><PROPERTY NAME=\"NetworkServerModeEnabled\" TYPE=\"boolean\"><VALUE>true</VALUE></PROPERTY><PROPERTY NAME=\"NumberOfLogicalProcessors\" TYPE=\"uint32\"><VALUE>8</VALUE></PROPERTY><PROPERTY NAME=\"NumberOfProcessors\" TYPE=\"uint32\"><VALUE>1</VALUE></PROPERTY><PROPERTY.ARRAY NAME=\"OEMLogoBitmap\" TYPE=\"uint8\"></PROPERTY.ARRAY><PROPERTY.ARRAY NAME=\"OEMStringArray\" TYPE=\"string\"></PROPERTY.ARRAY><PROPERTY NAME=\"PartOfDomain\" TYPE=\"boolean\"><VALUE>true</VALUE></PROPERTY><PROPERTY NAME=\"PauseAfterReset\" TYPE=\"sint64\"><VALUE>-1</VALUE></PROPERTY><PROPERTY NAME=\"PCSystemType\" TYPE=\"uint16\"><VALUE>2</VALUE></PROPERTY><PROPERTY NAME=\"PCSystemTypeEx\" TYPE=\"uint16\"><VALUE>2</VALUE></PROPERTY><PROPERTY NAME=\"PowerOnPasswordStatus\" TYPE=\"uint16\"><VALUE>0</VALUE></PROPERTY><PROPERTY NAME=\"PowerSupplyState\" TYPE=\"uint16\"><VALUE>2</VALUE></PROPERTY><PROPERTY NAME=\"ResetCount\" TYPE=\"sint16\"><VALUE>-1</VALUE></PROPERTY><PROPERTY NAME=\"ResetLimit\" TYPE=\"sint16\"><VALUE>-1</VALUE></PROPERTY><PROPERTY.ARRAY NAME=\"SupportContactDescription\" TYPE=\"string\"></PROPERTY.ARRAY><PROPERTY NAME=\"SystemStartupDelay\" TYPE=\"uint16\"></PROPERTY><PROPERTY.ARRAY NAME=\"SystemStartupOptions\" TYPE=\"string\"></PROPERTY.ARRAY><PROPERTY NAME=\"SystemStartupSetting\" TYPE=\"uint8\"></PROPERTY><PROPERTY NAME=\"SystemType\" TYPE=\"string\"><VALUE>x64-based PC</VALUE></PROPERTY><PROPERTY NAME=\"ThermalState\" TYPE=\"uint16\"><VALUE>2</VALUE></PROPERTY><PROPERTY NAME=\"TotalPhysicalMemory\" TYPE=\"uint64\"><VALUE>34169835520</VALUE></PROPERTY><PROPERTY NAME=\"UserName\" TYPE=\"string\"><VALUE>REDMOND\becarr</VALUE></PROPERTY><PROPERTY NAME=\"WakeUpType\" TYPE=\"uint16\"><VALUE>6</VALUE></PROPERTY><PROPERTY NAME=\"Workgroup\" TYPE=\"string\"></PROPERTY></INSTANCE>";

            MI_Instance instance;
            uint bufferRead;
            MI_Instance cimErrorDetails;
            res = newDeserializer.DeserializeInstance(MI_SerializerFlags.None,
                Encoding.Unicode.GetBytes(serializedString),
                new MI_Class[] { cimClass, cimClass }, // Deliberate, tests the serialization of the object array
                IntPtr.Zero,
                IntPtr.Zero,
                out bufferRead,
                out instance,
                out cimErrorDetails);
            MIAssert.Succeeded(res);

            MI_Value elementValue;
            MI_Type elementType;
            MI_Flags elementFlags;
            uint elementIndex;
            res = instance.GetElement("Caption", out elementValue, out elementType, out elementFlags, out elementIndex);
            MIAssert.Succeeded(res);

            Assert.Equal(MI_Type.MI_STRING, elementType);
            Assert.Equal("BECARR-LENOVO", elementValue.String);
            instance.Delete();

            res = newDeserializer.Close();
            MIAssert.Succeeded(res);

            res = newSession.Close(IntPtr.Zero, null);
            MIAssert.Succeeded(res);
        }
    }
}
