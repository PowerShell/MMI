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
            this.TestSerializationInput(SerializationTestData.CreateBasicSerializableTestInstance,
                SerializationTestData.BasicSerializableTestInstanceXMLRepresentation);
        }
    }
}
