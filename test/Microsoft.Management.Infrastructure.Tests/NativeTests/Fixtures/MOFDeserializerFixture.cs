using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Management.Infrastructure.Native;
using MMI.Tests;

namespace MMI.Tests.Native
{
    public class MOFDeserializerFixture : DeserializerFixture
    {
        public MOFDeserializerFixture() : base(MI_SerializationFormat.MOF)
        {
        }
    }
}
