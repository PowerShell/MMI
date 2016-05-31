using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Management.Infrastructure.Native;
using MMI.Tests;

namespace MMI.Tests.Native
{
    public class MOFSerializerTests : NativeTestsBase
    {   
        public MOFSerializerTests() : base()
        {
        }

        [WindowsFact]
        public void CanCreateSerializer()
        {
            MI_Serializer newSerializer = null;
            MI_Result res = this.application.NewSerializer(MI_SerializerFlags.None,
                MI_SerializationFormat.MOF,
                out newSerializer);
            MIAssert.Succeeded(res);
            Assert.NotNull(newSerializer, "Expect newly created serializer to be non-null");

            res = newSerializer.Close();
            MIAssert.Succeeded(res);
        }
        
        [WindowsFact]
        public void CanSerializeInstance()
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
                MI_SerializationFormat.MOF,
                out newSerializer);
            MIAssert.Succeeded(res);
            Assert.NotNull(newSerializer, "Expect newly created serializer to be non-null");

            byte[] serializedInstance;
            res = newSerializer.SerializeInstance(MI_SerializerFlags.None, toSerialize, out serializedInstance);
            MIAssert.Succeeded(res);

            string serializedString = Encoding.Unicode.GetString(serializedInstance);
            Assert.Equal("instance of TestInstance\n{\n    string = \"Test string\";\n};\n\n", serializedString, "Expect the serialized string to be the one we generated elsewhere");

            res = newSerializer.Close();
            MIAssert.Succeeded(res);
        }
    }
}
