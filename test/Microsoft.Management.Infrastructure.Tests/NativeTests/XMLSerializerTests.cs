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
    public class XMLSerializerTests : SerializerTestsBase
    {
        public XMLSerializerTests() : base(MI_SerializationFormat.XML)
        {
        }
        
        [WindowsFact]
        public void CanSerializeInstance()
        {
            MI_Instance toSerialize;
            MI_Result res = this.Application.NewInstance("TestInstance", null, out toSerialize);
            MIAssert.Succeeded(res);
            MI_Value valueToSerialize = MI_Value.NewDirectPtr();
            valueToSerialize.String = "Test string";
            res = toSerialize.AddElement("string", valueToSerialize, MI_Type.MI_STRING, MI_Flags.None);
            MIAssert.Succeeded(res);

            byte[] serializedInstance;
            res = this.Serializer.SerializeInstance(MI_SerializerFlags.None, toSerialize, out serializedInstance);
            MIAssert.Succeeded(res);

            string serializedString = Encoding.Unicode.GetString(serializedInstance);
            Assert.Equal("<INSTANCE CLASSNAME=\"TestInstance\"><PROPERTY NAME=\"string\" TYPE=\"string\" MODIFIED=\"TRUE\"><VALUE>Test string</VALUE></PROPERTY></INSTANCE>",
                serializedString, "Expect the serialized representation to be as created with PowerShell");
        }
    }
}
