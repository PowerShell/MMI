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
    public class ApplicationFixture
    {
        private readonly string ApplicationName = "MMINativeTests";

        private static Lazy<ApplicationFixture> CurrentFixture { get; set; }

        private MI_Application application;

        internal static MI_Application Application { get { return CurrentFixture.Value.application; } }

        static ApplicationFixture()
        {
            CurrentFixture = new Lazy<ApplicationFixture>(() => new ApplicationFixture());
        }

        ~ApplicationFixture()
        {
            this.Dispose();
        }

        private ApplicationFixture()
        {
            MI_Instance extendedError = null;
            MI_Application newApplication;
            MI_Result res = MI_Application.Initialize(ApplicationName, out extendedError, out newApplication);
            MIAssert.Succeeded(res, "Expect basic application initialization to succeed");
            this.application = newApplication;
        }

        private void Dispose()
        {
            if (this.application != null)
            {
                var shutdownTask = Task.Factory.StartNew(() => this.application.Close());
                bool completed = shutdownTask.Wait(TimeSpan.FromSeconds(5));
                Assert.True(completed, "MI_Application did not complete shutdown in the expected time - did you leave an object open?");
                MIAssert.Succeeded(shutdownTask.Result);
                ApplicationFixture.CurrentFixture = null;
            }
        }
    }
}
