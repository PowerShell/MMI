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
    public class MOFSerializerTests : SerializerTestsBase
    {
        public MOFSerializerTests() : base(MI_SerializationFormat.MOF)
        {
        }

        [WindowsFact]
        public void CanSerializeInstance()
        {
            this.TestSerializationInput(SerializationTestData.CreateBasicSerializableTestInstance,
                SerializationTestData.BasicSerializableTestInstanceMOFRepresentation);
        }
    }
}
