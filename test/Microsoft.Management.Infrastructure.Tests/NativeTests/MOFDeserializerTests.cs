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
    }
}
