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
    public class MOFDeserializerTests : DeserializerTestsBase
    {
        public MOFDeserializerTests() : base(MI_SerializationFormat.MOF)
        {
        }

        [Fact]
        public void CanDeserializeInstance()
        {
            this.VerifyRoundTripInstance();
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

            var res = this.Deserializer.DeserializeInstanceArray(MI_SerializerFlags.None,
                MI_OperationOptions.Null,
                IntPtr.Zero,
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
    }
}
