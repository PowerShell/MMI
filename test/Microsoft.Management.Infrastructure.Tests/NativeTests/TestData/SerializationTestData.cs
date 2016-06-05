using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Management.Infrastructure.Native;
using MMI.Tests;

namespace MMI.Tests.Native
{
    internal static class SerializationTestData
    {
        internal static MI_Instance CreateBasicSerializableTestInstance()
        {
            MI_Instance toSerialize;
            MI_Result res = StaticFixtures.Application.NewInstance("TestInstance", null, out toSerialize);
            MIAssert.Succeeded(res);
            MI_Value valueToSerialize = MI_Value.NewDirectPtr();
            valueToSerialize.String = "Test string";
            res = toSerialize.AddElement("string", valueToSerialize, MI_Type.MI_STRING, MI_Flags.None);
            MIAssert.Succeeded(res);

            return toSerialize;
        }

        public const string BasicSerializableTestInstanceXMLRepresentation = "<INSTANCE CLASSNAME=\"TestInstance\"><PROPERTY NAME=\"string\" TYPE=\"string\" MODIFIED=\"TRUE\"><VALUE>Test string</VALUE></PROPERTY></INSTANCE>";
        public const string BasicSerializableTestInstanceMOFRepresentation = "instance of TestInstance\n{\n    string = \"Test string\";\n};\n\n";
    }
}
