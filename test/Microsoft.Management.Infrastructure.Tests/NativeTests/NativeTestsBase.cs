using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Management.Infrastructure.Native;
using MMI.Tests;

namespace MMI.Tests.Native
{
    public class NativeTestsBase
    {
        internal MI_Application Application { get { return ApplicationFixture.Application; } }

        internal MI_Session Session { get; private set; }

        protected NativeTestsBase()
        {

        }
        
        public NativeTestsBase(SessionFixture sessionFixture)
        {
            this.Session = sessionFixture.Session;
        }

        public NativeTestsBase(DeserializerFixture deserializerFixture)
        {
            this.Session = SessionFixture.CurrentFixture.Session;
        }
    }
}
