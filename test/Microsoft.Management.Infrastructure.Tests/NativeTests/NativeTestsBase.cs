using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Management.Infrastructure.Native;
using MMI.Tests;

namespace MMI.Tests.Native
{
    public class NativeTestsBase : IDisposable
    {
        private readonly string ApplicationName = "MMINativeTests";

        internal MI_Application application = null;

        public NativeTestsBase()
        {
            MI_Instance extendedError = null;
            MI_Result res = MI_Application.Initialize(ApplicationName, out extendedError, out this.application);
            MIAssert.Succeeded(res, "Expect basic application initialization to succeed");
        }

        public virtual void Dispose()
        {
            if (this.application != null)
            {
                var shutdownTask = Task.Factory.StartNew(() => this.application.Close());
                bool completed = shutdownTask.Wait(TimeSpan.FromSeconds(5));
                Assert.True(completed, "MI_Application did not complete shutdown in the expected time - did you leave an object open?");
                MIAssert.Succeeded(shutdownTask.Result);
            }
        }
    }
}
