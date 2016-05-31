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
    public class XMLDeserializerTests : NativeTestsBase, IClassFixture<SessionFixture>
    {   
        public XMLDeserializerTests(SessionFixture sessionFixture) : base(sessionFixture)
        {
        }

        [WindowsFact]
        public void CanCreateMOFDeserializer()
        {
            MI_Deserializer newDeserializer = null;
            MI_Result res = this.Application.NewDeserializer(MI_SerializerFlags.None,
                MI_SerializationFormat.MOF,
                out newDeserializer);
            MIAssert.Succeeded(res);
            Assert.NotNull(newDeserializer, "Expect newly created deserializer to be non-null");

            res = newDeserializer.Close();
            MIAssert.Succeeded(res);
        }
        
        [WindowsFact]
        public void CanMOFDeserializeInstance()
        {
            MI_Deserializer newDeserializer = null;
            MI_Result res = this.Application.NewDeserializer(MI_SerializerFlags.None,
                MI_SerializationFormat.MOF,
                out newDeserializer);
            MIAssert.Succeeded(res);
            Assert.NotNull(newDeserializer);
            
            MI_Operation cimClassOperation;
            this.Session.GetClass(MI_OperationFlags.Default, null, "root/cimv2", "Win32_ComputerSystem", null, out cimClassOperation);

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
        }
    }
}
