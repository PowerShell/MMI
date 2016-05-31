using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Management.Infrastructure.Native;
using MMI.Tests;

namespace MMI.Tests.Native
{
    public class DeserializerTests : NativeTestsBase
    {   
        public DeserializerTests() : base()
        {
        }
        
        [WindowsFact]
        public void CanCreateXMLDeserializer()
        {
            MI_Deserializer newDeserializer = null;
            MI_Result res = this.application.NewDeserializer(MI_SerializerFlags.None,
                MI_SerializationFormat.XML,
                out newDeserializer);
            MIAssert.Succeeded(res);
            Assert.NotNull(newDeserializer, "Expect newly created deserializer to be non-null");

            res = newDeserializer.Close();
            MIAssert.Succeeded(res);
        }

        [WindowsFact]
        public void CanCreateMOFDeserializer()
        {
            MI_Deserializer newDeserializer = null;
            MI_Result res = this.application.NewDeserializer(MI_SerializerFlags.None,
                MI_SerializationFormat.MOF,
                out newDeserializer);
            MIAssert.Succeeded(res);
            Assert.NotNull(newDeserializer, "Expect newly created deserializer to be non-null");

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
            Assert.NotNull(newDeserializer, "Expect newly created deserializer to be non-null");

            MI_Session newSession = null;
            MI_Instance extendedError = null;
            res = this.application.NewSession(MI_Protocol.WSMan,
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
            Assert.False(moreResults, "Expect no more results after getting named class");
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

            Assert.Equal(MI_Type.MI_STRING, elementType, "Expect the property to have the normal type");
            Assert.Equal("BECARR-LENOVO", elementValue.String, "Expect the property to have the value from the serialized blob");
            instance.Delete();

            res = newDeserializer.Close();
            MIAssert.Succeeded(res);

            res = newSession.Close(IntPtr.Zero, null);
            MIAssert.Succeeded(res);
        }

        [WindowsFact]
        public void CanMOFDeserializeInstance()
        {
            MI_Deserializer newDeserializer = null;
            MI_Result res = this.application.NewDeserializer(MI_SerializerFlags.None,
                MI_SerializationFormat.MOF,
                out newDeserializer);
            MIAssert.Succeeded(res);
            Assert.NotNull(newDeserializer);

            MI_Session newSession = null;
            MI_Instance extendedError = null;
            res = this.application.NewSession(MI_Protocol.WSMan,
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
            Assert.False(moreResults, "Expect no more results after getting named class");
            cimClassOperation.Close();

            string serializedString = "instance of Win32_ComputerSystem\n{\n    Caption = \"BECARR-LENOVO\";\n    Description = \"AT/AT COMPATIBLE\";\n    Name = \"BECARR-LENOVO\";\n    Status = \"OK\";\n    CreationClassName = \"Win32_ComputerSystem\";\n    PrimaryOwnerName = \"Windows User\";\n    Roles = {\"LM_Workstation\", \"LM_Server\", \"NT\"};\n    PowerState = 0;\n    ResetCapability = 1;\n    AdminPasswordStatus = 0;\n    AutomaticManagedPagefile = False;\n    AutomaticResetBootOption = True;\n    AutomaticResetCapability = True;\n    BootROMSupported = True;\n    BootupState = \"Normal boot\";\n    ChassisBootupState = 2;\n    CurrentTimeZone = -420;\n    DaylightInEffect = True;\n    DNSHostName = \"becarr-lenovo\";\n    Domain = \"redmond.corp.microsoft.com\";\n    DomainRole = 1;\n    EnableDaylightSavingsTime = True;\n    FrontPanelResetStatus = 2;\n    HypervisorPresent = True;\n    InfraredSupported = False;\n    KeyboardPasswordStatus = 2;\n    Manufacturer = \"LENOVO\";\n    Model = \"2447MD7\";\n    NetworkServerModeEnabled = True;\n    NumberOfLogicalProcessors = 8;\n    NumberOfProcessors = 1;\n    PartOfDomain = True;\n    PauseAfterReset = -1;\n    PCSystemType = 2;\n    PCSystemTypeEx = 2;\n    PowerOnPasswordStatus = 0;\n    PowerSupplyState = 2;\n    ResetCount = -1;\n    ResetLimit = -1;\n    SystemType = \"x64-based PC\";\n    ThermalState = 2;\n    TotalPhysicalMemory = 34169835520;\n    UserName = \"REDMOND\\\\becarr\";\n    WakeUpType = 6;\n};\n\n";

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

            Assert.Equal(MI_Type.MI_STRING, elementType, "Expect the Caption to have the normal type");
            Assert.Equal("BECARR-LENOVO", elementValue.String, "Expect the Caption to have the machine name from the serialized blob");
            instance.Delete();

            res = newDeserializer.Close();
            MIAssert.Succeeded(res);

            res = newSession.Close(IntPtr.Zero, null);
            MIAssert.Succeeded(res);
        }
    }
}
