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
    public class XMLDeserializerTests : DeserializerTestsBase
    {
        public XMLDeserializerTests() : base(MI_SerializationFormat.XML)
        {
        }
        
        [WindowsFact]
        public void CanDeserializeInstance()
        {
            this.VerifyRoundTripInstance();
        }

        [WindowsFact]
        public void CanDeserializeClass()
        {
            this.VerifyRoundTripClass();
        }
    }
}
