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
        internal MI_Application Application { get; private set; }

        internal MI_Session Session { get; private set; }

        public NativeTestsBase(ApplicationFixture appFixture)
        {
            this.Application = appFixture.Application;
        }

        public NativeTestsBase(SessionFixture sessionFixture)
        {
            this.Application = sessionFixture.Application;
            this.Session = sessionFixture.Session;
        }
    }
}
