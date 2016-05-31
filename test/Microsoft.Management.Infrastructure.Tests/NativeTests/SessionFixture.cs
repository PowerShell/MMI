using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Management.Infrastructure.Native;
using MMI.Tests;

namespace MMI.Tests.Native
{
    public class SessionFixture : IDisposable
    {
        ApplicationFixture appFixture;

        internal MI_Session Session { get; private set; }

        internal MI_Application Application { get; private set; }

        public SessionFixture()
        {
            appFixture = new ApplicationFixture();
            this.Application = appFixture.Application;

            MI_Session newSession;
            MI_Instance extendedError = null;
            MI_Result res = this.Application.NewSession(null,
                    null,
                    MI_DestinationOptions.Null,
                    MI_SessionCallbacks.Null,
                    out extendedError,
                    out newSession);
            MIAssert.Succeeded(res, "Expect simple NewSession to succeed");
            this.Session = newSession;
        }

        public virtual void Dispose()
        {
            if (this.Session != null)
            {
                this.Session.Close(IntPtr.Zero, null);
            }

            if (this.appFixture != null)
            {
                this.appFixture.Dispose();
            }
        }
    }
}
