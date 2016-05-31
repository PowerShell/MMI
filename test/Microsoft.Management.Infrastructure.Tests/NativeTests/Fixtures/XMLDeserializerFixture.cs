using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Management.Infrastructure.Native;
using MMI.Tests;

namespace MMI.Tests.Native
{
    public class XMLDeserializerFixture : DeserializerFixture
    {
        public XMLDeserializerFixture() : base(MI_SerializationFormat.XML)
        {
        }
    }
}
